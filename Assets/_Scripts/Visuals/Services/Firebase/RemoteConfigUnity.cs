
using System.Threading.Tasks;
using Unity.Services.RemoteConfig;
using Unity.Services.Authentication;
using Unity.Services.Core;
using UnityEngine;

public class ExampleSample : MonoBehaviour
{
    public struct userAttributes {}
    public struct appAttributes {}

    [SerializeField] private HandView handview;
    [SerializeField] private UiManager uiManager;


    async Task InitializeRemoteConfigAsync()
    {
            await UnityServices.InitializeAsync();

            if (!AuthenticationService.Instance.IsSignedIn)
            {
                await AuthenticationService.Instance.SignInAnonymouslyAsync();
            }
    }

    async Task Start()
    {
        if (Utilities.CheckForInternetConnection())
        {
            await InitializeRemoteConfigAsync();
        }

        RemoteConfigService.Instance.FetchCompleted += ApplyRemoteSettings;
        RemoteConfigService.Instance.FetchConfigs(new userAttributes(), new appAttributes());
    }

    void ApplyRemoteSettings(ConfigResponse configResponse)
    {
        int maxManafromcofig = RemoteConfigService.Instance.appConfig.GetInt("manaCost");
        Debug.Log("Max Mana from config: " + maxManafromcofig);
        handview.MaxMana = maxManafromcofig;
        Debug.Log("Max Mana after config: " + handview.MaxMana);
        uiManager.UpdateManaDisplay(handview.CurrentMana, handview.MaxMana);
    }
}