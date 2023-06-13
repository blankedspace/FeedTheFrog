using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LevelElement : MonoBehaviour
{
    private void Awake()
    {
        GetComponent<SpriteRenderer>().color = Color.gray;
    }
    public virtual bool Interact()
    {
        return false;
    }
}


public class MementoElement
{
    public int X;
    public int Y;
    public string ElementType;
    public string TileInPositions;
    public MementoElement(int x, int y, string elementType, string tileInPositions)
    {
        X = x;
        Y = y;
        ElementType = elementType;
        TileInPositions = tileInPositions;
    }
}