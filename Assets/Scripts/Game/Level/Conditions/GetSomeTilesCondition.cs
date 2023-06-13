using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class GetSomeTilesCondition : Condition
{

    public int Type = -999;
    public int AmmountToGet = -999;
    private int _ammount = 0;
    public GameObject UpdateVisual;

    public override void Complete()
    {
        GetComponent<Level>().OnConditionComplete(this);
    }

    public override MementoCondtion ToMemento()
    {
        List<object> allParams = new List<object>();
        allParams.Add(typeof(GetSomeTilesCondition));
        allParams.Add(Type);
        allParams.Add(AmmountToGet);
        return new MementoCondtion(allParams);
    }
    public override void FromMemento(MementoCondtion memento)
    {
        Type = (int)Convert.ChangeType(memento.AllObjects[1], typeof(int));
        AmmountToGet = (int)Convert.ChangeType(memento.AllObjects[2], typeof(int));
    }
    //Паттерн наблюдатель

    public override void Destroy()
    {
        board.OnTileDestroyed.Remove(ToConnect);
        Destroy(this);
    }
    public override void UpdateCondition(HashSet<Vector2Int> tiles)
    {
        foreach (var item in tiles)
        {
            if (board.GetTile(item)?.Type == Type)
            {
                Vector2 pos = Camera.main.ScreenToWorldPoint(UpdateVisual.transform.position);
                board.GetTile(item).DestroyAndMove(pos);
                AmmountToGet--;
                UpdateVisual.GetComponentInChildren<Text>().text = "" + AmmountToGet;
            }
            if (AmmountToGet<=0)
            {
                Complete();
                Destroy(UpdateVisual);
                return;
            }
        }
    }
}
