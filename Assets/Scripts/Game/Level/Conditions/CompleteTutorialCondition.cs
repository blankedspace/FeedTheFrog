using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CompleteTutorialCondition : Condition
{

    public override void FromMemento(MementoCondtion memento)
    {

    }
    public override MementoCondtion ToMemento()
    {
        List<object> allParams = new List<object>();
        allParams.Add(typeof(CompleteTutorialCondition));
        return new MementoCondtion(allParams);

    }
    public override void Destroy()
    {

    }
    public override void ConnectToBoard()
    {

    }
    public void UpdateCondition(HashSet<Vector2Int> tiles)
    {

    }
}
