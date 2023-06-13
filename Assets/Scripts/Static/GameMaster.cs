using Skytanet.SimpleDatabase;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
public class GameMaster : MonoBehaviour
{
    public static PlayerData Data;

    private static SaveFile _saveFile;

    private static Button _startLevelButton;

    private void Awake()
    {
        Application.targetFrameRate = 60;
       
        _saveFile = new SaveFile("Save", Application.persistentDataPath);
  
        List<string> Keys = _saveFile.GetKeys();
        bool exists = false;
        foreach (var item in Keys)
        {
            if (item == "Save")
            {
                exists = true;
                Data = _saveFile.Get<PlayerData>(item);

            }
        }
        if (!exists)
        {
            Data = new PlayerData(1);
            _saveFile.Set("Save", Data);
            
        }
       
        DontDestroyOnLoad(this);
        DontDestroyOnLoad(FindObjectOfType<EventSystem>());
        FindObjectOfType<SceneLoader>().Initialize();
        SceneLoader.OnLoadSceneComplete.Add(SetButtonData);
        SceneLoader.LoadScene("MainMenu");
    }

    internal static void ResetData()
    {
        _saveFile.Delete("Save");
        Data = new PlayerData(1);
        _saveFile.Set("Save", Data);
    }

    private void OnApplicationQuit()
    {
        _saveFile.Close();
    }
    public static void LoadNextLevel()
    {

        SceneLoader.OnLoadSceneComplete.Add(LevelInit);
        SceneLoader.LoadScene("Level");
        
    }
    public static void LevelInit()
    {
        Level level = FindObjectOfType<Level>();
        if (Data.tutorial)
            level.LevelName = "LevelTutorial" + Data.CurrentLevel;
        else
            level.LevelName = "Level" + Data.CurrentLevel;
        level.LoadLevel();

    }

    public static void SetButtonData()
    {
        _startLevelButton = GameObject.Find("Level Button").GetComponent<Button>();
        _startLevelButton.onClick.RemoveAllListeners();
        if (Data.CurrentLevel > 20)
        {
            _startLevelButton.GetComponentInChildren<Text>().text = "Feed ";
            _startLevelButton.onClick.AddListener(FeedFrog);
            _startLevelButton.onClick.AddListener(() => { 
            if(GameMaster.Data.size >= 100)
                {
                    
                    _startLevelButton.transform.position += new Vector3(0, 300);
                    _startLevelButton.GetComponentInChildren<Text>().text = "Game over?";
                    _startLevelButton.onClick.AddListener(() => {
                        SceneLoader.LoadScene("Final");
                    });

                }
            });
            
            return;
        }

        if (Data.tutorial)
            _startLevelButton.GetComponentInChildren<Text>().text = "Tutorial";
        else if(Data.CurrentLevel <= 20)
            _startLevelButton.GetComponentInChildren<Text>().text ="Level " + Data.CurrentLevel;
      
       
     
        _startLevelButton.onClick.AddListener(LoadNextLevel);

        Frog frog = FindObjectOfType<Frog>();
        frog.LoveBar.fillAmount = Data.love;
        frog.FoodBar.fillAmount = Data.food;
    }
    public static void OnLevelCompleted()
    {

        Data.CurrentLevel += 1;
        if (Data.tutorial && Data.CurrentLevel == 4)
        {
            Data.tutorial = false;
            Data.CurrentLevel = 1;

        }
        _saveFile.Set("Save", Data);
        //SetButtonData();
    }

    public static void FeedFrog()
    {
        Data.size++;
        FindObjectOfType<Frog>().Feed(7);

        _saveFile.Set("Save", Data);
    }
    public static void FeedFrogOnLose()
    {

        FindObjectOfType<Frog>().Feed(2);
        _saveFile.Set("Save", Data);
    }

}

[System.Serializable]
public struct PlayerData
{
    public int CurrentLevel;
    public int size;
    public float love;
    public float food;
    public bool tutorial;
    public void AddFood(float Add)
    {
        food += Add;
        food = Mathf.Clamp01(food);
    }
    public void AddLove(float Add)
    {
        love += Add;
        love = Mathf.Clamp01(love);
    }
    public PlayerData(int currentLevel)
    {
        CurrentLevel = currentLevel;
        size = 1;
        love = food = 0;
        tutorial = true;

    }

}