
using Firebase.Analytics;
using System;
using System.Collections;
using UnityEngine;

public class AdEnums
{
    public enum ShowType
    {
        NO_AD = 0,
        INTERSTITIAL = 1,
        VIDEO_REWARD = 2,
    }
}

public class AdManager : Singleton<AdManager>
{
    string bannerAdUnitId = "6e0a05a55690345d";
    string rewardVideoAdUnitID = "c2a229f518508501";
    string interstitialUnitId = "e340bd9f14c311cd";
    private int stepLoadAds = 0;
    Action OnEarnRewardAction = null;

    public override void Start()
    {
        InitializeBannerAds();
      // stepLoadAds = 1;
        //InitializeInterstitialAds();
    }

    //MaxMediationController MaxMediation;
    //public bool IsInterstitialLoaded(int id = 1)
    //{

    //    return true;
    //}
    //public bool ShowInterstitial(string _placement, int id = 1)
    //{
    //    if (!IsInterstitialLoaded(id))
    //    {
    //        return false;
    //    }
    //    switch (id)
    //    {
    //        case (int)AdEnums.ShowType.NO_AD:
    //            return true;
    //        case (int)AdEnums.ShowType.INTERSTITIAL:
    //            AppOpenAdManager.ResumeFromAds = true;
    //            MaxMediation.ShowInterstitial(_placement);
    //            return true;
    //        case (int)AdEnums.ShowType.VIDEO_REWARD:
    //            AppOpenAdManager.ResumeFromAds = true;
    //            MaxMediation.ShowInterstitial(_placement);
    //            return true;
    //        default:
    //            return false;
    //    }

    //}
    //private void OnInterstitialDismissedEvent(string adUnitId, MaxSdkBase.AdInfo adInfo)
    //{
    //    LoadInterstitial();
    //    AppOpenAdManager.ResumeFromAds = false;
    //}
    //private void OnRewardedDismissedEvent(string adUnitId, MaxSdkBase.AdInfo adInfo)
    //{
    //    LoadRewardedAd();
    //    AppOpenAdManager.ResumeFromAds = false;
    //}

    //private void LoadRewardedAdsOLD()
    //{
    //    MaxMediation.ShowInterstitial("EndLevel");
    //    MaxMediation.ShowInterstitial("Retry");
    //}


    #region BannerAds
    public void ShowAdsOpen()
    {
        Debug.Log("Call AdsOpen ShowAdIfReady in Admanager");

        AppOpenManager.Ins.ShowAdIfReady();
    }
    
    public void InitializeBannerAds()
    {
        // You may call the utility method MaxSdkUtils.isTablet() to help with view sizing adjustments
        MaxSdk.CreateBanner(bannerAdUnitId, MaxSdkBase.BannerPosition.BottomCenter);
        // Set background or background color for banners to be fully functional
        MaxSdk.SetBannerBackgroundColor(bannerAdUnitId, UnityEngine.Color.white);

        MaxSdkCallbacks.Banner.OnAdLoadedEvent += OnBannerAdLoadedEvent;
        //MaxSdkCallbacks.Banner.OnAdLoadFailedEvent += OnBannerAdLoadFailedEvent;
        //MaxSdkCallbacks.Banner.OnAdClickedEvent += OnBannerAdClickedEvent;
        //MaxSdkCallbacks.Banner.OnAdRevenuePaidEvent += OnBannerAdRevenuePaidEvent;
        //MaxSdkCallbacks.Banner.OnAdExpandedEvent += OnBannerAdExpandedEvent;
        //MaxSdkCallbacks.Banner.OnAdCollapsedEvent += OnBannerAdCollapsedEvent;
    }

    private void OnBannerAdLoadedEvent(string adUnitId, MaxSdkBase.AdInfo adInfo)
    {
        if (!GameManager.removeAds)
        {
            MaxSdk.ShowBanner(bannerAdUnitId);
        }
        if (stepLoadAds==0)
        {
            stepLoadAds = 1;
            InitializeInterstitialAds();

        }
    }
    public void HideBannerAds()
    {
        MaxSdk.HideBanner(bannerAdUnitId);
    }


    //private void OnBannerAdLoadFailedEvent(string adUnitId, MaxSdkBase.ErrorInfo errorInfo) { }

    //private void OnBannerAdClickedEvent(string adUnitId, MaxSdkBase.AdInfo adInfo) { }

    //private void OnBannerAdRevenuePaidEvent(string adUnitId, MaxSdkBase.AdInfo adInfo) { }

    //private void OnBannerAdExpandedEvent(string adUnitId, MaxSdkBase.AdInfo adInfo) { }

    //private void OnBannerAdCollapsedEvent(string adUnitId, MaxSdkBase.AdInfo adInfo) { }
    #endregion

    #region Reward Video

    int retryAttempt;

    public void InitializeRewardedAds()
    {
        // Attach callback
        MaxSdkCallbacks.Rewarded.OnAdLoadedEvent += OnRewardedAdLoadedEvent;
        MaxSdkCallbacks.Rewarded.OnAdLoadFailedEvent += OnRewardedAdLoadFailedEvent;
        MaxSdkCallbacks.Rewarded.OnAdDisplayedEvent += OnRewardedAdDisplayedEvent;
        //MaxSdkCallbacks.Rewarded.OnAdClickedEvent += OnRewardedAdClickedEvent;
        //MaxSdkCallbacks.Rewarded.OnAdRevenuePaidEvent += OnRewardedAdRevenuePaidEvent;
        MaxSdkCallbacks.Rewarded.OnAdHiddenEvent += OnRewardedAdHiddenEvent;
        MaxSdkCallbacks.Rewarded.OnAdDisplayFailedEvent += OnRewardedAdFailedToDisplayEvent;
        MaxSdkCallbacks.Rewarded.OnAdReceivedRewardEvent += OnRewardedAdReceivedRewardEvent;

        // Load the first rewarded ad
        LoadRewardedAd();
    }

    public bool CheckHasRewardAds(bool showNotice)
    {
        if (showNotice)
        {
            if (Application.internetReachability == NetworkReachability.NotReachable)
            {
                noInternetPopup.SetActive(true);
                StartCoroutine(HidePopUp(noInternetPopup, 1f));
            }
            else
            {
                if (!MaxSdk.IsRewardedAdReady(rewardVideoAdUnitID))
                {
                    noAdsPopup.SetActive(true);
                    StartCoroutine(HidePopUp(noAdsPopup, 1.5f));
                }
            }
        }
        return MaxSdk.IsRewardedAdReady(rewardVideoAdUnitID);
    }
    IEnumerator HidePopUp(GameObject popup, float timeWait)
    {
        yield return new WaitForSeconds(timeWait);
        popup.SetActive(false);
    }
    private void LoadRewardedAd()
    {
        if (!MaxSdk.IsRewardedAdReady(rewardVideoAdUnitID))
        {
            MaxSdk.LoadRewardedAd(rewardVideoAdUnitID);
        }
    }

    private void OnRewardedAdLoadedEvent(string adUnitId, MaxSdkBase.AdInfo adInfo)
    {
        // Rewarded ad is ready for you to show. MaxSdk.IsRewardedAdReady(adUnitId) now returns 'true'.
        // Reset retry attempt
        retryAttempt = 0;
    }

    private void OnRewardedAdLoadFailedEvent(string adUnitId, MaxSdkBase.ErrorInfo errorInfo)
    {
        // Rewarded ad failed to load 
        // AppLovin recommends that you retry with exponentially higher delays, up to a maximum delay (in this case 64 seconds).
        retryAttempt++;
        double retryDelay = Math.Pow(2, Math.Min(6, retryAttempt));
        //double retryDelay = 2* Math.Min(6, retryAttempt);
        Invoke("LoadRewardedAd", (float)retryDelay);
        Debug.Log("MAX > Rewarded ad failed to load with error code: " + errorInfo.Code);
    }

    private void OnRewardedAdDisplayedEvent(string adUnitId, MaxSdkBase.AdInfo adInfo) {
        //AppOpenAdManager.ResumeFromAds = false;
        if (OnEarnRewardAction != null)
        {
            OnEarnRewardAction.Invoke();
        }
    }

    private void OnRewardedAdFailedToDisplayEvent(string adUnitId, MaxSdkBase.ErrorInfo errorInfo, MaxSdkBase.AdInfo adInfo)
    {
        // Rewarded ad failed to display. AppLovin recommends that you load the next ad.
        LoadRewardedAd();
    }

    private void OnRewardedAdClickedEvent(string adUnitId, MaxSdkBase.AdInfo adInfo) { }

    private void OnRewardedAdHiddenEvent(string adUnitId, MaxSdkBase.AdInfo adInfo)
    {
        //AppOpenAdManager.ResumeFromAds = false;
        // Rewarded ad is hidden. Pre-load the next ad
        LoadRewardedAd();
    }

    private void OnRewardedAdReceivedRewardEvent(string adUnitId, MaxSdk.Reward reward, MaxSdkBase.AdInfo adInfo)
    {
        // The rewarded ad displayed and the user should receive the reward.
        
    }

    private void OnRewardedAdRevenuePaidEvent(string adUnitId, MaxSdkBase.AdInfo adInfo)
    {
        // Ad revenue paid. Use this callback to track user revenue.
    }
    public bool ShowVideoAds(string _placement, Action rewardAction)
    {
        if (GameManager.debug)
        {
            rewardAction.Invoke();
            return true; 
        }
        else
        {
            if (MaxSdk.IsRewardedAdReady(rewardVideoAdUnitID))
            {
                
                //CheckFrank
                //AppOpenAdManager.ResumeFromAds = true;
                OnEarnRewardAction = rewardAction;
                //if (GameManager.Instance.isFirebaseReady)
                //    {
                //        FirebaseAnalytics.LogEvent("rewarded_video_show",
                //      new Parameter(
                //        "level", "CameraMode"),
                //      new Parameter(
                //        "placement", _placement)
                //        );
                //    }

                //}
                //else
                //{
                //    if (GameManager.Instance.isFirebaseReady)
                //    {
                //        FirebaseAnalytics.LogEvent("rewarded_video_show",
                //      new Parameter(
                //        "level", GameManager.Instance.currentLevel.ToString()),
                //      new Parameter(
                //        "time", _placement)
                //        );
                //    }
                //}
                MaxSdk.ShowRewardedAd(rewardVideoAdUnitID, _placement);
                //GameManager.Instance.popUpAdsNotReadyGO.SetActive(true);
                return true;
            }
            else
            {
                //CheckFrank
                //GameManager.Instance.popUpAdsNotReadyGO.SetActive(true);
                return false;
            }
        }
        
    }
    #endregion

    #region Interstitial Ads

    int retryAttemptInter;
    [SerializeField] private GameObject noInternetPopup = null;
    [SerializeField] private GameObject noAdsPopup = null;

    public void InitializeInterstitialAds()
    {
        // Attach callback
        MaxSdkCallbacks.Interstitial.OnAdLoadedEvent += OnInterstitialLoadedEvent;
        MaxSdkCallbacks.Interstitial.OnAdLoadFailedEvent += OnInterstitialLoadFailedEvent;
        MaxSdkCallbacks.Interstitial.OnAdDisplayedEvent += OnInterstitialDisplayedEvent;
        MaxSdkCallbacks.Interstitial.OnAdClickedEvent += OnInterstitialClickedEvent;
        MaxSdkCallbacks.Interstitial.OnAdHiddenEvent += OnInterstitialHiddenEvent;
        MaxSdkCallbacks.Interstitial.OnAdDisplayFailedEvent += OnInterstitialAdFailedToDisplayEvent;

        // Load the first interstitial
        LoadInterstitial();
    }

    private void LoadInterstitial()
    {
        MaxSdk.LoadInterstitial(interstitialUnitId);
    }

    private void OnInterstitialLoadedEvent(string adUnitId, MaxSdkBase.AdInfo adInfo)
    {
        // Interstitial ad is ready for you to show. MaxSdk.IsInterstitialReady(adUnitId) now returns 'true'

        // Reset retry attempt
        retryAttemptInter = 0;
        if (stepLoadAds==1)
        {
            stepLoadAds = 2;
            InitializeRewardedAds();
        }
    }

    private void OnInterstitialLoadFailedEvent(string adUnitId, MaxSdkBase.ErrorInfo errorInfo)
    {
        // Interstitial ad failed to load 
        // AppLovin recommends that you retry with exponentially higher delays, up to a maximum delay (in this case 64 seconds)

        retryAttemptInter++;
        double retryDelay = Math.Pow(2, Math.Min(6, retryAttempt));

        Invoke("LoadInterstitial", (float)retryDelay);
    }

    private void OnInterstitialDisplayedEvent(string adUnitId, MaxSdkBase.AdInfo adInfo) {
       //AppOpenAdManager.ResumeFromAds = false;
    }

    private void OnInterstitialAdFailedToDisplayEvent(string adUnitId, MaxSdkBase.ErrorInfo errorInfo, MaxSdkBase.AdInfo adInfo)
    {
        // Interstitial ad failed to display. AppLovin recommends that you load the next ad.
        LoadInterstitial();
    }

    private void OnInterstitialClickedEvent(string adUnitId, MaxSdkBase.AdInfo adInfo) { }

    private void OnInterstitialHiddenEvent(string adUnitId, MaxSdkBase.AdInfo adInfo)
    {
        // Interstitial ad is hidden. Pre-load the next ad.
        //AppOpenAdManager.ResumeFromAds = false;
        LoadInterstitial();
    }

    public void showInterstitialAds(string _placement)
    {
        if (GameManager.removeAds||GameManager.debug)
        {
            return;
        }
        if (MaxSdk.IsInterstitialReady(interstitialUnitId))
        {
            //AppOpenAdManager.ResumeFromAds = true;
            //if (ImageDataFromCamera.Instance.cameraMode)
            //{
            //    if (GameManager.Instance.isFirebaseReady)
            //    {
            //        FirebaseAnalytics.LogEvent("Interstitial_show",
            //      new Parameter(
            //        "level", "CameraMode"),
            //      new Parameter(
            //        "placement", _placement)
            //    );
            //    }
            //}
            //else
            //{
            //    if (GameManager.Instance.isFirebaseReady)
            //    {
            //        FirebaseAnalytics.LogEvent("Interstitial_show",
            //      new Parameter(
            //        "level", GameManager.Instance.currentLevel.ToString()),
            //      new Parameter(
            //        "time", _placement)
            //    );
            //    }
            //}
            MaxSdk.ShowInterstitial(interstitialUnitId, _placement);
        }
    }
    #endregion
}
