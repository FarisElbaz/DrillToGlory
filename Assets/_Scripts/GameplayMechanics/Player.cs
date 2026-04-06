using TMPro;
using UnityEngine;

public class Player : MonoBehaviour
{
    [SerializeField] private int playerHealth = 30;

    public int CurrentPlayerHealth { get; private set; }

    [SerializeField] private HandView handView;
    [SerializeField] private UiManager uiManager;

    [SerializeField] private TextMeshProUGUI healthDisplay;

    public void SetHandView(HandView value)
    {
        handView = value;
    }

    public void DamageTaken(int damage)
    {
        Debug.Log("Player DamageTaken called with damage: " + damage);
        
        CurrentPlayerHealth -= damage;
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

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        CurrentPlayerHealth = playerHealth;
        UpdateHealthDisplay();
    }

}
