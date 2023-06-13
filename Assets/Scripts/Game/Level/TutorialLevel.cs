
using System.Collections;
using System.Collections.Generic;

using UnityEngine;


public class TutorialLevel : MonoBehaviour
{
    private Level _level;
    private Board _board;
    private Vector2Int _firstPosition;
    private List<Vector2Int> AllowSwap = new List<Vector2Int>();
    private GameObject _finger;

    private bool _secondComplete = false;
    private GameObject _tongueAttackPrefab;
    private bool _useFrog = false;
    private int _eatedWithFrog = 0;


    private void Start()
    {
        _tongueAttackPrefab = GetComponent<BoardInput>().TongueAttackPrefab;
        GetComponent<BoardInput>().enabled = false;
        _level = FindObjectOfType<Level>();
        _finger = Instantiate(_level.FingerPrefab);
        _finger.SetActive(false);
        defaultSprite = _finger.GetComponentInChildren<SpriteRenderer>().sprite;
        _board = FindObjectOfType<Board>();
        Input input = GetComponent<Input>();
        input.ProcessDrag = HandleDrag;
        input.QuickDrag = HandleDragEnd;


        AllowSwap.Clear();
        AllowSwap.Add(new Vector2Int(3, 1));
        AllowSwap.Add(new Vector2Int(3, 2));
        _finger.SetActive(true);
        
        StartCoroutine("MoveOne");

        if (GameMaster.Data.CurrentLevel == 3)
        {
            StopAllCoroutines();
            StartCoroutine("MoveThree");
            AllowSwap.Clear();
            AllowSwap.Add(new Vector2Int(3, 2));
            AllowSwap.Add(new Vector2Int(2, 2));
            FindObjectOfType<BoardCombos>().FrogTimeActive = 999;

        }
    }

    public IEnumerator MoveOne()
    {
        while (true)
        {
            _finger.transform.position = new Vector2(3, 1);
            LeanTween.move(_finger, new Vector2(3, 2), 0.5f);
            yield return new WaitForSeconds(1);
        }
    }
    public IEnumerator MoveTwo()
    {
        while (true)
        {
            _finger.transform.position = new Vector2(3, 2);
            LeanTween.move(_finger, new Vector2(4, 2), 0.5f);
            yield return new WaitForSeconds(1);
        }
    }
    public IEnumerator MoveThree()
    {
        while (true)
        {
            _finger.transform.position = new Vector2(2, 2);
            LeanTween.move(_finger, new Vector2(3, 2), 0.5f);
            yield return new WaitForSeconds(1);
        }
    }
    public IEnumerator MoveFour()
    {
        while (true)
        {
            _finger.transform.position = new Vector2(3, 0);
            LeanTween.move(_finger, new Vector2(4, 0), 0.5f);
            yield return new WaitForSeconds(1);
        }
    }
    public Sprite defaultSprite;
    public IEnumerator ClickFrog()
    {
        while (true)
        {
            _finger.transform.position = new Vector2(2 + Random.Range(0, 2), 1 + Random.Range(0, 2));
            _finger.GetComponentInChildren<SpriteRenderer>().sprite = _level.FingerClosed;
            yield return new WaitForSeconds(0.25f);

            _finger.GetComponentInChildren<SpriteRenderer>().sprite = defaultSprite;
            yield return new WaitForSeconds(0.25f);
        }

    }
    private void HandleDragEnd(Vector2 startpos, Vector2 pos)
    {
        Vector2Int posInt = new Vector2Int(Mathf.RoundToInt(Camera.main.ScreenToWorldPoint(pos).x), Mathf.RoundToInt(Camera.main.ScreenToWorldPoint(pos).y));
        if (!_board.Idle)
        {
            return;
        }

        if ((_firstPosition - posInt).magnitude > 2 || _firstPosition == posInt)
        {

            return;
        }

        Vector2Int direction = new Vector2Int();
        float max = 0;
        foreach (var item in Board.UDLR)
        {
            float magnitude = Vector3.Dot((Vector2)(_firstPosition - posInt), (Vector2)item);
            if (magnitude >= max)
            {
                max = magnitude;
                direction = item;
            }
        }
        if (AllowSwap.Contains(_firstPosition - direction))
        {
            GetComponent<Board>().SwapTiles(_firstPosition, _firstPosition - direction);
            if (GameMaster.Data.CurrentLevel == 1)
            {
                StopAllCoroutines();
                Destroy(_finger);
                AllowSwap.Clear();
                Utility.Invoke(this, () => { _level.WinLevel(); }, 0.5f);
                return;
            }
            if (!_secondComplete && GameMaster.Data.CurrentLevel == 2)
            {
                StopAllCoroutines();
                StartCoroutine("MoveTwo");
                _finger.transform.position = new Vector2(3, 2);
                AllowSwap.Remove(new Vector2Int(3, 1));
                AllowSwap.Add(new Vector2Int(3, 3));
                AllowSwap.Add(new Vector2Int(4, 2));
                AllowSwap.Add(new Vector2Int(2, 2));
                _secondComplete = true;
                return;
            }
            if (_secondComplete && GameMaster.Data.CurrentLevel == 2)
            {
                StopAllCoroutines();
                Destroy(_finger);
   
                _level.WinLevel();
                return;
            }

            if (!_useFrog && GameMaster.Data.CurrentLevel == 3)
            {
                StopAllCoroutines();
                _finger.transform.position = new Vector2(3, 0);
                StartCoroutine("MoveFour");
                AllowSwap.Clear();
                AllowSwap.Add(new Vector2Int(3, 0));
                AllowSwap.Add(new Vector2Int(4, 0));
                _useFrog = true;
           
                return;
            }
        }
    }

    internal void StartGoldenFrog()
    {
        StopAllCoroutines();
        StartCoroutine("ClickFrog");
        GetComponent<Input>().ProcessTap = HandleGoldenFrogTap;
    }
    private void HandleGoldenFrogTap(Vector2 pos)
    {
        Vector2Int posInt = new Vector2Int(Mathf.RoundToInt(Camera.main.ScreenToWorldPoint(pos).x), Mathf.RoundToInt(Camera.main.ScreenToWorldPoint(pos).y));

        var temp = Instantiate(_tongueAttackPrefab).GetComponent<Tongue>();
        if (UnityEngine.Random.value > 0.5f)
        {
            temp.transform.position = new Vector3(-10, UnityEngine.Random.value * 10f);
        }
        else
        {
            temp.transform.position = new Vector3(15, UnityEngine.Random.value * 10f);
        }
        temp.Target = new Vector3(posInt.x, posInt.y);
        _board.AskToDestroy(posInt);
        _eatedWithFrog++;
        if (_eatedWithFrog > 3)
        {

            _level.WinLevel(); 
            return;
        }
    }
    private void HandleDrag(Vector2 pos, bool first)
    {
        Vector2Int posInt = new Vector2Int(Mathf.RoundToInt(Camera.main.ScreenToWorldPoint(pos).x), Mathf.RoundToInt(Camera.main.ScreenToWorldPoint(pos).y));
        if (!_board.Idle)
        {
            return;
        }

        if (first)
        {
            if (_board.GetTile(posInt) != null)
            {
                if(AllowSwap.Contains(posInt))
                    _firstPosition = posInt;
            }
        }
    }

}
