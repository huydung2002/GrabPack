using UnityEngine;
using System.Collections;

public class PopupBase : GameDataObject {

    public Transform childsContainer;
	public bool fadeOnShow = true;
    public bool bringToTop;
	public System.Action OnShowFinished;
    public virtual void OnCreate(params object[] args)
    {

    }

    public virtual void OnDismiss()
    {

    }
    public void Dismiss()
    {
	    GUIManager.instance.OnPopupClosed(this);
        OnDismiss();
        if (!this.gameObject) return;
        float delayTime = 0.2f;
		if (!fadeOnShow) delayTime = 0;
		var tween = TweenAlpha.Begin(this.gameObject, delayTime, 0);
		tween.onFinished.Clear();
        Destroy(this.gameObject, delayTime);
    }
}
