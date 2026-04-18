using DG.Tweening;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using System.Threading.Tasks;
using System.Linq;

public class UiManager : MonoBehaviour
{
    [Header("Dimension VFX")]
    [SerializeField] private Camera shakeCamera;
    [SerializeField] private float dimensionShakeDuration = 0.35f;
    [SerializeField] private float dimensionShakeStrength = 3f;

    [SerializeField] private Image backgroundUI;
    [SerializeField] private Color realityColor = Color.white;
    [SerializeField] private Color voidColor = new Color(0.5f, 0, 0.5f);

    [SerializeField] private TextMeshProUGUI manaText;

    [Header("End Screens")]
    [SerializeField] private GameObject victoryScreen;
    [SerializeField] private GameObject defeatScreen;

    [SerializeField] private GameObject leaderboardPanel; 

    [SerializeField] private TextMeshProUGUI firstPlace;
    [SerializeField] private TextMeshProUGUI secondPlace;
    [SerializeField] private TextMeshProUGUI thirdPlace;
    [SerializeField] private TextMeshProUGUI currentPlayer;

    [SerializeField] private TextMeshProUGUI recordedHighscore;
    
    [SerializeField] private Firestoresaving firestoreSaving;

    [SerializeField] private TextMeshProUGUI victoryLevelText;
    [SerializeField] private TextMeshProUGUI defeatLevelText;

    private HandView.GameState lastKnownState = HandView.GameState.PlayerTurn;

    public void InitializeUi(HandView.GameState currentState, int currentMana, int maxMana)
    {
        SetGameState(currentState);
        UpdateManaDisplay(currentMana, maxMana);
    }

    public void SetCurrentLevel(int currentLevel, HandView.GameState currentState)
    {
        string levelText = currentLevel.ToString();

        if (currentState == HandView.GameState.Victory)
        {
            if (victoryLevelText != null)
            {
                victoryLevelText.text = levelText;
            }
            return;
        }

        if (currentState == HandView.GameState.Defeat)
        {
            if (defeatLevelText != null)
            {
                defeatLevelText.text = levelText;
            }
        }
    }

    public void SetGameState(HandView.GameState currentState, int currentRoom = 0)
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

        if (currentState == HandView.GameState.Victory || currentState == HandView.GameState.Defeat)
        {
            SetCurrentLevel(currentRoom, currentState);
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

        healthDisplay.text = $"Health: {currentHealth}";
    }

    public void UpdateDefenseDisplay(TextMeshProUGUI defenseDisplay, int currentDefense)
    {
        if (defenseDisplay == null)
        {
            Debug.LogWarning("Defense display is not assigned.");
            return;
        }

        defenseDisplay.text = $"Defense: {currentDefense}";
    }

    public async Task UpdateLeaderBoard()
    {
        if (firestoreSaving == null)
        {
            Debug.LogError("Firestoresaving is not assigned on UiManager!");
            return;
        }
        // So i can test the game without logging in everytime.
        AuthManager authManager = AuthManager.Instance;
        if (authManager == null || authManager.User == null)
        {
            if (currentPlayer != null)
            {
                currentPlayer.text = "You (Offline Test)";
            }

            if (recordedHighscore != null)
            {
                recordedHighscore.text = "0";
            }

            if (firstPlace != null)
            {
                firstPlace.text = "Leaderboard requires login";
            }

            if (secondPlace != null)
            {
                secondPlace.text = "";
            }

            if (thirdPlace != null)
            {
                thirdPlace.text = "";
            }

            return;
        }

        await firestoreSaving.GetTop3();
        var top3 = firestoreSaving.Top3Players;
        
        string currentUserId = authManager.CurrentUserId;
        await firestoreSaving.GetCurrentHighest(currentUserId);
        int currentUserScore = firestoreSaving.CurrentHighestRoom;
        
        Debug.Log($"Leaderboard updated - Top 3 count: {top3.Count}, Current user score: {currentUserScore}");
        
        string userName;
        if (authManager.User.DisplayName != null)
        {
            userName = authManager.User.DisplayName;
        }
        else
        {
            userName = "Test Player";
        }
        if (currentPlayer != null)
        {
            currentPlayer.text = $"You ({userName}):";
        }
        if (recordedHighscore != null)
        {
            recordedHighscore.text = currentUserScore.ToString();
        }
        
        // Display top 3
        if (top3.Count == 0)
        {
            firstPlace.text = "No players yet!";
            secondPlace.text = "";
            thirdPlace.text = "";
            return;
        }
        else
        {
            firstPlace.text = $"{top3.Keys.ElementAt(0)}: {top3.Values.ElementAt(0)}";
            secondPlace.text = $"{top3.Keys.ElementAt(1)}: {top3.Values.ElementAt(1)}";
            thirdPlace.text = $"{top3.Keys.ElementAt(2)}: {top3.Values.ElementAt(2)}";
        }
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
        Image cardArt,
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

        if (cardArt != null)
        {
            cardArt.sprite = cardData.cardArt;
        }
    }

    public void DimensionSwitchVisuals(Dimension currentDimension)
    {
        if (backgroundUI != null)
        {
            if(currentDimension == Dimension.Reality)
            {
                backgroundUI.DOColor(realityColor, 1f);
            }
            else
            {
                backgroundUI.DOColor(voidColor, 1f);
            }
        }

        
        if (Camera.main != null)
        {
            Camera.main.transform.DOShakeRotation(dimensionShakeDuration, dimensionShakeStrength);
            Camera.main.transform.DOShakePosition(dimensionShakeDuration, dimensionShakeStrength);
        }
        else
        {
            Debug.LogWarning("No camera found for shake. Assign shakeCamera on UiManager or tag one camera as MainCamera.");
        }
    }

    public void OpenLeaderboard()
    {
        if (leaderboardPanel != null)
        {
            leaderboardPanel.SetActive(true);
            _= UpdateLeaderBoard();
        }
    }

    public void CloseLeaderboard()
    {
        if (leaderboardPanel != null)
        {
            leaderboardPanel.SetActive(false);
        }
    }

    public void PlayGame()
    {
        DOTween.KillAll();
        BackgroundMusicPersistence.MuteMusic();
        SceneManager.LoadScene(2);
    }

    public void ReturnToMainMenu()
    {
        DOTween.KillAll();
        SceneManager.LoadScene(1);
    }

    public void Logout()
    {
        AuthManager.Instance.SignOut();
        SceneManager.LoadScene(0);
    }

    private void Start()
    {
        Application.targetFrameRate = 60;

        SetGameState(lastKnownState);

        if (leaderboardPanel != null)
        {
            leaderboardPanel.SetActive(false);
        }
    }


}