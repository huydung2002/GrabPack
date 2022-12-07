
using UnityEngine;
using UnityEngine.SceneManagement;
using TMPro;

public class PreloadScene : MonoBehaviour
{
    public float timeLoading = 7f;
    private float minPreloadValue = 0.2f;

    private float currentTime = 0f;
    private bool callLoadScene = false;

    public bool allowGoToSceneAfterLoad = false;

    [SerializeField] private string _sceneName = "Main";
    [SerializeField] private TextMeshProUGUI textLoadValue;
    public string _SceneName => this._sceneName;

[HideInInspector]    public bool finishAOA = false;

    public GameObject mycv;
    public GameObject mycam;

    [SerializeField] private UnityEngine.UI.Slider sliderBar;

    private AsyncOperation _asyncOperation;
    private void LoadSceneAsyncProcess(string sceneName)
    {
        // Begin to load the Scene you have specified.
        this._asyncOperation = SceneManager.LoadSceneAsync(sceneName,LoadSceneMode.Additive);
        // this._asyncOperation.allowSceneActivation = allowGoToSceneAfterLoad;
        this._asyncOperation.allowSceneActivation = false;

    }
    private void Start()
    {
        currentTime = 0f;
        LoadSceneAsyncProcess(_sceneName);
    }
    private void Update()
    {
        currentTime += Time.deltaTime;
        if (currentTime<timeLoading)
        {
            if (currentTime / timeLoading < minPreloadValue)
            {
                sliderBar.value = minPreloadValue;
            }
            else
            {
                sliderBar.value = currentTime / timeLoading;
            }
        }
        else
        {
            currentTime = timeLoading;
            sliderBar.value = 1;
            //if (!callLoadScene&& this._asyncOperation != null)
            if (!callLoadScene)
                {
                    callLoadScene = true;
                GoToNextScene();
                //this._asyncOperation.allowSceneActivation = true;
                //LoadSceneAsyncProcess(sceneName: this._sceneName);
                
            }
        }
        textLoadValue.text = "Loading..."+ Mathf.Floor(sliderBar.value*100)+"%";
    }
    private void GoToNextScene()
    {
        Debug.Log("Try gotoNext Scene:"+ finishAOA);
        
        if (finishAOA)
        {
            // SceneManager.LoadScene(_sceneName, LoadSceneMode.Additive);
            this._asyncOperation.allowSceneActivation=true;
            Debug.Log("Finish go to next Scene");
            mycv.SetActive(false);
            mycam.SetActive(false);
        }
        else
        {
            Invoke("GoToNextScene", 1);
        }
    }
}
