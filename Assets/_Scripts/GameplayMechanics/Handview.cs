using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.SceneManagement;

public enum Dimension { Reality, Void };

public class HandView : MonoBehaviour
{

    [SerializeField] private Upgrades upgrades;
    public enum DamageTarget { Player, Enemy }
    public Dimension currentDimension = Dimension.Reality;

    [SerializeField] private List<CardViews> cardsInHand = new List<CardViews>();

    [SerializeField] private DeckManager deckManager;

    [SerializeField] private RectTransform dropZone;

    [SerializeField] private CardViews cardPrefab;

    [SerializeField] private CardDescription cardDescription;

    public enum GameState { PlayerTurn, EnemyTurn, Victory, Defeat}

    public GameObject[] enemyPool;

    public Transform enemyLocation;

    [SerializeField] private GameState currentState;

    [SerializeField] private List<Enemy> enemies = new List<Enemy>();

    [Header("Enemy Spawning")]
    [SerializeField] private int baseEnemiesPerRoom = 2;

    [SerializeField] private Player playerstats;

    [SerializeField] private int roomCounter = 0;

    [SerializeField] private UiManager uiManager;

    [Header("Hand Arch Tuning")]
    [SerializeField] private float maxWidth = 400f;
    [SerializeField] private float curveHeight = 50f;
    [SerializeField] private float maxRotation = 15f;
    [SerializeField] private int targetHandSize = 5;

    [SerializeField] private int maxMana = 5;

    [SerializeField] private int currentMana = 0;

    [Header("Dimension Damage Modifiers")]
    [SerializeField] private float realityDamageToPlayerMultiplier = 1f;
    [SerializeField] private float realityDamageToEnemyMultiplier = 1f;
    [SerializeField] private float voidDamageToPlayerMultiplier = 1.25f;
    [SerializeField] private float voidDamageToEnemyMultiplier = 1.5f;

    public GameState CurrentState => currentState;
    public int CurrentMana => currentMana;
    public int MaxMana => maxMana;
    public int RoomCounter => roomCounter;
    public IReadOnlyList<Enemy> Enemies => enemies;

    public void SetGameState(GameState newState)
    {
        currentState = newState;
        if (uiManager != null)
        {
            uiManager.SetGameState(currentState);
        }
    }

    public void Spawner(int count)
    {
        if (enemyPool == null || enemyPool.Length == 0)
        {
            Debug.LogWarning("Enemy pool is empty. Spawner aborted.");
            return;
        }

        count = Mathf.Max(1, count);

        foreach (Enemy enemy in enemies)
        {
            if (enemy != null)
            {
                Destroy(enemy.gameObject);
            }
        }
        enemies.Clear();

        for (int i = 0; i < count; ++i)
        {
            GameObject prefab = enemyPool[Random.Range(0, enemyPool.Length)];
            GameObject random_enemy = Instantiate(prefab, enemyLocation, false);
            
            Enemy enemyComponent = random_enemy.GetComponent<Enemy>();
            if (enemyComponent == null)
            {
                Debug.LogWarning($"Spawned prefab '{prefab.name}' has no Enemy component.");
                Destroy(random_enemy);
                continue;
            }

            enemies.Add(enemyComponent);

            enemyComponent.SetHand(this);
            enemyComponent.SetupForRoom(roomCounter);

            Debug.Log($"Spawned enemy from pool: {prefab.name}");
        }

        Debug.Log($"Spawner complete. Active enemies: {enemies.Count}");
    }



    void Start()
    {
        enemies.RemoveAll(enemy => enemy == null);
        if (enemies.Count == 0)
        {
            Enemy[] foundEnemies = FindObjectsByType<Enemy>(FindObjectsSortMode.None);
            enemies.AddRange(foundEnemies.Where(enemy => enemy != null));
        }

        foreach (Enemy enemy in enemies)
        {
            enemy.SetHand(this);
        }

        if (playerstats == null)
        {
            playerstats = FindFirstObjectByType<Player>();
        }

        if (playerstats != null)
        {
            playerstats.SetHandView(this);
        }

        if (enemyPool != null && enemyPool.Length > 0)
        {
            int enemyCount = Mathf.Max(1, baseEnemiesPerRoom);
            Spawner(enemyCount);
        }
        else
        {
            foreach (Enemy enemy in enemies)
            {
                if (enemy != null)
                {
                    enemy.SetHand(this);
                    enemy.SetupForRoom(roomCounter);
                }
            }
        }

        cardsInHand.Clear(); 

        SetGameState(GameState.PlayerTurn);
        currentMana = maxMana;
        deckManager.initilizeDeck();

        DrawUpToHandSize();

        UpdateHandVisuals();
        if (uiManager != null)
        {
            uiManager.InitializeUi(currentState, currentMana, maxMana);
        }
    }

    public void UpdateHandVisuals()
    {
        cardsInHand.RemoveAll(card => card == null);

        int cardCount = cardsInHand.Count;

        for (int i = 0; i < cardCount; i++)
        {
            float t = 0f;
            if (cardCount > 1) 
            {
                t = ((float)i / (cardCount - 1)) * 2f - 1f;
            }

            float targetX = t * maxWidth;
            float targetY = -(t * t) * curveHeight;
            
            float targetRotZ = -t * maxRotation; 

            Vector3 targetPos = new Vector3(targetX, targetY, 0f);

            cardsInHand[i].SetBaseTargets(targetPos, targetRotZ, i);
            
            cardsInHand[i].SetHandView(this); 
        }
    }


    public void OnCardPlayed(CardViews playedCard, Enemy targetEnemy)
    {
        Debug.Log("OnCardPlayed called");
        
        if (currentState == GameState.Victory || currentState == GameState.Defeat)
        {
            Debug.Log("Game is already over, cannot play card");
            return;
        }

        if (playedCard == null || playedCard.cardData == null)
        {
            Debug.LogWarning("Played card or card data is missing.");
            return;
        }

        if (currentState != GameState.PlayerTurn)
        {
            Debug.Log("Not player turn, current state: " + currentState);
            UpdateHandVisuals();
            return;
        }

        if (playedCard.cardData.manaCost > currentMana)
        {
            Debug.Log("Not enough mana to play this card!");
            UpdateHandVisuals();
            return;
        }
        if(playedCard.cardData.cardType == CardData.CardType.Heal)
        {
            playerstats.regenerate(playedCard.cardData.heal);
        }
        else if (playedCard.cardData.cardType == CardData.CardType.Defense)
        {
            playerstats.defend(playedCard.cardData.defense);
        }
        else if (playedCard.cardData.cardType == CardData.CardType.Attack)
        {
            int damage = ApplyDimensionDamageModifier(playedCard.cardData.damage, DamageTarget.Enemy);
            if(upgrades.BaseDamage > 0)
            {
                damage += Mathf.RoundToInt(upgrades.BaseDamage);
            }
            if (targetEnemy != null)
            {
                Debug.Log("Dealing " + damage + " damage to enemy");
                targetEnemy.DamageTaken(damage);
            }
            else
            {
                Debug.LogWarning("No target enemy selected.");
            }
        }

        currentMana -= playedCard.cardData.manaCost;
        deckManager.discardCardsAdd(playedCard.cardData);
        cardsInHand.Remove(playedCard);

        UpdateHandVisuals();

        Debug.Log("Played card: " + playedCard.name);
        Destroy(playedCard.gameObject); 

        if (uiManager != null)
        {
            uiManager.UpdateManaDisplay(currentMana, maxMana);
        }
    }

    public int ApplyDimensionDamageModifier(int baseDamage, DamageTarget target)
    {
        float multiplier = 1f;

        if (currentDimension == Dimension.Reality)
        {
            multiplier = target == DamageTarget.Player
                ? realityDamageToPlayerMultiplier
                : realityDamageToEnemyMultiplier;
        }
        else if (currentDimension == Dimension.Void)
        {
            multiplier = target == DamageTarget.Player
                ? voidDamageToPlayerMultiplier
                : voidDamageToEnemyMultiplier;
        }

        int modifiedDamage = Mathf.Max(0, Mathf.RoundToInt(baseDamage * multiplier));
        return modifiedDamage;
    }

    public void onDrawCardButton()
    {
        if (!DrawCard())
        {
            return;
        }
        onEndTurn();
    }

    private bool DrawCard()
    {
        if (currentState != GameState.PlayerTurn)
        {
            return false;
        }

        if(!deckManager.DrawCard(out CardData drawnData))
        {
            return false;
        }
        CardViews newCard = Instantiate(cardPrefab, transform);
        newCard.injection(drawnData, dropZone, this, cardDescription);
        cardsInHand.Add(newCard);
        UpdateHandVisuals();
        if (uiManager != null)
        {
            uiManager.UpdateManaDisplay(currentMana, maxMana);
        }

        return true;
    }

    public void onEndTurn()
    {
        Debug.Log("onEndTurn called, current state: " + currentState);

        if( playerstats == null)
        {
            Debug.LogWarning("Player stats reference is missing.");
            SetGameState(GameState.PlayerTurn);
            return;
        }

        if (currentState != GameState.PlayerTurn)
        {
            Debug.Log("Not player's turn, cannot end turn");
            return;
        }

        ReturnHandToDeckAndShuffle();

        Debug.Log("Transitioning to Enemy Turn");
        SetGameState(GameState.EnemyTurn);

        Debug.Log("Enemy dealing damage to player");

        var validEnemies = enemies.Where(e => e != null).ToList();

        foreach (var enemy in validEnemies)
        {
            enemy.dealDamage(playerstats);
        }
        playerstats.Defense = 0;
        BackToPlayerTurn();
    }

    private void ReturnHandToDeckAndShuffle()
    {
        cardsInHand.RemoveAll(card => card == null);

        deckManager.ReturnHandToDeckAndShuffle(cardsInHand);

        foreach (CardViews card in cardsInHand)
        {
            if (card != null)
            {
                Destroy(card.gameObject);
            }
        }

        cardsInHand.Clear();
        UpdateHandVisuals();
    }

    private void DrawUpToHandSize()
    {
        targetHandSize = Mathf.Max(0, targetHandSize);

        cardsInHand.RemoveAll(card => card == null);

        while (cardsInHand.Count < targetHandSize)
        {
            if (!DrawCard())
            {
                break;
            }
        }
    }

    public void RemoveEnemy(Enemy deadEnemy)
    {
        enemies.RemoveAll(e => e == null || e == deadEnemy || e.CurrentHealth <= 0);
        CheckForVictory();
    }

    private void CheckForVictory()
    {
        if (currentState == GameState.Defeat)
        {
            return;
        }

        enemies.RemoveAll(e => e == null || e.CurrentHealth <= 0);

        Enemy[] activeSceneEnemies = FindObjectsByType<Enemy>(FindObjectsSortMode.None);
        bool hasLivingEnemyInScene = activeSceneEnemies.Any(e => e != null && e.CurrentHealth > 0);

        if (!hasLivingEnemyInScene && enemies.Count == 0)
        {
            Debug.Log("All enemies defeated, transitioning to Victory state");
            SetGameState(GameState.Victory);
        }
    }

    public void BackToPlayerTurn()
    {
        if (currentState == GameState.Victory || currentState == GameState.Defeat)
        {
            return;
        }

        Debug.Log("Transitioning from Enemy Turn to Player Turn - 5 second delay starting");
        StartCoroutine(TransitionToPlayerTurnDelayed());
    }

    private IEnumerator TransitionToPlayerTurnDelayed()
    {
        yield return new WaitForSeconds(5f);
        Debug.Log("Transitioning to Player Turn NOW");
        
        StartPlayerTurn();
    }

    public void StartPlayerTurn()
    {
        SetGameState(GameState.PlayerTurn);
        currentMana = maxMana;
        DrawUpToHandSize();
        UpdateHandVisuals();
        if (uiManager != null)
        {
            uiManager.UpdateManaDisplay(currentMana, maxMana);
        }
    }

    public void IncreaseMaxMana( bool refill= true)
    {
        maxMana += 1;
        if(refill)
        {
            currentMana = maxMana;
        }
        if (uiManager != null)
        {
            uiManager.UpdateManaDisplay(currentMana, maxMana);
        }
    }


    public void NextRoom()
    {
        roomCounter += 1;
        if(upgrades != null)
        {
            upgrades.ApplyroomRegen();
            upgrades.UpgradePoints += 1;
            if(upgrades.UpgradePoints > 0)
            {
                upgrades.ToggleUpgrades();
            }
            else
            {
                Debug.Log("No upgrade points available");
            }
        }

        Debug.Log("Room Counter: " + roomCounter);

        foreach (CardViews card in cardsInHand)
        {
            deckManager.discardCardsAdd(card.cardData);
            Destroy(card.gameObject);
        }
        cardsInHand.Clear();

        SetGameState(GameState.PlayerTurn);
        currentMana = maxMana;
        if (uiManager != null)
        {
            uiManager.UpdateManaDisplay(currentMana, maxMana);
        }
        UpdateHandVisuals();

        int enemyCount = Mathf.Max(1, baseEnemiesPerRoom + roomCounter);
        Spawner(enemyCount);

        DrawUpToHandSize();
        UpdateHandVisuals();
    }

    public void DimensionSwitchVisuals()
    {
        currentDimension = currentDimension == Dimension.Reality ? Dimension.Void : Dimension.Reality;

        if (uiManager != null)
        {
            uiManager.DimensionSwitchVisuals(currentDimension);
        }
    }

    public void ReturnToStart()
    {
        if(currentState == GameState.Defeat)
        {
            SceneManager.LoadScene(1);
        }
    }
}