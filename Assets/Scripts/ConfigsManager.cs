using LitJson;
using System.Collections;
using System.Collections.Generic;
//using UnityEditor.PackageManager;
using UnityEngine;


[DefaultExecutionOrder(-200)]
public class ConfigsManager : MonoBehaviour
{
	public static Dictionary<string, LevelConfig> levels;
	void Awake()
	{
		DontDestroyOnLoad(this.gameObject);


		levels = LoadConfig<Dictionary<string, LevelConfig>>("level");
		
	}
	// Use this for initialization
	void Start()
	{
	}


	public static T LoadConfig<T>(string fileName)
	{
		string path = "Configs/" + fileName;
		//Debug.Log(path);
		TextAsset asset = Resources.Load<TextAsset>(path);
		if (asset == null) return default(T);
		string data = asset.ToString();
		return (JsonMapper.ToObject<T>(data));
    }

    public class LevelConfig {
		public float delayPopupWin = 1f;
		public float delayPopupLose = 1f;
		public float delayMainWin = 1f;
		public float delayMainLose = 1f;
	}
	

}