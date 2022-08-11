using System;
using UnityEngine;
using UnityEngine.Advertisements;
using UnityEngine.Events;

public class AdsManager : MonoBehaviour, IUnityAdsInitializationListener, IUnityAdsLoadListener, IUnityAdsShowListener
{
    [SerializeField] string _androidGameId;
    [SerializeField] string _iOSGameId;
    [SerializeField] bool _testMode = true;
    [SerializeField] bool startShowBannerAd = false;
    [SerializeField] BannerPosition startBannerPositionAd = BannerPosition.BOTTOM_CENTER;

    private string platform;
    private Action onRewardAdCompleted;
    private Action onInterstitialAdCompleted;


    //Singleton implementation
    public static AdsManager Instance { get; private set; }
    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(this.gameObject);
        }
        else
        {
            Instance = this;
            InitializeAds();
            DontDestroyOnLoad(gameObject);

            if (startShowBannerAd) ShowBannerAd(startBannerPositionAd);
        }
    }


    public void InitializeAds()
    {
        var _gameId = (Application.platform == RuntimePlatform.IPhonePlayer) ? _iOSGameId : _androidGameId;
        platform = (Application.platform == RuntimePlatform.IPhonePlayer) ? "iOS" : "Android";
        Advertisement.Initialize(_gameId, _testMode, this);
    }


    public void OnUnityAdsFailedToLoad(string placementId, UnityAdsLoadError error, string message)
    {
        Debug.Log($"Error showing Ad Unit {placementId}: {error.ToString()} - {message}");
    }

    public void OnUnityAdsShowFailure(string placementId, UnityAdsShowError error, string message)
    {
        Debug.Log("OnUnityAdsShowFailure");
    }

    public void OnUnityAdsShowStart(string placementId)
    {
        Time.timeScale = 0;
        Advertisement.Banner.Hide();
    }



    public void OnUnityAdsShowComplete(string placementId, UnityAdsShowCompletionState showCompletionState)
    {
        if (placementId.Equals($"Rewarded_{platform}"))
        {
            if (UnityAdsShowCompletionState.COMPLETED.Equals(showCompletionState))
            {
                onRewardAdCompleted?.Invoke();
            }
        }
        else if (placementId.Equals($"Interstitial_{platform}"))
        {
            onInterstitialAdCompleted?.Invoke();
        }

        Time.timeScale = 1;
    }

    // Interstitial
    public void ShowInterstitialAd(Action onAdCompleted)
    {
        this.onInterstitialAdCompleted = onAdCompleted;
        Advertisement.Show($"Interstitial_{platform}", this);
    }


    // Rewarded
    public void ShowRewardedAd(Action onAdCompleted)
    {
        this.onRewardAdCompleted = onAdCompleted;
        Advertisement.Show($"Rewarded_{platform}", this);
    }


    //Banners
    public void ShowBannerAd(BannerPosition bannerPosition = BannerPosition.BOTTOM_CENTER)
    {
        if (Advertisement.Banner.isLoaded)
        {
            Advertisement.Banner.Show($"Banner_{platform}");
        }
        else
        {
            Advertisement.Banner.SetPosition(bannerPosition);
            Advertisement.Banner.Load($"Banner_{platform}",
                new BannerLoadOptions
                {
                    loadCallback = OnBannerLoaded,
                    errorCallback = OnBannerError
                }
            );
        }
    }

    public void HideBannerAd()
    {
        Advertisement.Banner.Hide();
    }

    void OnBannerLoaded()
    {
        Advertisement.Banner.Show($"Banner_{platform}");
    }

    void OnBannerError(string message) { }
    public void OnInitializationComplete() { }
    public void OnInitializationFailed(UnityAdsInitializationError error, string message) { }
    public void OnUnityAdsAdLoaded(string placementId) { }
    public void OnUnityAdsShowClick(string placementId) { }
}