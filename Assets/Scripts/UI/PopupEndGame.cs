using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PopupEndGame : PopupBase
{
	public GameObject loseGrp;
	public GameObject winGrp;
	public UITexture mTexture;
	public override void OnCreate(params object[] args)
	{
		bool win = (bool)args[0];
		if(loseGrp)loseGrp.SetActive(!win);
		if(winGrp) winGrp.SetActive(win);
		var texture = Resources.Load($"Level Ending/UI_level{GameManager.instance.level}_{(win ? "win" : "lose")}") as Texture;
		mTexture.mainTexture = texture;
	}
    public void ClickSkipadsfail()
    {
        System.Action ClickReviewADSfail = () =>
        {
            //  this.OnCloseBtnClick();
            OnNextBtnClick();
        };
	#if UNITY_EDITOR
        ClickReviewADSfail();
	#else
            if (AdManager.Ins.CheckHasRewardAds(true))
        {
            AdManager.Ins.ShowVideoAds("VideoAds_in_ClickToReview", ClickReviewADSfail);
        }
	#endif
    }
    public void OnNextBtnClick()
	{
		FindObjectOfType<AudioManager>().Play("Button");
		GameManager.instance.LoadNextLevel();
		Dismiss();
    }
    

    public void OnReplayBtnClick()
	{
		FindObjectOfType<AudioManager>().Play("Button");
		GameManager.instance.ReloadLevel();
		Dismiss();
	}
}
