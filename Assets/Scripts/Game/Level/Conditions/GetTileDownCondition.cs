using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GetTileDownCondition : Condition
{
    public int TileTypeToGetDown;
    private Level _level;
    public override void FromMemento(MementoCondtion memento)
    {

        TileTypeToGetDown = (int)Convert.ChangeType(memento.AllObjects[1], typeof(int));
    }

    public override void Destroy()
    {
        board.OnTileDestroyed.Remove(ToConnect);
        Destroy(this);
    }
    public override MementoCondtion ToMemento()
    {
        List<object> allParams = new List<object>();
        allParams.Add(typeof(GetTileDownCondition));
        allParams.Add(TileTypeToGetDown);
        return new MementoCondtion(allParams);

    }
    public override void ConnectToBoard()
    {

        board = FindObjectOfType<Board>();
        _level = FindObjectOfType<Level>();
        ToConnect = new Board.TilesDestroyed(UpdateCondition);
        board.OnTileDestroyed.Add(ToConnect);
    }

    public override void UpdateCondition(HashSet<Vector2Int> tiles)
    {

        bool done = true;
        foreach (var key in _level.Elements.Keys)
        {
            Tile tempTile = board.GetTile(key);
            if (tempTile != null && tempTile.Type == TileTypeToGetDown)
            {
               
                for (int i = 1; i < 100; i++)
                {
                    Vector2Int keybelow = key + Vector2Int.down * i;
                    
                    if (board.GetTile(keybelow) != null && !tiles.Contains(keybelow))
                    {
 
                        done = false;
                    }
                }
            }
        }

        if (done)
            Complete();
    }
}

