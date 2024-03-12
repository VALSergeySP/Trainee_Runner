using UnityEngine;
using UnityEngine.Advertisements;

public class AdsInitializationManager : MonoBehaviour, IUnityAdsInitializationListener
{
    [SerializeField] string androidGameId;
    [SerializeField] string iosGameId;

    [SerializeField] bool isTestingMode = true;
    string gameId;

    void Awake()
    {
        InitializeAdsMethod();
    }

    private void InitializeAdsMethod()
    {
#if UNITY_IOS
        gameId = iosGameId;
#elif UNITY_ANDROID
        gameId = androidGameId;
#elif UNITY_EDITOR
        gameId = androidGameId; // Test
#endif

        if (!Advertisement.isInitialized && Advertisement.isSupported)
        {
            Advertisement.Initialize(gameId, isTestingMode, this);
        }
    }

    public void OnInitializationComplete()
    {
        print("Ads initialized!");
    }

    public void OnInitializationFailed(UnityAdsInitializationError error, string message)
    {
        print("Ads not initialized!");
    }
}
