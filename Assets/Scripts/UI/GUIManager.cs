using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;
//using System;

public class GUIManager : MonoBehaviour
{
    public static readonly Color RES_ENOUGH_COLOR = new Color(0, 0, 0);
    public static readonly Color RES_NOT_ENOUGH_COLOR = new Color(255, 0, 0);

    public static GUIManager instance;
	public Dictionary<string, ScreenBase> screens = new Dictionary<string,ScreenBase>();
    private string crScreenName;
    public UIRoot uiRoot;
    public Camera uiCamera;

    public GameObject popUpContainer;
    public Transform ScreenContainer;
    public Transform GameManagerTran;

    public delegate void OnSceenActive(ScreenBase screen);
	public int activedPopupCount = 0;
    private int depthLevel;
	[SerializeField]
	private string defaultScreen;
    public event Action<PopupBase> OnPopupCreated;
    public ScreenBase activeScreen;

    public AsyncOperation PreloadScreen(string screenName)
    {
        return Resources.LoadAsync("Screens/Screen" + screenName);
    }
    ScreenBase LoadScreen(string screenName)
    {
        UnityEngine.Object o = Resources.Load("Screens/Screen" + screenName);

        GameObject go = (GameObject)Instantiate(o);

        if (go == null)
        {
            return null;
        }

        //		go.SetActive(false);

        Vector3 pos = go.transform.localPosition;
        go.transform.parent = ScreenContainer;
        go.transform.localPosition = pos;
        go.transform.localScale = Vector3.one;

        ScreenBase obj = go.GetComponent<ScreenBase>();

        if (obj == null)
        {
            return null;
        }
		obj.name = screenName;

		if (screens.ContainsKey(screenName))
        {
            screens[screenName] = obj;
        }
        else
        {
            screens.Add(screenName, obj);
        }

        go.SetActive(false);
        return obj;
    }

	public void RemoveScreen(string screenName)
	{
		ScreenBase screen;
		this.screens.TryGetValue(screenName, out screen);
		if (screen != null)
		{
			this.screens.Remove(screenName);
			Destroy(screen.gameObject);
		}

	}

    void Awake()
    {



        if (instance != null)
        {
            GameObject.Destroy(gameObject);
            return;
        }
        else
        {
            instance = this;
        }

        ScreenBase[] screens = ScreenContainer.GetComponentsInChildren<ScreenBase>(true);
        foreach (ScreenBase sc in screens)
        {
            if (sc != null)
                Destroy(sc.gameObject);
        }
		ClearPopups(false);
		//Init();


		InitSounds();
		//Localization.language = "Vietnamese";
	}
	private void InitSounds()
	{
		//UIButton.clickClip = Resources.Load("Sounds/button") as AudioClip;
	}
    public void ClearPopups(bool dissmis = true)
    {
        for (int i = 0; i < this.popUpContainer.transform.childCount; i++)
        {
            var child = this.popUpContainer.transform.GetChild(i);
            if (dissmis)
                child.GetComponent<PopupBase>().Dismiss();
            else Destroy(child.gameObject);

            //Destroy(this.popUpContainer.transform.GetChild(i).gameObject);
        }
        this.activedPopupCount = 0;
        depthLevel = 0;
    }
    // Use this for initialization
    void Start()
    {
       // PopupLoading.Create();
	   if(!string.IsNullOrEmpty(defaultScreen))
			SetScreen(defaultScreen);
		//Application.targetFrameRate = 40;
    }

    // Update is called once per frame
    void Update()
    {
		if (Input.GetKeyDown(KeyCode.Escape))
		{
			ScreenBase screen = GetScreen(this.crScreenName);
			if (activedPopupCount == 0)
			{
				screen.OnBackBtnClick();
			}
		}
        if (Input.GetKeyDown(KeyCode.Space))
        {
            //GUIManager.instance.CreatePopup<Popup
            //  RenderSettings.fog = !RenderSettings.fog;
        }
    }

    public static void log(string str)
    {
        //if (instance != null && DebugManager.isDebug)
        //{
        //    EGDebug.Log(str);
        //    //if (instance.txtDebug != null) {
        //    //    instance.txtDebug.text += str + "\n";
        //    //}
        //}
    }

    public ScreenBase GetScreen(string screen, bool isForceLoad = true)
    {
        if (screens.ContainsKey(screen) == false)
        {
            if (isForceLoad)
                LoadScreen(screen);
            else
                return null;
        }

        return screens[screen];
    }
	
	public ScreenBase SetScreen(string screenName, OnSceenActive onScreenActive = null)
    {
		//GUIManager.instance.ClearPopups();
		if (!string.IsNullOrEmpty(screenName))
		{
			if (screens.ContainsKey(screenName) == false)
			{
				ScreenBase screen = LoadScreen(screenName);
			}

			if (crScreenName == screenName)
			{
				return this.screens[crScreenName];
			}
		}
        ScreenBase curScreen= null;
		if (!string.IsNullOrEmpty(crScreenName))
		{
			if (screens.ContainsKey(crScreenName))
			{
				curScreen = screens[crScreenName];
				curScreen.OnDeactive();
				curScreen.gameObject.SetActive(false);
			}
		}
		if (!string.IsNullOrEmpty(screenName))
		{
			curScreen = screens[screenName];
			curScreen.gameObject.SetActive(true);
			if (onScreenActive != null) onScreenActive(curScreen);
			curScreen.OnActive();
            
			UIPanel panel = (screens[screenName].transform.parent).GetComponent<UIPanel>();
			if (panel)
				panel.Refresh();
            curScreen.panel = panel;
		}
		
        crScreenName = screenName;
		if (GameManagerTran != null)
		{

			for (int i = 0; i < GameManagerTran.childCount; i++)
			{
				Transform gameManager = GameManagerTran.GetChild(i);
				gameManager.gameObject.SetActive(gameManager.name == crScreenName);
			}
		}

        
        //accountResourcesGroup.SetActive(curScreen.isShowAccountResources);
#if !UNITY_IPHONE
       // EGDebug.Log("Garbage collect");
        System.GC.Collect();
#endif
        activeScreen = curScreen;
        if (!activeScreen.panel)
        {
            activeScreen.panel = activeScreen.GetComponent<UIPanel>();
        }
        return curScreen;
    }



    public T CreatePopup<T>(params object[] args) where T : PopupBase
	{
        GameObject pref = GetPopupPref(typeof(T).ToString());
        if(pref != null)
        {
            GameObject obj = Instantiate(pref, this.popUpContainer.transform) as GameObject;
			obj.name = pref.name;
            obj.transform.localPosition = Vector3.back * (activedPopupCount + 1) * 300;
            obj.transform.localScale = Vector3.one;
            obj.SetActive(true);
			Transform blackSprite = obj.transform.Find("BlackSprite");
			if (blackSprite != null)
			{
				BoxCollider box = blackSprite.gameObject.AddComponent<BoxCollider>();
                blackSprite.GetComponent<UISprite>().SetDimensions(3000, 4000);

                box.size = new Vector3(3000, 3000);
			}
            T popup = obj.GetComponent<T>();
            var rootDepth = popup.bringToTop ? 10000 : 100;        
            UIPanel[] panels = obj.GetComponentsInChildren<UIPanel>(true);
            foreach(UIPanel panel in panels) {
                panel.depth += rootDepth * (depthLevel + 1);
                panel.sortingOrder = panel.depth;
            }
			if (popup.fadeOnShow)
			{
				panels[0].alpha = 0.2f;
				var tw = TweenAlpha.Begin(popup.gameObject, 0.1f, 1f);
				tw.SetOnFinished(() =>
				{
					if (popup.OnShowFinished != null) popup.OnShowFinished();
				});
			}
			else if (popup.OnShowFinished != null)popup.OnShowFinished();
			activedPopupCount++;
            depthLevel++;
			popup.OnCreate(args);
            OnPopupCreated?.Invoke(popup);

            return popup;
        }
        return null;
    }

    public PopupBase CreatePopup(string popupName, params object[] args)
    {
        GameObject pref = GetPopupPref(popupName);
        if (pref != null)
        {
            GameObject obj = Instantiate(pref, this.popUpContainer.transform) as GameObject;
            obj.name = pref.name;
            obj.transform.localPosition = Vector3.back * (popUpContainer.transform.childCount + 2) * 10;
            obj.transform.localScale = Vector3.one;
            obj.SetActive(true);
            Transform blackSprite = obj.transform.Find("BlackSprite");
            if (blackSprite != null)
            {
                BoxCollider box = blackSprite.gameObject.AddComponent<BoxCollider>();
                blackSprite.GetComponent<UISprite>().SetDimensions(3000, 4000);

                box.size = new Vector3(3000, 3000);
            }
            UIPanel[] panels = obj.GetComponentsInChildren<UIPanel>(true);
            foreach (UIPanel panel in panels)
            {
                panel.depth += 100 * (popUpContainer.transform.childCount);
                panel.sortingOrder = panel.depth;
            }
            PopupBase popup = obj.GetComponent<PopupBase>();
            if (popup.fadeOnShow)
            {
                panels[0].alpha = 0.2f;
                var tw = TweenAlpha.Begin(popup.gameObject, 0.2f, 1f);
                tw.SetOnFinished(() =>
                {
                    if (popup.OnShowFinished != null) popup.OnShowFinished();
                });
            }
            else if (popup.OnShowFinished != null) popup.OnShowFinished();
            activedPopupCount++;
            popup.OnCreate(args);
            return popup;
        }
        return null;
    }
    public void OnPopupClosed(PopupBase p)
	{
		activedPopupCount--;
        if (activedPopupCount == 0) depthLevel = 0;
    }
    public GameObject GetPopupPref(string popupName)
    {
        return Resources.Load("Popups/" + popupName) as GameObject;
    }
    public T CreateGridViewItem<T>(int index, UIGrid grid) where T : Component
    {
        Transform child;
        if (index < grid.transform.childCount)
        {
            child = grid.transform.GetChild(index);
        }
        else
        {
            child = Instantiate(grid.transform.GetChild(0).gameObject, grid.transform).transform;
        }
        child.gameObject.SetActive(true);
        var item = child.GetComponent<T>();
        return item;
    }

    public T CreateTableViewItem<T>(int index, UITable table) where T : Component
    {
        Transform child;
        if (index < table.transform.childCount)
        {
            child = table.transform.GetChild(index);
        }
        else
        {
            child = Instantiate(table.transform.GetChild(0).gameObject, table.transform).transform;
        }
        child.gameObject.SetActive(true);
        var item = child.GetComponent<T>();
        return item;
    }

    public void ClearTableHiddenChild(UITable table, int length)
    {
	    for (int i = length; i < table.transform.childCount; i++)
	    {
		    table.transform.GetChild(i).gameObject.gameObject.SetActive(false);
	    }
	    table.repositionNow = true;
    }

    public void HideChilds(Transform trams)
    {
        for (int i = 0; i < trams.childCount; i++)
        {
            trams.GetChild(i).gameObject.SetActive(false);
        }
    }
    public void ClearGridHidenChild(UIGrid grid, int length)
    {
        for (int i = length; i < grid.transform.childCount; i++)
        {
            grid.transform.GetChild(i).gameObject.gameObject.SetActive(false);
        }
        grid.Reposition();
    }
    public T GetObjectFromPool<T>(Transform container) where T : Component
    {
        GameObject obj = null;
        for (int i = container.childCount - 1; i >= 0; i--)
        {
            var child = container.GetChild(i).gameObject;
            if (!child.activeSelf)
            {
                obj = child;
                break;
            }
        }
        if (!obj) obj = Instantiate(container.GetChild(0).gameObject, container);
        obj.SetActive(true);
        return obj.GetComponentInChildren<T>();
    }
    public Vector3 GetNGUIPosByWorldPos(Vector3 worldPos)
    {
        Vector3 pos = Camera.main.WorldToViewportPoint(worldPos);
        pos.z = 0;

        var nguiPos = GUIManager.instance.uiCamera.ViewportToWorldPoint(pos);
        return nguiPos;
    }
    public bool CheckUIRaycast(out Collider collider)
    {
        var screenPos = Input.mousePosition;
        var ray = this.uiCamera.ScreenPointToRay(screenPos);
        RaycastHit hitinfo;
        var layerMask = 1 << this.gameObject.layer;
        var hitted = Physics.Raycast(ray, out hitinfo, 200, layerMask);
        if (hitted)
        {
            collider = hitinfo.collider;
            return true;

        }
        collider = null;
        return false;
    }
    public void UpdateTeamColor(UILabel label, ushort team)
    {
        label.color = team == 0 ? Color.white : Color.red;
    }

    public T GetPopup<T>() where T: Component
    {
        for (int i = 0; i < this.popUpContainer.transform.childCount; i++)
        {
            var child = this.popUpContainer.transform.GetChild(i);
            if (child.gameObject.activeSelf)
            {
                var component = child.GetComponent<T>();
                if (component != null)
                    return component;
            }
        }
        return null;
    }



}

public enum GameScreen
{
    Game,
    Login,
    CreateClan,
    JoinClan,
    MyClan,
    NoClan
}
