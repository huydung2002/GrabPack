using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public enum LoseStatus
{
    Lose,
    Lose1,
    Lose2,
}

public class GameManager : MonoBehaviour
{
    public static GameManager instance;
    public LayerMask hazardLayer;
    public LoseStatus loseStatus;
    public int level;
    public int maxLevel = 1;
    public bool ended = false;
    public bool win = false;

    public Character character;
    public GameObject levelObj;
    public List<Collider2D> goals = new List<Collider2D>();

    public event Action<int> OnLevelLoaded;
    public event Action<bool> OnLevelEnded;
    public bool switchMainPoseOnEnd;

    private void Awake()
    {
        instance = this; 
    }

    // Start is called before the first frame update
    void Start()
    {
       // if (this.level == 0) this.level = 1;
      int oldlevl=  PlayerPrefs.GetInt("level", level);
        LoadLevel(oldlevl);
    }
    // Update is called once per frame
    void LateUpdate()
    {
        UpdateLoseStatus();
        // if(Input.GetKeyDown(KeyCode.Escape))
        // {
        //     LoadLevel(1);
        // }
    }

    public void CollectGoal(Collider2D collider)
    {
        goals.Remove(collider);
        FindObjectOfType<AudioManager>().Play("Cling");
        if(goals.Count == 0)
        {
            EndGame(true);
        }
    }

    public void EndGame(bool win)
    {
        if (this.ended) return;
        this.ended = true;
        this.win = win;
        if (switchMainPoseOnEnd)
        {
            StartCoroutine(HideCharacterHand(win));
        }
        
        OnLevelEnded?.Invoke(win);
    }

    IEnumerator HideCharacterHand(bool win)
    {
        var characterSprite = character.GetComponentInChildren<CharacterSprite>();

        float delay = win ? characterSprite.winDelay : characterSprite.loseDelay;
        yield return new WaitForSeconds(delay);
        foreach (var hand in this.character.hands)
        {
            hand.gameObject.SetActive(false);
        }
        if(win)
        {
            FindObjectOfType<AudioManager>().Play("Win");
        }
        else{
            FindObjectOfType<AudioManager>().Play("Lost");
        }
    }

    private void LoadLevel(int level)
    {
        this.level = level;
        //var obj = this.transform.Find($"Level{level}")?.gameObject;
        //if(!obj)
       // {
            var pref = Resources.Load($"Levels/Level{level}");
            var obj = Instantiate(pref, this.transform) as GameObject;
            obj.name = pref.name;
        //}
        if(this.levelObj)
        {
            //if (this.levelObj != obj)
            {
                Destroy(this.levelObj);
            }
        }
        this.levelObj = obj;
        this.levelObj.SetActive(true);
        var pathFiniding = this.levelObj.GetComponentInChildren<PolygonPathFinding>();
        foreach (var hand in this.character.hands)
        {
            hand.pathFinding = pathFiniding;
        }
        UpdateLoseStatus();

        StartLevel();
        PlayerPrefs.SetInt("level", level);
    }
    void UpdateLoseStatus(){
        var characterSprite = character.GetComponentInChildren<CharacterSprite>();
        if (ConfigsManager.levels.TryGetValue(levelObj.name, out var config))
        {
            characterSprite.winDelay = config.delayMainWin;
            characterSprite.loseDelay = config.delayMainLose;
        }
        characterSprite.transform.GetChild(1).GetComponent<SpriteRenderer>().sprite = Resources.Load<Sprite>($"Level Ending/main_win_{level}");
        
        switch(loseStatus){
            case LoseStatus.Lose:
                characterSprite.transform.GetChild(2).GetComponent<SpriteRenderer>().sprite = Resources.Load<Sprite>($"Level Ending/main_lose");
                break;
            case LoseStatus.Lose1:
                characterSprite.transform.GetChild(2).GetComponent<SpriteRenderer>().sprite = Resources.Load<Sprite>($"Level Ending/main_lose_Saw");
                break;
            case LoseStatus.Lose2:
                characterSprite.transform.GetChild(2).GetComponent<SpriteRenderer>().sprite = Resources.Load<Sprite>($"Level Ending/main_lose_Electric");
                break;
        }
        
        switchMainPoseOnEnd = characterSprite.transform.GetChild(2).GetComponent<SpriteRenderer>().sprite != null;
        characterSprite.enabled = switchMainPoseOnEnd;
    }
    public void StartLevel()
    {
        this.win = false;
        this.ended = false;
        foreach (var hand in this.character.hands)
        {
            hand.gameObject.SetActive(true);
            hand.Restart();
        }
        character.transform.localRotation = Quaternion.identity;

        var spawnPoint = levelObj.transform.Find("SpawnPoint");
        if (spawnPoint) character.transform.position = spawnPoint.position;
        goals.Clear();
        var grabableObjs = levelObj.GetComponentsInChildren<Collider2D>();
        foreach (var collider in grabableObjs)
        {
            if (collider.CompareTag("Goal"))
            {
                goals.Add(collider);
            }
        }
        var objs = levelObj.GetComponentsInChildren<GameObjectBase>();
        foreach(var obj in objs)
        {
            obj.Reset();
        }
        OnLevelLoaded?.Invoke(this.level);
    }

    public void OnMaxLevel()
    {
        LoadLevel(1);
        StartLevel();
    }

    public void LoadNextLevel()
    {
        if(this.level >= maxLevel)
        {
            OnMaxLevel();
            return;
        }
        LoadLevel(this.level + 1);
        StartLevel();
    }
    public void ReloadLevel()
    {
        LoadLevel(this.level);
        StartLevel();
    }
}
