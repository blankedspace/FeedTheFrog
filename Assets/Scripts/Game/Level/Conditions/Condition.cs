using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class Condition : MonoBehaviour
{
    public Board.TilesDestroyed ToConnect;
    public Board board;
    public void Awake()
    {
        ConnectToBoard();
    }

    public virtual void Complete()
    {
        GetComponent<Level>().OnConditionComplete(this);
    }

    public virtual MementoCondtion ToMemento()
    {
        return new MementoCondtion(null);
    }
    public virtual void FromMemento(MementoCondtion memento) { }
    public virtual void ConnectToBoard()
    {
        board = FindObjectOfType<Board>();
        ToConnect = new Board.TilesDestroyed(UpdateCondition);
        board.OnTileDestroyed.Add(ToConnect);
    }
    public virtual void Destroy()
    { 
    }
    public virtual void UpdateCondition(HashSet<Vector2Int> tiles)
    { 
    }
}

public class MementoCondtion
{
    public List<object> AllObjects;
    public MementoCondtion(List<object> obj)
    {
        AllObjects = obj;
    }
}