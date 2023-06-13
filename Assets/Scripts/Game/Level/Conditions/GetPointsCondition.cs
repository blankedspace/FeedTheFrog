using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class GetPointsCondition : Condition
{
    public int PointsToGet = 100;
    public int Points;
    public GameObject UpdateVisual;

    public override void FromMemento(MementoCondtion memento)
    {
        PointsToGet = (int)Convert.ChangeType(memento.AllObjects[1], typeof(int));
    }
    public override MementoCondtion ToMemento()
    {
        List<object> allParams = new List<object>();
        allParams.Add(typeof(GetPointsCondition));
        allParams.Add(PointsToGet);
        return new MementoCondtion(allParams);

    }
    public override void Destroy()
    {
        board.OnTileDestroyed.Remove(ToConnect);
        Destroy(this);
    }

    public override void UpdateCondition(HashSet<Vector2Int> tiles)
    {
        foreach (var item in tiles)
        {
            Points++;
            UpdateVisual.GetComponentInChildren<Text>().text = "" + (PointsToGet - Points);
            if (Points >= PointsToGet)
            {

                Complete();
                return;
            }
        }
    }
}
