using UnityEngine;
using TMPro;
using System.Linq;

public class Enemy : MonoBehaviour
{

    [SerializeField] private int health = 20;

    [SerializeField] private HandView hand;
    [SerializeField] private UiManager uiManager;

    public int CurrentHealth { get; private set; }

    [SerializeField] private TextMeshProUGUI healthDisplay;

    [SerializeField] EnemyType enemytype;
    public enum EnemyType
    {
        Aggro,
        Tank,
        Healer
    }

    private void Awake()
    {
        CurrentHealth = health;
    }

    public void SetHand(HandView handView, UiManager uiManager)
    {
        hand = handView;
        this.uiManager = uiManager;
    }

    public void SetupForRoom(int roomCounter)
    {
        CurrentHealth = health + (roomCounter * 5);

        if (uiManager != null)
        {
            uiManager.UpdateEnemyHealthDisplay(healthDisplay, CurrentHealth);
        }
        else if (healthDisplay != null)
        {
            healthDisplay.text = $"Enemy Health: {CurrentHealth}";
        }
    }

    public void DamageTaken(int damage)
    {
        CurrentHealth -= damage;

        if (uiManager != null)
        {
            uiManager.UpdateEnemyHealthDisplay(healthDisplay, CurrentHealth);
        }
        else if (healthDisplay != null)
        {
            healthDisplay.text = $"Enemy Health: {CurrentHealth}";
        }

        if (CurrentHealth <= 0)
        {
            hand.RemoveEnemy(this);
            Destroy(gameObject);

            Debug.Log("Enemy Defeated!");
        }
    }

    void healAlly()
    {
        var target = hand.Enemies.OrderBy(e => e.CurrentHealth).FirstOrDefault();

        if(target != null) target.CurrentHealth += 5;
        uiManager.UpdateEnemyHealthDisplay(healthDisplay, target.CurrentHealth);

    }

    public void dealDamage(Player player)
    {
        Debug.Log("Enemy dealDamage called");
        
        if (player == null || hand == null)
        {
            Debug.LogWarning("Player or hand reference is missing.");
            return;
        }

        if (hand.CurrentState == HandView.GameState.Victory || hand.CurrentState == HandView.GameState.Defeat)
        {
            Debug.Log("Game is over");
            return;
        }

        switch (enemytype)
        {
            case EnemyType.Aggro:
                player.DamageTaken(10);
                break;
            case EnemyType.Tank:
                player.DamageTaken(5);
                break;
            case EnemyType.Healer:
                healAlly();
                break;
        }
    }

}
