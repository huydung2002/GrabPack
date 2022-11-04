using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum CharacterStatus
{
    Idle,
    Win,
    Lose,
}

public class CharacterSprite : MonoBehaviour
{
    public float winDelay;
    public float loseDelay;

    [HideInInspector]
    public CharacterStatus status;

    // Start is called before the first frame update
    void Start()
    {
        GameManager.instance.OnLevelLoaded += OnLevelLoaded;
        GameManager.instance.OnLevelEnded += OnLevelEnded;
        UpdateStatus();
    }
    private void OnDestroy()
    {
        GameManager.instance.OnLevelLoaded -= OnLevelLoaded;
        GameManager.instance.OnLevelEnded -= OnLevelEnded;
    }
    private void OnLevelLoaded(int level)
    {
        SetStatus(CharacterStatus.Idle);
    }
    private void OnLevelEnded(bool win)
    {
        if(this.enabled)
            StartCoroutine(DelayUpdateStatus(win ? CharacterStatus.Win : CharacterStatus.Lose, win ? winDelay : loseDelay));
    }


    IEnumerator DelayUpdateStatus(CharacterStatus status, float delay)
    {
        yield return new WaitForSeconds(delay);
        SetStatus(status);
    }

    private void SetStatus(CharacterStatus stt)
    {
        this.status = stt;
        UpdateStatus();
    }


    void UpdateStatus()
    {
        for(int i = 0; i < this.transform.childCount; i ++)
        {
            this.transform.GetChild(i).gameObject.SetActive(i == (int)this.status);
            this.transform.GetChild(i).rotation = Quaternion.identity;
        }
    }
    // Update is called once per frame
    void Update()
    {
        
    }
}
