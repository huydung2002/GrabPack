using UnityEngine;
using System.Collections;



public class ScreenBase : GameDataObject {

    public bool isShowAccountResources = false;
	public GameObject testGroup;
	public UIPanel panel;

	public virtual void OnActive()
	{
		if (!isFirstTimeUpdateData) {
			isFirstTimeUpdateData = true;
			firstTimeInit();
		}
        SyncGameData();
	}


	public virtual void OnDeactive()
	{
        //Resources.UnloadUnusedAssets();
	}


	protected virtual void firstTimeInit()
	{
		// override
#if !TEST_BUILD
		if(testGroup !=null) this.testGroup.SetActive(false);
#endif

	}



    public virtual void OnBackBtnClick()
    {
		GUIManager.instance.RemoveScreen(name);
    }

    void OnDisable()
    {
        
    }
    void OnDestroy()
    {
    }
}
