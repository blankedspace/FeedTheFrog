using Skytanet.SimpleDatabase;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.U2D;
using UnityEngine.UI;

public class TileEditor : MonoBehaviour
{
    private SaveFile _saveFile;
    public TilePanel TilePrefab;
    private List<TilePanel> _allPanels = new List<TilePanel>();
    public SpriteAtlas Atlas;
    public Button button;
    public Button saveButton;
    public static TileEditor Instance;

    void Start()
    {
        Instance = this;
        _saveFile = new SaveFile("Tiles", Application.streamingAssetsPath);
        foreach (var key in _saveFile.GetKeys())
        {
            TileMemento tempTile = _saveFile.Get<TileMemento>(key);
            AddPanel();
            _allPanels[_allPanels.Count - 1].Preview.sprite = Atlas.GetSprite(tempTile.spriteName);
            _allPanels[_allPanels.Count - 1].SpriteName = tempTile.spriteName;
            _allPanels[_allPanels.Count - 1].TypeText.text = ""+tempTile.type;
        }
        button.onClick.AddListener(AddPanel);
        saveButton.onClick.AddListener(SaveFile);
    }

    private void OnApplicationQuit()
    {
        _saveFile.Close();
    }
    public void AddPanel()
    {
        var temp = Instantiate(TilePrefab, transform);
        _allPanels.Add(temp);
        button.transform.SetAsLastSibling();
    }
    public void SaveFile()
    {
        TileMemento tempTile = new TileMemento();
        foreach (var item in _allPanels)
        {
            
            tempTile.type = Convert.ToInt32(item.TypeText.text);
            tempTile.spriteName = item.SpriteName;
            Debug.Log(item.SpriteName);
            _saveFile.Set(Convert.ToString(tempTile.type), tempTile);
        }
    }
}