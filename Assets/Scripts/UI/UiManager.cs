using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UiManager : MonoBehaviour
{
    public static UiManager instance;
    // public HandDrag handDrag;
    public GameObject LayerBase;
    public GameObject SoundOn;
    public GameObject SoundOff;
    public GameObject GrabHand1,GrabHand2;

    private bool isSound;

    void Awake()
    {
        if(instance == null){
            instance = this;
        }
        else
        {
            Destroy(gameObject);
        }
        DontDestroyOnLoad(gameObject);
        
        LayerBase.SetActive(false);
        SoundOn.SetActive(true);
        SoundOff.SetActive(false);
        isSound = true;
    }

    public void SettingBtnClick()
    {
        FindObjectOfType<AudioManager>().Play("Button");
        if(GameManager.instance.win == false && GameManager.instance.ended == false)
        {
            LayerBase.SetActive(true);
            CantDraging();
        }
    }
    public void SkipBtnClick()
    {
        FindObjectOfType<AudioManager>().Play("Button");
        if(GameManager.instance.win == false && GameManager.instance.ended == false)
        {
        GameManager.instance.LoadNextLevel();
        LayerBase.SetActive(false);
        CanDraging();
        }
    }
    public void xButton()
    {
        FindObjectOfType<AudioManager>().Play("Button");
        CanDraging();
        LayerBase.SetActive(false);
    }

    void CanDraging(){
        GrabHand1.GetComponentInParent<GrabHand>().enabled = true;
        GrabHand2.GetComponentInParent<GrabHand>().enabled = true;
        
        GrabHand1.GetComponentInChildren<HandDrag>().enabled = true;
        GrabHand1.GetComponentInChildren<CircleCollider2D>().enabled = true;
        GrabHand2.GetComponentInChildren<HandDrag>().enabled = true;
        GrabHand2.GetComponentInChildren<CircleCollider2D>().enabled = true;
    }
    void CantDraging(){
        GrabHand1.GetComponentInParent<GrabHand>().enabled = false;
        GrabHand2.GetComponentInParent<GrabHand>().enabled = false;
    }
    public void ButtonSound(){
        FindObjectOfType<AudioManager>().Play("Button");
        if(isSound){
            ButtonSoundStatus(isSound);
            isSound = false;
        }
        else if(isSound == false)
        {
            ButtonSoundStatus(isSound);
            isSound = true;
        }
    }
    void ButtonSoundStatus(bool on)
    {
        on = isSound;
        if(!on){
            SoundOff.SetActive(false);
            SoundOn.SetActive(true);
            AudioListener.volume = 1;
        }
        else
        {
            SoundOn.SetActive(false);
            SoundOff.SetActive(true);
            AudioListener.volume = 0;
        }
    }

    public void MoreGameBtn(){
        FindObjectOfType<AudioManager>().Play("Button");
    }
}
