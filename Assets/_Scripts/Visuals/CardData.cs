using UnityEngine;
[CreateAssetMenu(fileName = "New Card", menuName = "Deckbuilder/Card Data")]
public class CardData : ScriptableObject
{
    public string cardName;
    public int manaCost;
    public int damage;
    public int defense;
    public Sprite cardArt;
    [TextArea(3,5)]
    public string description;
}
