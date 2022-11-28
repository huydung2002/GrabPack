using UnityEngine;

public class MaxLoading : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        MaxSdkCallbacks.OnSdkInitializedEvent += (MaxSdkBase.SdkConfiguration sdkConfiguration) => {
            // AppLovin SDK is initialized, start loading ads
        };
        MaxSdk.SetSdkKey("785rXkTy0HOAg_rPZmSPinn-8Df-soogR_ydkn0iFmlhb3tafHqERvKeKTdozmxlSDRpO-Oue23U26lzN379e6");
        //MaxSdk.SetUserId("USER_ID");
        MaxSdk.InitializeSdk();
    }
}
