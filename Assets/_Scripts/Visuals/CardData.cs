using UnityEngine;
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
    [TextArea(3,5)]
    public string description;
}
