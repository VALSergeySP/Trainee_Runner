using UnityEngine;
using UnityEngine.Advertisements;

public class RespawnAdsRewardedController : MonoBehaviour, IUnityAdsLoadListener, IUnityAdsShowListener
{
    public delegate void RespawnAds();
    public event RespawnAds RespawnAdsLoadedEvent;
    public event RespawnAds RespawnAdsClickedEvent;
    public event RespawnAds RespawnAdsRewardEvent;

    [SerializeField] string androidAdUnitId;
    [SerializeField] string iosAdUnitId;

    string adUnitId;

    void Awake()
    {
#if UNITY_IOS
        adUnitId = iosAdUnitId;
#elif UNITY_ANDROID
        adUnitId = androidAdUnitId;
#endif
    }

    void Start()
    {
        Singleton.Instance.LevelGenerationManagerInstance.StartLevelEvent += LoadAd; // Отписать события
    }

    public void LoadAd()
    {
        Advertisement.Load(adUnitId, this);
    }

    public void ShowAd()
    {
        Advertisement.Show(adUnitId, this);
    }

    public void OnUnityAdsAdLoaded(string placementId)
    {
        if (placementId.Equals(adUnitId))
        {
            print("Ad loaded!");
            RespawnAdsLoadedEvent?.Invoke();
        }
    }

    public void OnUnityAdsFailedToLoad(string placementId, UnityAdsLoadError error, string message)
    {
        print("Failed To Load!");
    }

    public void OnUnityAdsShowClick(string placementId)
    {
        print("Ads Show Click!");
        if (placementId.Equals(adUnitId))
        {
            RespawnAdsClickedEvent?.Invoke();
            print("Reward after clicking!");
        }
    }

    public void OnUnityAdsShowComplete(string placementId, UnityAdsShowCompletionState showCompletionState)
    {
        print("Complited!");
        if (placementId.Equals(adUnitId) && showCompletionState.Equals(UnityAdsShowCompletionState.COMPLETED))
        {
            print("Reward!");
            RespawnAdsRewardEvent?.Invoke();
        }
    }

    public void OnUnityAdsShowFailure(string placementId, UnityAdsShowError error, string message)
    {
        print("Failure!");
    }

    public void OnUnityAdsShowStart(string placementId)
    {
        print("Ads Show Start!");
    }
}
