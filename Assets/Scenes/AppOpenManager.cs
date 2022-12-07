using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AppOpenManager : Singleton<AppOpenManager>
{
    public string AppOpenAdUnitId = "9b88e0fb0bf48975";

    public void ShowAdIfReady()
    {
        //if (!AVT.Saver.saveFile.player.removeAds)
        {
            if (MaxSdk.IsAppOpenAdReady(AppOpenAdUnitId))
            {
                MaxSdk.ShowAppOpenAd(AppOpenAdUnitId);
            }
            else
            {
                MaxSdk.LoadAppOpenAd(AppOpenAdUnitId);
            }
        }

    }
}
