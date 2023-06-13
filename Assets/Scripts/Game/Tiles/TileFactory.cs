using Skytanet.SimpleDatabase;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using UnityEngine.U2D;

public class TileFactory : MonoBehaviour
{
    private SaveFile _saveFile;
    private Stream _block, _dat;
    private void Awake()
    {
        BetterStreamingAssets.Initialize();

        _dat = BetterStreamingAssets.OpenRead("tiles.dat");
        _block = BetterStreamingAssets.OpenRead("tiles.block");
        _saveFile = new SaveFile(_dat, _block);
        if (_saveFile == null)
        {
            Debug.LogError("Couldnt load Tiles.dat from " + Application.dataPath);
        }

    }
    private void OnApplicationQuit()
    {
        _dat.Close();
        _block.Close();
    }
    public Tile Create(string key)
    {
       TileMemento tileMemento = _saveFile.Get<TileMemento>(key);
        if (tileMemento == null)
        {
            Debug.LogError("No tile associated with key: " + key);
        }
        SpriteRenderer objectTile =  new GameObject("Tile").AddComponent<SpriteRenderer>();
        Tile tile = objectTile.gameObject.AddComponent<Tile>();

        objectTile.sprite = SpriteFactory.GetSprite(tileMemento.spriteName);
        tile.Type = tileMemento.type;

        return tile;
    }
}

public class TileMemento
{
    public int type;
    public string spriteName;

}