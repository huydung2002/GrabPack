using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TargetObjectTrigger : MonoBehaviour
{
    public GameObjectBase target;
    public float delay;

    private Coroutine targetCorotine;
    private GameObjectBase obj;
    private void Start()
    {
        obj = GetComponent<GameObjectBase>();
        obj.OnStatusChanged += OnObjectStatusChanged;
        obj.OnReset += OnReset;

    }
    private void OnReset()
    {
        if(targetCorotine != null)
        {
            StopCoroutine(targetCorotine);
            targetCorotine = null;
        }
    }
    private void OnObjectStatusChanged(ObjectStatus status)
    {
        //if(targetCorotine != null)
        //{
        //    StopCoroutine(targetCorotine);
        //    targetCorotine = null;
        //}
        //if(status == ObjectStatus.ON)
            targetCorotine = StartCoroutine(SwitchTargetStatusDelay(this.delay));
    }

    IEnumerator SwitchTargetStatusDelay(float delay)
    {
        FindObjectOfType<AudioManager>().Play("Button");
        yield return new WaitForSeconds(delay);
        this.target.SwitchStatus();
        targetCorotine = null;
    }
}
