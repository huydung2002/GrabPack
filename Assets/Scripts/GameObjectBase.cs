using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum ObjectStatus { 
    ON,
    OFF

}

public interface ILevelObject
{

    public void Restart();

}

public class GameObjectBase : MonoBehaviour
{
    public ObjectStatus status;
    public bool oneTime;
    public GameObject[] onObjects;
    public GameObject[] offObjects;

    public UITweener tweener;

    private bool trigged = false;

    public event System.Action<ObjectStatus> OnStatusChanged;
    public event System.Action OnReset;
    private ObjectStatus defaultStattus;
    private void Awake()
    {
        defaultStattus = status;

        if (tweener && tweener is TweenPosition)
        {
            ((TweenPosition)tweener).from = tweener.transform.localPosition;
        }

    }
    private void Start()
    {
        UpdateStatus(true);
    }
    public void Reset()
    {
        this.status = defaultStattus;
        UpdateStatus(true);
        trigged = false;
        OnReset?.Invoke();
        
    }
    protected virtual void UpdateStatus(bool reset)
    {
        foreach(var onObj in onObjects)
        {
            onObj.SetActive(status == ObjectStatus.ON);
        }
        foreach (var offObj in offObjects)
        {
            offObj.SetActive(status == ObjectStatus.OFF);
        }
        if(tweener) {
            if (tweener.style == UITweener.Style.PingPong)
            {
                tweener.enabled = status == ObjectStatus.ON;
                if(reset)
                {
                    tweener.Sample(0, false);
                }
            }
            else
            {
                if (reset)
                {
                    tweener.enabled = false;
                    tweener.tweenFactor = 0f;
                    tweener.Sample(0, false);
                }
                else 
                {
                    if (status == ObjectStatus.ON)
                        tweener.PlayForward();
                    else
                        tweener.PlayReverse();
                }
            }
        }
    }

    public void SetStatus(ObjectStatus status)
    {
        if(oneTime)
        {
            if (trigged) return;
            trigged = true;
        }
        this.status = status;
        UpdateStatus(false);
        OnStatusChanged?.Invoke(status);
    }

    public void SwitchStatus()
    {
        SetStatus(this.status == ObjectStatus.ON ? ObjectStatus.OFF : ObjectStatus.ON);
    }

    private void OnDrawGizmos()
    {
        if(tweener && tweener is TweenPosition)
        {
            var twPos = (TweenPosition)tweener;
            Vector3 fromPos = twPos.transform.position;
            Vector3 toPos = twPos.transform.parent.position + twPos.to;
            Gizmos.color = Color.blue;
            Gizmos.DrawWireCube(fromPos, Vector3.one);
            Gizmos.color = Color.cyan;
            Gizmos.DrawWireCube(toPos, Vector3.one);
            Gizmos.DrawLine(fromPos, toPos);
        }
    }
}
