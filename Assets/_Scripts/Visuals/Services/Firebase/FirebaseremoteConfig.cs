using System;
using Firebase;
using Firebase.RemoteConfig;
using UnityEngine;

public class FirebaseremoteConfig : MonoBehaviour
{
    [SerializeField] private Player player;

    async void Start()
    {
        var dependencyStatus = await FirebaseApp.CheckAndFixDependenciesAsync();
        if (dependencyStatus != DependencyStatus.Available)
        {
            Debug.LogWarning($"Firebase error: {dependencyStatus}");
            return;
        }

        var remoteConfig = FirebaseRemoteConfig.DefaultInstance;
        await remoteConfig.FetchAsync(TimeSpan.Zero);
        await remoteConfig.ActivateAsync();

        player.SetMaxHealth((int)remoteConfig.GetValue("maxHealth").LongValue);
    }
}
