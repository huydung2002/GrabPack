using UnityEngine;
using System.Collections;

public class GameDataObject : MonoBehaviour {
    protected bool isFirstTimeUpdateData = false;
    protected bool inited = false;
	// Use this for initialization
	public virtual void Start () {
        inited = true;
        OnEnable();
        isFirstTimeUpdateData = false;
    }

    void OnEnable()
    {
        if(inited) SyncGameData();
    }
	
	// Update is called once per frame
    public virtual void Update()
    {
		
	}


    public virtual void SyncGameData()
    {

    }
}
