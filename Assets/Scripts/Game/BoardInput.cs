using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
public class BoardInput : MonoBehaviour
{
    private bool _tapped = false;
    private Vector2Int _firstPosition = new Vector2Int(-1,-1);
    private Board _board;

    public GameObject SelectPrefab;
    public GameObject TongueAttackPrefab;
    private void Awake()
    {
        Input input = GetComponent<Input>();
        input.ProcessTap = HandleTap;
        input.ProcessDrag = HandleDrag;
        input.QuickDrag = HandleDragEnd;

        _board = GetComponent<Board>();

    }
    public void BlockInput()
    {
        GetComponent<Input>().enabled = false;
    }
    public void UnblockInput()
    {
        GetComponent<Input>().enabled = true;
    }
    public void StartGoldenFrog()
    {
        GetComponent<Input>().ProcessTap = HandleGoldenFrogTap;
    }
    public void EndGoldenFrog()
    {
        GetComponent<Input>().ProcessTap = HandleTap;
    }
    private void HandleGoldenFrogTap(Vector2 pos)
    {
        Vector2Int posInt = new Vector2Int(Mathf.RoundToInt(Camera.main.ScreenToWorldPoint(pos).x), Mathf.RoundToInt(Camera.main.ScreenToWorldPoint(pos).y));
        
        var temp = Instantiate(TongueAttackPrefab).GetComponent<Tongue>();
        if (UnityEngine.Random.value > 0.5f)
        {
            temp.transform.position = new Vector3(-10, UnityEngine.Random.value*10f);
        }
        else
        {
            temp.transform.position = new Vector3(15, UnityEngine.Random.value * 10f);
        }
        temp.Target = new Vector3(posInt.x, posInt.y);
        _board.AskToDestroy(posInt);
    }
    private void HandleDragEnd(Vector2 startpos, Vector2 pos)
    {
        Vector2Int posInt = new Vector2Int(Mathf.RoundToInt(Camera.main.ScreenToWorldPoint(pos).x), Mathf.RoundToInt(Camera.main.ScreenToWorldPoint(pos).y));
        if (_tapped || !_board.Idle)
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

        GetComponent<Board>().SwapTiles(_firstPosition, _firstPosition - direction);

    }

    private void HandleDrag(Vector2 pos, bool first)
    {
        Vector2Int posInt = new Vector2Int(Mathf.RoundToInt(Camera.main.ScreenToWorldPoint(pos).x), Mathf.RoundToInt(Camera.main.ScreenToWorldPoint(pos).y));
        if (_tapped || !_board.Idle)
        {
            return;
        }

        if (first)
        {
            if (_board.GetTile(posInt) != null)
            {
                _firstPosition = posInt;
            }
        }
    }

    private void HandleTap(Vector2 pos)
    {
        return;

        Vector2Int posInt = new Vector2Int(Mathf.RoundToInt(Camera.main.ScreenToWorldPoint(pos).x), Mathf.RoundToInt(Camera.main.ScreenToWorldPoint(pos).y));

        if (!_board.Idle)
        {
            return;
        }

        if (!_tapped)
        {
           
            if (_board.GetTile(posInt) != null)
            {
                SelectPrefab.SetActive(true);
                SelectPrefab.transform.position = (Vector2)posInt;
                _firstPosition = posInt;
                _tapped = true;
            }
        }
        else if(_tapped)
        {

            if ((_firstPosition - posInt).magnitude > 1.1 || _firstPosition == posInt)
            {
                //if tapped much further, process tapps again
                _tapped = false;
                HandleTap(pos);

                return;
            }

            GetComponent<Board>().SwapTiles(_firstPosition, posInt);
            _firstPosition = new Vector2Int(-1, -1);


            SelectPrefab.SetActive(false);
            _tapped = false;
        }
    }
    
}
