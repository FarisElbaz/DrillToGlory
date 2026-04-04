using UnityEngine;
using Firebase.Firestore;
using System.Collections.Generic;

[FirestoreData]
public class PlayerData
{
    [FirestoreProperty]
    public string UserId { get; set; }
    [FirestoreProperty]
    public int floor { get; set; }
    [FirestoreProperty]
    public List<string> Deck { get; set; }

    public PlayerData() { }

    public PlayerData(string userId, int floor, List<string> deck)
    {
        UserId = userId;
        this.floor = floor;
        Deck = deck;
    }
}
