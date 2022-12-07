using UnityEngine;

public class MaxLoading : MonoBehaviour
{
    private bool firstOpen = false;

    public PreloadScene preload;

    //// Start is called before the first frame update
    void Start()
    {

        //MaxSdkCallbacks.OnSdkInitializedEvent += (MaxSdkBase.SdkConfiguration sdkConfiguration) =>
        MaxSdkCallbacks.OnSdkInitializedEvent += sdkConfiguration =>
        {
            Debug.Log("Init MAX Succes");
            // AppLovin SDK is initialized, start loading ads
            MaxSdkCallbacks.AppOpen.OnAdHiddenEvent += OnAppOpenDismissedEvent;
            MaxSdkCallbacks.AppOpen.OnAdLoadedEvent += OnAppLoadedEvent;
            MaxSdkCallbacks.AppOpen.OnAdLoadFailedEvent += OnAppLoadedFailedEvent;
            AppOpenManager.Ins.ShowAdIfReady();

        };
        MaxSdk.SetSdkKey("785rXkTy0HOAg_rPZmSPinn-8Df-soogR_ydkn0iFmlhb3tafHqERvKeKTdozmxlSDRpO-Oue23U26lzN379e6");
        //MaxSdk.SetUserId("USER_ID");
        MaxSdk.InitializeSdk();
    }
    // Start is called before the first frame update
    //void Start()
    //{
    //    MaxSdkCallbacks.OnSdkInitializedEvent += sdkConfiguration =>
    //    {
    //        MaxSdkCallbacks.AppOpen.OnAdHiddenEvent += OnAppOpenDismissedEvent;

    //        AppOpenManager.Ins.ShowAdIfReady();
    //    };

    //    MaxSdk.SetSdkKey("7PspscCcbGd6ohttmPcZTwGmZCihCW-Jwr7nSJN2a_9Mg0ERPs0tmGdKTK1gs__nr6XHQvK0vTNaTb1uR1mCIN");
    //    MaxSdk.InitializeSdk();
    //}

    public void OnAppOpenDismissedEvent(string adUnitId, MaxSdkBase.AdInfo adInfo)
    {
        MaxSdk.LoadAppOpenAd(AppOpenManager.Ins.AppOpenAdUnitId);
    }
    public void OnAppLoadedEvent(string adUnitId, MaxSdkBase.AdInfo adInfo)
    {
        if (!firstOpen)
        {
            firstOpen = true;
            preload.finishAOA = true;
            MaxSdk.ShowAppOpenAd(AppOpenManager.Ins.AppOpenAdUnitId);
        }


    }
    public void OnAppLoadedFailedEvent(string adUnitId, MaxSdkBase.ErrorInfo adInfo)
    {
        Debug.Log("on AppOpen Failed:" + adUnitId + "infor: " + adInfo);
        MaxSdk.LoadAppOpenAd(AppOpenManager.Ins.AppOpenAdUnitId);
    }

    private void OnApplicationPause(bool pauseStatus)
    {
        if (!pauseStatus)
        {
            AppOpenManager.Ins.ShowAdIfReady();
        }
    }
}
