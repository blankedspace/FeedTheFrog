using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using SFB;
using System.IO;

public class TilePanel : MonoBehaviour
{

    public Button Sprite;
    public Image Preview;

    public string SpriteName;
    public TMP_InputField TypeText;
    private void Start()
    {
        Sprite.onClick.AddListener(OpenSprite);
    }
    public void OpenSprite()
    {
        var paths = StandaloneFileBrowser.OpenFilePanel("Open File", "", "", false);
        string spr = Path.GetFileNameWithoutExtension(paths[0]);
        SpriteName = spr;
        Preview.sprite = TileEditor.Instance.Atlas.GetSprite(spr);
        
    }
}
