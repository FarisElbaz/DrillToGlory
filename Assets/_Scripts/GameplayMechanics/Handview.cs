using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public enum Dimension { Reality, Void };

public class HandView : MonoBehaviour
{

    [SerializeField] private Upgrades upgrades;
    public Dimension currentDimension = Dimension.Reality;

    [SerializeField] private List<CardViews> cardsInHand = new List<CardViews>();

    [SerializeField] private DeckManager deckManager;

    [SerializeField] private EnemySpawner enemyManager;

    [SerializeField] private RectTransform dropZone;

    [SerializeField] private CardViews cardPrefab;

    [SerializeField] private CardDescription cardDescription;
    [SerializeField] private CardPlayer cardPlayer;
    [SerializeField] private TurnController turnController;

    public enum GameState { PlayerTurn, EnemyTurn, Victory, Defeat}

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

    private bool SwitchedDimensionAlready = false;

    private void Awake()
    {
    }

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
            uiManager.SetGameState(currentState, roomCounter);
        }
    }

    void Start()
    {
        if (enemyManager != null && enemyManager.CanSpawn)
        {
            int enemyCount = Mathf.Max(1, baseEnemiesPerRoom);
            enemyManager.Spawner(enemyCount, this, enemies, roomCounter, uiManager);
        }

        cardsInHand.Clear(); 

        SetGameState(GameState.PlayerTurn);
        currentMana = maxMana;
        deckManager.InitilizeDeck();

        if (turnController != null)
        {
            turnController.Initialize(this, playerstats);
        }

        if (uiManager != null)
        {
            uiManager.UpdateManaDisplay(currentMana, maxMana);
        }

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

        if (currentState != GameState.PlayerTurn)
        {
            Debug.Log("Not player turn, current state: " + currentState);
            UpdateHandVisuals();
            return;
        }

        if (playedCard == null || playedCard.cardData == null)
        {
            Debug.LogWarning("Played card or card data is missing.");
            return;
        }

        if (playedCard.cardData.manaCost > currentMana)
        {
            Debug.Log("Not enough mana to play this card!");
            UpdateHandVisuals();
            return;
        }
        if (cardPlayer != null)
        {
            cardPlayer.CardClasses(playedCard.cardData, playerstats, targetEnemy, upgrades, currentDimension);
        }
        else
        {
            Debug.LogWarning("CardPlayer reference is missing.");
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

    public void onDrawCardButton()
    {
        if (currentState != GameState.PlayerTurn)
        {
            return;
        }

        ReturnHandToDeckAndShuffle();
        onEndTurn();
    }

    private bool DrawCard()
    {
        if (currentState != GameState.PlayerTurn || !deckManager.DrawCard(out CardData drawnData))
        {
            return false;
        }

        CardViews newCard = Instantiate(cardPrefab, transform);
        newCard.SetUp(drawnData, dropZone, this, cardDescription);
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
        if (turnController != null)
        {
            turnController.EndTurn();
            return;
        }

        Debug.LogWarning("TurnController reference is missing.");
    }

    public void ReturnHandToDeckAndShuffle()
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
        if (turnController != null)
        {
            turnController.HandleEnemyRemoved(deadEnemy);
            return;
        }

        Debug.LogWarning("TurnController reference is missing.");
        enemies.RemoveAll(e => e == null || e == deadEnemy || e.CurrentHealth <= 0);
    }

    public void StartPlayerTurn()
    {
        SetGameState(GameState.PlayerTurn);
        currentMana = maxMana;
        SwitchedDimensionAlready = false;
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
            upgrades.AddUpgradePoint();
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
        if (enemyManager != null)
        {
            enemyManager.Spawner(enemyCount, this, enemies, roomCounter, uiManager);
        }
        else
        {
            Debug.LogWarning("EnemySpawner reference is missing. Cannot spawn room enemies.");
        }

        DrawUpToHandSize();
        UpdateHandVisuals();
    }

    public void DimensionSwitch()
    {
        if (SwitchedDimensionAlready)
        {
            Debug.Log("Dimension switch already used this turn.");
            return;
        }

        SwitchedDimensionAlready = true;

        if(currentDimension == Dimension.Reality)
        {
            currentDimension = Dimension.Void;
            if (uiManager != null)
            {
                uiManager.DimensionSwitchVisuals(currentDimension);
            }
        }
        else
        {
            currentDimension = Dimension.Reality;
            if (uiManager != null)
            {
                uiManager.DimensionSwitchVisuals(currentDimension);
            }
        }
    }

    public void ReturnToStart()
    {
        if(currentState == GameState.Defeat)
        {
            BackgroundMusicPersistence.MuteMusic();
            SceneManager.LoadScene(2);
        }
    }
    public void ReturnToMainMenu()
    {
        if (currentState == GameState.Defeat || currentState == GameState.Victory)
        {
            SceneManager.LoadScene(1);
        }
    }
}