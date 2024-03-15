using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UIDeathMenuController : MonoBehaviour
{
    [SerializeField] Button respawnButton;

    // Start is called before the first frame update
    void Start()
    {
        respawnButton.gameObject.SetActive(false);
        respawnButton.interactable = false;

        Singleton.Instance.RespawnAdsRewardedControllerInstance.RespawnAdsLoadedEvent += OnAdLoaded;
        Singleton.Instance.RespawnAdsRewardedControllerInstance.RespawnAdsClickedEvent += OnAdClicked;
        Singleton.Instance.RespawnAdsRewardedControllerInstance.RespawnAdsRewardEvent += OnAdReward;
    }

    void OnAdLoaded()
    {
        respawnButton.gameObject.SetActive(true);
        respawnButton.interactable = true;
    }

    void OnAdClicked()
    {
        Debug.Log("Ad was clicked!");
        OnAdReward();
    }

    void OnAdReward()
    {
        respawnButton.gameObject.SetActive(false);
        Singleton.Instance.LevelGenerationManagerInstance.ContinueLevel();
    }


    public void OnMainMenuButton() // Test button
    {
        Singleton.Instance.LevelGenerationManagerInstance.ResetLevel();
    }

    public void OnRespawnButton() // Test button
    {
        Singleton.Instance.RespawnAdsRewardedControllerInstance.ShowAd();
    }

    private void OnDisable()
    {

        Singleton.Instance.RespawnAdsRewardedControllerInstance.RespawnAdsLoadedEvent -= OnAdLoaded;
        Singleton.Instance.RespawnAdsRewardedControllerInstance.RespawnAdsClickedEvent -= OnAdClicked;
        Singleton.Instance.RespawnAdsRewardedControllerInstance.RespawnAdsRewardEvent -= OnAdReward;
    }
}
