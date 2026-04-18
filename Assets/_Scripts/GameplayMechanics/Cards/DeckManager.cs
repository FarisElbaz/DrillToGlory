using System.Collections.Generic;
using UnityEngine;

public class DeckManager : MonoBehaviour
{
    [Header("Cards in starting hand")]
    [SerializeField] private List<CardData> drawPile = new List<CardData>();

    [SerializeField] private List<CardData> discardCards = new List<CardData>();

    [SerializeField] private CardData attackCardPrefab;
    [SerializeField] private CardData defenseCardPrefab;
    [SerializeField] private CardData healCardPrefab;

    private int deckSize = 15;

    private int maxDamagecards = 10;
    private int minDamagecards = 7;
    private int maxDefensecards = 5;
    private int minDefensecards = 2;
    private int maxHealCards = 3;
    private int minHealCards = 0;

    public void InitilizeDeck(){
        int remainingCards = deckSize;
        drawPile = new List<CardData>();
        discardCards = new List<CardData>();

        int damageCardsToAdd = Random.Range(minDamagecards, maxDamagecards + 1);
        remainingCards -= damageCardsToAdd;
        int defenseCardsToAdd = Random.Range(minDefensecards, maxDefensecards + 1);
        remainingCards -= defenseCardsToAdd;
        int healCardsToAdd = remainingCards;
        healCardsToAdd = Mathf.Clamp(healCardsToAdd, minHealCards, maxHealCards);
        remainingCards -= healCardsToAdd;

        if(remainingCards > 0){
            while(remainingCards > 0)
            {
                int cardRoll = Random.Range(0, 3);
                switch (cardRoll)
                {
                    case 0:
                        if (damageCardsToAdd < maxDamagecards)
                        {
                            damageCardsToAdd++;
                            remainingCards--;
                        }
                        break;
                    case 1:
                        if (defenseCardsToAdd < maxDefensecards)                        {
                            defenseCardsToAdd++;
                            remainingCards--;
                        }
                        break;
                    case 2:
                        if (healCardsToAdd < maxHealCards)
                        {
                            healCardsToAdd++;
                            remainingCards--;
                        }
                        break;
                }
            }
        }

        if (damageCardsToAdd + defenseCardsToAdd + healCardsToAdd == deckSize)
        {
            for(int i = 0; i < damageCardsToAdd; i++)
            {
                drawPile.Add(attackCardPrefab);
            }
        for(int i = 0; i < defenseCardsToAdd; i++)
            {
                drawPile.Add(defenseCardPrefab);
            }
        for(int i = 0; i < healCardsToAdd; i++)
            {
                drawPile.Add(healCardPrefab);
            }

        CardRandomizer(drawPile);
        }
            else
            {
                Debug.LogError("Card distribution does not match deck size. Please check the min and max values for each card type.");
            }
    }

    public bool DrawCard(out CardData drawnData)
    {
        if(drawPile.Count == 0)
        {
            if(discardCards.Count > 0)
            {
                Debug.Log("Reshuffling discard pile into deck");
                drawPile.AddRange(discardCards);
                CardRandomizer(drawPile);
                discardCards.Clear();
            }
            else
            {
                Debug.Log("No cards left to draw!");
                drawnData = null;
                return false;
            }
        }

        drawnData = drawPile[0];
        drawPile.RemoveAt(0);
        return true;
    }

    public void discardCardsAdd(CardData cardData)
    {
        discardCards.Add(cardData);
    }

    public void ReturnHandToDeckAndShuffle(List<CardViews> cardsInHand)
    {
        if (cardsInHand.Count == 0)
        {
            if (drawPile.Count > 1)
            {
                CardRandomizer(drawPile);
            }
            return;
        }

        foreach (CardViews card in cardsInHand)
        {
            if (card != null && card.cardData != null)
            {
                drawPile.Add(card.cardData);
            }
        }

        if (drawPile.Count > 1)
        {
            CardRandomizer(drawPile);
        }
    }

    private static void CardRandomizer(List<CardData> cardList)
    {
        for (int i = 0; i < cardList.Count; i++)
        {
            CardData temp = cardList[i];
            int randomIndex = Random.Range(i, cardList.Count);
            cardList[i] = cardList[randomIndex];
            cardList[randomIndex] = temp;
        }
    }
}
