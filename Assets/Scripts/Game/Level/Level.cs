using Skytanet.SimpleDatabase;
using System;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using UnityEngine.UI;
public class Level : MonoBehaviour
{

    private List<Condition> _conditions = new List<Condition>();
    private Stream _block, _dat;
    private SaveFile _saveFile;
    private Board _board;
    private int _moves;
    private int _points;
    private bool _levelIsOver = false;
    private bool _hasPreexistingTiles = false;
    private Dictionary<Vector2Int, string> _tileTypesInPositions;
    private List<int> _pointsForStars = new List<int> { 0, 100, 200 };
    private GameObject _startPanel;
    private bool _wathcedAd = false;

    [HideInInspector]
    public Dictionary<Vector2Int, LevelElement> Elements = new Dictionary<Vector2Int, LevelElement>();
    [HideInInspector]
    public string[] KeysForTiles;

    public GameObject ElementPrefab;
    public GameObject BreakableLevelElementPrefab;
    public GameObject EndWinPanel;
    public GameObject Firework;
    public GameObject EndLosePanel;
    public GameObject FingerPrefab;
    public Sprite FingerClosed;
    public Sprite Star;
    public Text pointsText;

    public string LevelName;

    private void Start()
    {
        _startPanel = GameObject.Find("StartPanel");
        _board = FindObjectOfType<Board>();
        if (FindObjectOfType<GameMaster>() == null)
        {
            LoadLevel();
        }
        Board.TilesDestroyed toConnect = new Board.TilesDestroyed(GetPoints);
        Board.ChainOver ChainOvertoConnect = new Board.ChainOver(ChainOver);
        _board.OnChainOver.Add(ChainOvertoConnect);
        _board.OnTileDestroyed.Add(toConnect);
    }
    public void GetPoints(HashSet<Vector2Int> tiles)
    {

        foreach (var item in tiles)
        {
            _points++;
        }
        pointsText.text = "" + _points;
    }
    public void ChainOver()
    {
        if (_moves <= 0 && !_levelIsOver)
        {
            LoseLevel();
            _levelIsOver = true;
        }
    }
    private void StartPanelInit()
    {
      
        var OkButton = _startPanel.GetComponentInChildren<Button>(); 
        var VerticalLayout = _startPanel.GetComponentInChildren<VerticalLayoutGroup>();

        var GoalsHorizontal = GameObject.Find("GoalsHorizontal");

        var textPrefab = GameObject.Find("JustTextCondition").GetComponent<Text>();
        var SpecificTilesPrefab = GameObject.Find("SpecificTilesCondition").GetComponent<Text>();

        var SpecificTilesGoal = GameObject.Find("SpecificTilesGoal");

        SpecificTilesPrefab.gameObject.SetActive(false);
        textPrefab.gameObject.SetActive(false);
        SpecificTilesGoal.gameObject.SetActive(false);
        foreach (var item in _conditions)
        {
            if (item.GetType() == typeof(GetSomeTilesCondition))
            {

                var itemtype = (GetSomeTilesCondition)item;
                var text = Instantiate<Text>(SpecificTilesPrefab, VerticalLayout.transform);
                text.text = "x" + itemtype.AmmountToGet;
                Tile tile = _board.GetComponent<TileFactory>().Create(Convert.ToString(itemtype.Type));
                text.GetComponentInChildren<Image>().sprite = tile.GetComponent<SpriteRenderer>().sprite;
                text.gameObject.SetActive(true);


                var textgoal = Instantiate<GameObject>(SpecificTilesGoal, GoalsHorizontal.transform);
                textgoal.GetComponentInChildren<Text>().text = "" + itemtype.AmmountToGet;
                itemtype.UpdateVisual = textgoal;
                textgoal.GetComponentInChildren<Image>().sprite = tile.GetComponent<SpriteRenderer>().sprite;
                textgoal.gameObject.SetActive(true);
                Destroy(tile.gameObject);
            }

            else if (item.GetType() == typeof(GetPointsCondition))
            {

                var itemtype = item.GetComponent<GetPointsCondition>();
                var text = Instantiate<Text>(textPrefab, VerticalLayout.transform);
                text.text = "Get points: " + itemtype.PointsToGet;
                text.gameObject.SetActive(true);

                var textgoal = Instantiate<GameObject>(SpecificTilesGoal, GoalsHorizontal.transform);
                textgoal.GetComponentInChildren<Text>().text = "" + (itemtype.PointsToGet - itemtype.Points);
                itemtype.UpdateVisual = textgoal;
                Destroy(textgoal.GetComponentInChildren<Image>().gameObject);
                textgoal.gameObject.SetActive(true);

            }
            else if (item.GetType() == typeof(BreakLevelElementCondition))
            {

                var itemtype = item.GetComponent<BreakLevelElementCondition>();
                var text = Instantiate<Text>(textPrefab, VerticalLayout.transform);
                text.text = "Destroy Ice";
                text.gameObject.SetActive(true);

                var textgoal = Instantiate<GameObject>(SpecificTilesGoal, GoalsHorizontal.transform);
                textgoal.GetComponentInChildren<Text>().text = "" + itemtype.Elements.Count;
                itemtype.UpdateVisual = textgoal;
                textgoal.GetComponentInChildren<Image>().sprite = SpriteFactory.GetSprite("IceFull");
                textgoal.GetComponentInChildren<Image>().color = Color.white;
                textgoal.gameObject.SetActive(true);


            }
            else if (item.GetType() == typeof(GetTileDownCondition))
            {

                var itemtype = item.GetComponent<BreakLevelElementCondition>();
                var text = Instantiate<Text>(textPrefab, VerticalLayout.transform);
                text.text = "Get cookies down";
                text.gameObject.SetActive(true);

                var textgoal = Instantiate<GameObject>(SpecificTilesGoal, GoalsHorizontal.transform);

                textgoal.GetComponentInChildren<Image>().sprite = SpriteFactory.GetSprite("Cookie");
                textgoal.GetComponentInChildren<Image>().color = Color.white;
                textgoal.gameObject.SetActive(true);

            }
            else if (item.GetType() == typeof(CompleteTutorialCondition))
            {

                var itemtype = item.GetComponent<BreakLevelElementCondition>();
                var text = Instantiate<Text>(textPrefab, VerticalLayout.transform);
                text.text = "Complete tutorial";
                text.gameObject.SetActive(true);

                var textgoal = Instantiate<GameObject>(SpecificTilesGoal, GoalsHorizontal.transform);
                Destroy(textgoal.GetComponentInChildren<Image>().gameObject);
                textgoal.GetComponentInChildren<Text>().text = "Tutorial";
                textgoal.gameObject.SetActive(true);

            }



        }
        OkButton.onClick.AddListener(() =>
        { if (_hasPreexistingTiles)
            {
                _board.InitializeBoardWithPreexistingTiles(_tileTypesInPositions);
            }
            else
            {
                _board.InitializeBoard();
            }
            _startPanel.gameObject.SetActive(false);
        });

        _moves++;
        MinusMoves();

    }
    private void OnApplicationQuit()
    {
        _dat.Close();
        _block.Close();
        //_saveFile.Close();
    }
    public void LoadLevel()
    {
        //load layout of elements
        //load tilePrefabs
        //load condtions to win
        if (GameMaster.Data.tutorial)
        {
            _board.gameObject.AddComponent<TutorialLevel>();
        }
        _dat = BetterStreamingAssets.OpenRead("levels.dat");
        _block = BetterStreamingAssets.OpenRead("levels.block");
        _saveFile = new SaveFile(_dat, _block);
        SaveLevel save = _saveFile.Get<SaveLevel>(LevelName);
        _tileTypesInPositions = new Dictionary<Vector2Int, string>();
        Elements.Clear();
        foreach (var item in save.Elements)
        {

            var temp = Instantiate(ElementPrefab, transform);
            temp.gameObject.AddComponent(Type.GetType((string)item.ElementType));
            temp.transform.position = new Vector2(item.X, item.Y);
            temp.gameObject.GetComponent<SpriteRenderer>().color = new Color(0, 0, 0, 0.1f);
            Elements.Add(new Vector2Int(item.X, item.Y), temp.GetComponent<LevelElement>());

            if (item.TileInPositions != null)
            {
                _hasPreexistingTiles = true;
                _tileTypesInPositions.Add(new Vector2Int(item.X, item.Y), item.TileInPositions);
            }
        }

        KeysForTiles = save.KeysForTiles.ToArray();
        int AdditionalMoves = (int)((GameMaster.Data.love + GameMaster.Data.food) / 0.45f);

        _moves = save.moves + AdditionalMoves; 
        foreach (var item in save.Conditions)
        {
            Condition condition = (Condition)gameObject.AddComponent(Type.GetType((string)item.AllObjects[0]));
            condition.FromMemento(item);
            _conditions.Add(condition);
        }
        _pointsForStars = save.PointsForStars;

        StartPanelInit();
        FindObjectOfType<BetterCamera>().SetCameraPositionAndSize();
        _dat.Close();
        _block.Close();

    }
    public void OnConditionComplete(Condition condition)
    {
        _conditions.Remove(condition);
        condition.Destroy();
        if (_conditions.Count <= 0 && !_levelIsOver)
        {
            _levelIsOver = true;
            WinLevel();
        }
    }
    public void WinLevelEffects()
    {
        var Effect = Instantiate(Firework);
        Effect.transform.position = Camera.main.transform.position + Vector3.forward * 10 + Vector3.right;
        SoundMaster.PlayOneShot("Win");

    }
    public void WinLevel()
    {
        _board.OnLevelEndAndBlockInput();
        GameMaster.OnLevelCompleted();


        WinLevelEffects();

        var temp = Instantiate(EndWinPanel, GameObject.Find("MainCanvas").GetComponent<Canvas>().transform);
        
        var pointsText = GameObject.Find("PointsText").GetComponent<Text>().text = "Points: " + _points + " + " +  Mathf.Abs(_moves-3) + "*5" ;
        _points += Mathf.Abs(_moves-3) * 5;
        List<Image> StarContainers = new List<Image>();
        StarContainers.Add(GameObject.Find("StarContainer1").GetComponent<Image>());
        StarContainers.Add(GameObject.Find("StarContainer2").GetComponent<Image>());
        StarContainers.Add(GameObject.Find("StarContainer3").GetComponent<Image>());

        var OkButon = GameObject.Find("OKbuttonFinishLevel").GetComponent<Button>();

        int starsAmmount = 0;
        for (starsAmmount = 0; starsAmmount < _pointsForStars.Count; starsAmmount++)
        {
            if (_points < _pointsForStars[starsAmmount])
                break;
            StarContainers[starsAmmount].GetComponent<Image>().sprite = Star;
        }

        OkButon.onClick.AddListener(() =>
        {
            SceneLoader.OnLoadSceneComplete.Add(GameMaster.FeedFrog);
            ExitLevel();
        });
    }
    public void RestartLevel()
    {

        GameMaster.LoadNextLevel();
    }
    public void LoseLevel()
    {
        SoundMaster.PlayOneShot("Lose");
        _board.OnLevelEndAndBlockInput();

        var temp = Instantiate(EndLosePanel, GameObject.Find("MainCanvas").GetComponent<Canvas>().transform);
        var pointsText = GameObject.Find("PointsText").GetComponent<Text>().text = "YOU LOSE";

        var OkButon = GameObject.Find("OKbuttonFinishLevel").GetComponent<Button>();

        OkButon.onClick.AddListener(() =>
        {
            SceneLoader.OnLoadSceneComplete.Add(GameMaster.FeedFrogOnLose);
            ExitLevel();
        });

        var ADButon = GameObject.Find("WatchAD").GetComponent<Button>();
        if (!AdsManager.Loaded || _wathcedAd)
        {
            ADButon.gameObject.SetActive(false);
        }
        _wathcedAd = true;

        ADButon.onClick.AddListener(() =>
        {
            
            if (AdsManager.ShowRewardAD(this))
            {
                
            }
            else
            {
                Debug.Log("CantLoadVideo");
            }
            Destroy(temp.gameObject);

        });

    }
    public void ContinueLevel()
    {
        _levelIsOver = false;
        _moves = 3;
        _board.ContinueLevel();
        GameObject.Find("leftMoves").GetComponent<Text>().text = "" + _moves;
    }
    public void ExitLevel()
    {
        SceneLoader.OnLoadSceneComplete.Add(GameMaster.SetButtonData);
        SceneLoader.LoadScene("MainMenu");
    }
    public void MinusMoves()
    {
        _moves--;

        GameObject.Find("leftMoves").GetComponent<Text>().text = "" + _moves;
    }
}

class SaveLevel
{
    public List<MementoElement> Elements;
    public List<MementoCondtion> Conditions;
    public List<string>KeysForTiles;
    public List<int> PointsForStars;
    public int moves;
    public SaveLevel(List<MementoElement> elements, List<MementoCondtion> conditions,  List<string> keyForTiles, List<int> pointsForStars)
    {
        Elements = elements;
        Conditions = conditions;
        KeysForTiles = keyForTiles;
        PointsForStars = pointsForStars;
    }
}