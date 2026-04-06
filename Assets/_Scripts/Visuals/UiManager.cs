using DG.Tweening;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class UiManager : MonoBehaviour
{
    [SerializeField] private Image backgroundUI;
    [SerializeField] private Color realityColor = Color.white;
    [SerializeField] private Color voidColor = new Color(0.5f, 0, 0.5f);

    [SerializeField] private TextMeshProUGUI manaText;

    [Header("End Screens")]
    [SerializeField] private GameObject victoryScreen;
    [SerializeField] private GameObject defeatScreen;

    private HandView.GameState lastKnownState = HandView.GameState.PlayerTurn;

    public void InitializeUi(HandView.GameState currentState, int currentMana, int maxMana)
    {
        SetGameState(currentState);
        UpdateManaDisplay(currentMana, maxMana);
    }

    public void SetGameState(HandView.GameState currentState)
    {
        lastKnownState = currentState;

        if (victoryScreen == null && currentState == HandView.GameState.Victory)
        {
            Debug.LogWarning("Victory state reached, but victoryScreen is not assigned on UiManager.");
        }

        if (defeatScreen == null && currentState == HandView.GameState.Defeat)
        {
            Debug.LogWarning("Defeat state reached, but defeatScreen is not assigned on UiManager.");
        }

        if (victoryScreen != null)
        {
            victoryScreen.SetActive(currentState == HandView.GameState.Victory);
        }

        if (defeatScreen != null)
        {
            defeatScreen.SetActive(currentState == HandView.GameState.Defeat);
        }
    }

    public void UpdateManaDisplay(int currentMana, int maxMana)
    {
        if (manaText == null)
        {
            return;
        }

        manaText.text = $"Mana: {currentMana}/{maxMana}";
    }

    public void UpdatePlayerHealthDisplay(TextMeshProUGUI healthDisplay, int currentHealth)
    {
        if (healthDisplay == null)
        {
            Debug.LogWarning("Player healthDisplay is not assigned.");
            return;
        }

        healthDisplay.text = $"Player Health: {currentHealth}";
    }

    public void UpdateEnemyHealthDisplay(TextMeshProUGUI healthDisplay, int currentHealth)
    {
        if (healthDisplay == null)
        {
            return;
        }

        healthDisplay.text = $"Enemy Health: {currentHealth}";
    }

    public void UpdateCardDisplay(
        TextMeshProUGUI cardName,
        TextMeshProUGUI manaCost,
        TextMeshProUGUI damage,
        TextMeshProUGUI defense,
        TextMeshProUGUI description,
        CardData cardData)
    {
        if (cardData == null)
        {
            return;
        }

        if (cardName != null)
        {
            cardName.text = cardData.cardName;
        }

        if (manaCost != null)
        {
            manaCost.text = cardData.manaCost.ToString();
        }

        if (damage != null)
        {
            damage.text = cardData.damage.ToString();
        }

        if (defense != null)
        {
            defense.text = cardData.defense.ToString();
        }

        if (description != null)
        {
            description.text = cardData.description;
        }
    }

    public void DimensionSwitchVisuals(Dimension currentDimension)
    {
        if (backgroundUI != null)
        {
            backgroundUI.color = currentDimension == Dimension.Reality ? realityColor : voidColor;
        }

        if (Camera.main != null)
        {
            Camera.main.DOShakeRotation(0.5f, 10f);
            Camera.main.DOShakePosition(0.5f, 10f);
        }
    }

    private void Start()
    {
        SetGameState(lastKnownState);
    }
}