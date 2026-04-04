using UnityEngine;
using TMPro;
using System.Linq;

public class Enemy : MonoBehaviour
{

    [SerializeField] private int health = 20;

    [SerializeField] private HandView hand;

    public int CurrentHealth { get; private set; }

    [SerializeField] private TextMeshProUGUI healthDisplay;

    EnemyType enemytype;

     public enum EnemyType
    {
        Aggro,
        Tank,
        Healer
    }

    public void SetHand(HandView handView)
    {
        hand = handView;
    }

    public void SetupForRoom(int roomCounter)
    {
        CurrentHealth = 20 + (roomCounter * 5);

        if (healthDisplay != null)
        {
            healthDisplay.text = $"Enemy Health: {CurrentHealth}";
        }
    }

    public void DamageTaken(int damage)
    {
        CurrentHealth -= damage;

        if (healthDisplay != null)
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

    public void TakeTurn(Player player)
    {
        switch (enemytype)
        {
            case EnemyType.Aggro:
                if (player != null && hand != null)
                {
                    int damage = hand.ApplyDimensionDamageModifier(10, HandView.DamageTarget.Player);
                    player.DamageTaken(damage);
                }
                break;
            case EnemyType.Tank:
                CurrentHealth += 20;
                break;
            case EnemyType.Healer:
                healAlly();
                break;
        }

    }

    void healAlly()
    {
        var target = hand.Enemies.OrderBy(e => e.CurrentHealth).FirstOrDefault();

        if(target != null) target.CurrentHealth += 5;

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

        int modifiedDamage = hand.ApplyDimensionDamageModifier(5, HandView.DamageTarget.Player);
        Debug.Log("Dealing " + modifiedDamage + " damage to player");
        player.DamageTaken(modifiedDamage);
    }

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        CurrentHealth = health;

        if (healthDisplay != null)
        {
            healthDisplay.text = $"Enemy Health: {CurrentHealth}";
        }
    }

}
