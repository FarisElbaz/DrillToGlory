using TMPro;
using UnityEngine;

public class Player : MonoBehaviour
{
    [SerializeField] private int playerHealth = 30;
    [SerializeField] private int defense = 0;
    
    public int Defense { get => defense; set => defense = value; }

    public int CurrentPlayerHealth { get; private set; }

    [SerializeField] private HandView handView;
    [SerializeField] private UiManager uiManager;

    [SerializeField] private TextMeshProUGUI healthDisplay;
    [SerializeField] private TextMeshProUGUI defenseDisplay;

    [SerializeField] private Upgrades upgrades;



    public void SetHandView(HandView value)
    {
        handView = value;
    }

    public void DamageTaken(int damage)
    {
        Debug.Log("Player DamageTaken called with damage: " + damage);

        float totalDefense = defense + upgrades.BaseDefense;
        
        int effectiveDamage = Mathf.Max(damage - (int)totalDefense, 0);  
        CurrentPlayerHealth -= effectiveDamage;
        Debug.Log("Player health is now: " + CurrentPlayerHealth);
        
        UpdateHealthDisplay();
        
        if (CurrentPlayerHealth <= 0)
        {
            Debug.Log("Player health is 0 or less, setting game state to Defeat");
            
            if (handView != null)
            {
                handView.SetGameState(HandView.GameState.Defeat);
                Debug.Log("Game state set to Defeat");
            }
            else
            {
                Debug.LogWarning("handView is null");
            }

            Debug.Log("Player Defeated!");
        }
    }

    public void regenerate(int amount)
    {
        if(amount <= 0){return;}
        CurrentPlayerHealth += amount;
        if(CurrentPlayerHealth > playerHealth)
        {
            CurrentPlayerHealth = playerHealth;
        }
        UpdateHealthDisplay();
    }

    public void defend(int defenseAmount)
    {
        defense += defenseAmount;
        UpdateDefenseDisplay();
    }

    public void UpdateHealthDisplay()
    {
        if (uiManager == null)
        {
            if (healthDisplay != null)
            {
                healthDisplay.text = $"Player Health: {CurrentPlayerHealth}";
            }
            return;
        }

        uiManager.UpdatePlayerHealthDisplay(healthDisplay, CurrentPlayerHealth);
    }

    public void UpdateDefenseDisplay()
    {
        if (uiManager == null)
        {
            if (defenseDisplay != null)
            {
                defenseDisplay.text = $"Defense: {defense}";
            }
            return;
        }

        uiManager.UpdateDefenseDisplay(defenseDisplay, defense);
    }

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        CurrentPlayerHealth = playerHealth;
        UpdateHealthDisplay();
        UpdateDefenseDisplay();
    }

}
