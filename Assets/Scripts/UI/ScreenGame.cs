using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ScreenGame : ScreenBase
{
	public static ScreenGame instance;

	public UILabel levelLbl;
	private void Awake()
	{
		instance = this;
	}
	public override void OnActive()
	{
		base.OnActive();
		GameManager.instance.OnLevelLoaded += OnLevelLoaded;
		GameManager.instance.OnLevelEnded += OnLevelEnded;
	}

    private void OnLevelLoaded(int level)
    {
		this.levelLbl.text = $"LEVEL {level}";
    }
	private void OnLevelEnded(bool win)
    {

		StartCoroutine(ShowEndGamePopupDelay(win));
    }

	IEnumerator ShowEndGamePopupDelay(bool win)
    {
		float delay = 0;
		if(ConfigsManager.levels.TryGetValue(GameManager.instance.levelObj.name, out var config)) {
			 delay = win ? config.delayPopupWin : config.delayPopupLose;
        }
		yield return new WaitForSeconds(delay);
		GUIManager.instance.CreatePopup<PopupEndGame>(win);
	}

	public override void Update()
	{
		base.Update();
	}


	public override void OnDeactive()
	{
		base.OnDeactive();
	}


	public override void SyncGameData()
	{
		base.SyncGameData();
	}
	public override void OnBackBtnClick()
	{
		GUIManager.instance.SetScreen("Menu");
	}
	// public void SkipBtnClick()
	// {
	// 	GameManager.instance.LoadNextLevel();
	// }
	// public void SettingBtnClick(){

	// }
}
