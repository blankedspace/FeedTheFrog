using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class GameOver : MonoBehaviour
{
    private Button _button;
    private bool _finished = false;
    // Start is called before the first frame update
    void Start()
    {
        _button = GetComponent<Button>();
        SoundMaster.PlayOneShot("EndSong");

    }

    // Update is called once per frame
    void Update()
    {
        if (_button.image.color.a > 0.9 && !_finished)
        {
            _button.onClick.AddListener(() =>
            {
                GameMaster.ResetData();

                SceneLoader.OnLoadSceneComplete.Add(GameMaster.SetButtonData);
                SceneLoader.LoadScene("MainMenu");


            }
            );
            _finished = true;
        }
    }
}
