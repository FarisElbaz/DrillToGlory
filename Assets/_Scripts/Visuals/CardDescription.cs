using UnityEngine;
using TMPro;
using UnityEngine.UI;

public class CardDescription : MonoBehaviour
{

    [SerializeField] private GameObject cardPanel;
    [SerializeField] private TextMeshProUGUI descriptionText;
    [SerializeField] private TextMeshProUGUI titleText;
    [SerializeField] private TextMeshProUGUI manaCostText;
    [SerializeField] private TextMeshProUGUI actionValueText;
    [SerializeField] private Image cardArt;

    private CardData currentCardData;

    public void UpdateCardPanel(CardData cardData)
    {
        if(cardData.cardType == CardData.CardType.Attack)
        {
            actionValueText.text = "Damage: " + cardData.damage;
        }
        else if(cardData.cardType == CardData.CardType.Defense)
        {
            actionValueText.text = "Defense: " + cardData.defense;
        }
        else if(cardData.cardType == CardData.CardType.Heal)
        {
            actionValueText.text = "Heal: " + cardData.heal;
        }
        titleText.text = cardData.cardName;
        manaCostText.text = "Mana Cost: " + cardData.manaCost;
        descriptionText.text = cardData.description;
        cardArt.sprite = cardData.cardArt;
    }

    public void setCurrentCard(CardData cardData)
    {
        currentCardData = cardData;
    }

    public void OpenCardPanel()
    {
        UpdateCardPanel(currentCardData);
        cardPanel.SetActive(true);
    }

    public bool IsPanelActive()
    {
        return cardPanel.activeSelf;}

    public void CloseCardPanel()
    {
        cardPanel.SetActive(false);
    }
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        cardPanel.SetActive(false);
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
