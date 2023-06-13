using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class BreakLevelElementCondition : Condition
{
    public List<BreakableLevelElement> Elements;
    public GameObject UpdateVisual;
    public override void FromMemento(MementoCondtion memento)
    {
    }
    public override void Destroy()
    {
        board.OnTileDestroyed.Remove(ToConnect);
        Destroy(this);
    }
    public override MementoCondtion ToMemento()
    {
        List<object> allParams = new List<object>();
        allParams.Add(typeof(BreakLevelElementCondition));
        return new MementoCondtion(allParams);

    }
    public override void ConnectToBoard()
    {
        Elements = new List<BreakableLevelElement>(FindObjectsOfType<BreakableLevelElement>());
        ToConnect = new Board.TilesDestroyed(UpdateCondition);
        board = FindObjectOfType<Board>();
        board.OnTileDestroyed.Add(ToConnect);
    }

    public override void UpdateCondition(HashSet<Vector2Int> tiles)
    {
        
        foreach (var key in tiles)
        {
            if (!GetComponent<Level>().Elements.ContainsKey(key))
            {
                return;
            }
            LevelElement element = GetComponent<Level>().Elements[key];
            if (element.GetType() == typeof(BreakableLevelElement))
            {
                BreakableLevelElement Belement = (BreakableLevelElement)element;

                if (Elements.Contains(Belement))
                {
                    if (Belement.Interact())
                    {
                        Elements.Remove(Belement);
                        UpdateVisual.GetComponentInChildren<Text>().text = ""+Elements.Count;
                    }
                }
                if (Elements.Count <= 0)
                {
                    Complete();
                }
            }
        }
    }
}
