using UnityEngine;
using UnityEngine.UI;
[CreateAssetMenu(fileName = "New Card", menuName = "Deckbuilder/Card Data")]
public class CardData : ScriptableObject
{
    public string cardName;
    public int manaCost;
    public int damage;
    public int defense;
    public int heal;
    public enum CardType { Attack, Defense, Heal}
    public CardType cardType;
    public Sprite cardArt;
    public string description;
}
