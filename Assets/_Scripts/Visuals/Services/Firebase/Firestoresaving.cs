using UnityEngine;
using Firebase.Firestore;
using System.Threading.Tasks;
using System.Collections.Generic;

public class Firestoresaving : MonoBehaviour
{
    private FirebaseFirestore db;
    private AuthManager authManager;
    private int currenthighestRoom; 

    public int CurrentHighestRoom => currenthighestRoom;

    private Dictionary<string, int> leaderboardCache = new Dictionary<string, int>();

    public Dictionary<string, int> Top3Players => leaderboardCache;

    public void SaveHighestRoom(int highestRoom)
    {
        if (authManager == null || authManager.User == null)
        {
            Debug.LogWarning("Skipping Firestore save because no authenticated user is available.");
            return;
        }

        string userID = authManager.CurrentUserId.ToString();
        string userName = string.IsNullOrWhiteSpace(authManager.User.DisplayName) ? "Test Player" : authManager.User.DisplayName;
        DocumentReference leaderboard = db.Collection("Leaderboard").Document(userID);
        leaderboard.SetAsync(new { username = userName, highestroom = highestRoom});
    }

    public async Task GetCurrentHighest(string uid)
    {
        if (authManager == null || authManager.User == null || string.IsNullOrWhiteSpace(uid))
        {
            currenthighestRoom = 0;
            return;
        }

        DocumentReference leaderboard = db.Collection("Leaderboard").Document(uid);   
        DocumentSnapshot snapshot = await leaderboard.GetSnapshotAsync();

        if(snapshot.Exists)
        {
            if (snapshot.ContainsField("highestroom"))
            {
                currenthighestRoom = snapshot.GetValue<int>("highestroom");
            }
        }
        else
        {
            return;
        }
    }

    public async Task GetTop3()
    {
        if (authManager == null || authManager.User == null)
        {
            leaderboardCache.Clear();
            return;
        }

        leaderboardCache.Clear();
        Query top3Query = db.Collection("Leaderboard").OrderByDescending("highestroom").Limit(3);
        QuerySnapshot querySnapshot = await top3Query.GetSnapshotAsync();
        foreach (DocumentSnapshot documentSnapshot in querySnapshot.Documents)
        {
            if (documentSnapshot.ContainsField("username") && documentSnapshot.ContainsField("highestroom"))
            {
                string username = documentSnapshot.GetValue<string>("username");
                int highestRoom = documentSnapshot.GetValue<int>("highestroom");
                leaderboardCache[username] = highestRoom;
            }
        }
    }

    void Awake()
    {
        db = FirebaseFirestore.DefaultInstance;
        authManager = AuthManager.Instance;
    }
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
