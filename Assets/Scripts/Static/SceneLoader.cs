using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class SceneLoader : MonoBehaviour
{
    private static AsyncOperation _loading;
    private static SceneLoader _instance;
    public Image LoadingBar;
    public Image Hint;
    public Image Background;
    public static List<System.Action> OnLoadSceneComplete;
    public Sprite[] Hints;
    public bool FirstScene;

    public Color[] colors;

    private void Start()
    {
        if (FirstScene)
        {

            _instance = this;
        
            Utility.Invoke(this, () => LoadScene("GameMaster"), 0.25f);
            
        }
    
    }
    public void Initialize()
    {
        _instance = this; 
        OnLoadSceneComplete = new List<System.Action>();

   
        DontDestroyOnLoad(this);

    }
    public static void LoadScene(string sceneName)
    {
        _instance.StopAllCoroutines();
        _instance.gameObject.SetActive(true);
        foreach (var item in _instance.GetComponentsInChildren<Image>())
        {
            item.color = new Color(1, 1, 1, 1);
        }
        _instance.Background.color = _instance.colors[Random.Range(0, _instance.colors.Length)];
        _instance.Hint.sprite = _instance.Hints[Random.Range(0, _instance.Hints.Length)];
        _loading = SceneManager.LoadSceneAsync(sceneName, LoadSceneMode.Single);
        var load = SceneLoadingProgress();
        _instance.StartCoroutine(load);

    }
    public static IEnumerator SceneLoadingProgress()
    {
   
       
        
        while (!_loading.isDone)
        {
            _instance.LoadingBar.transform.RotateAround(_instance.LoadingBar.transform.position, Vector3.forward, -10);
   
            yield return new WaitForEndOfFrame();
        }
       
        foreach (var item in OnLoadSceneComplete)
        {
            item();
            
        }
        float time = 0;

        while (time < 1f)
        {
            _instance.LoadingBar.transform.RotateAround(_instance.LoadingBar.transform.position, Vector3.forward, -10); ;
            time += 0.01f;
            yield return new WaitForSeconds(0.01f);
        }
        OnLoadSceneComplete.Clear();
        var Anim = Animation();
        _instance.StartCoroutine(Anim);
    }
    public static IEnumerator Animation()
    {
        float time = 0;
        float until = 0.5f;
        var Images = _instance.GetComponentsInChildren<Image>();
        while (time < until)
        {
            time += 0.05f;
            foreach (var item in Images)
            {
                if(_loading.isDone)
                item.color = Color.Lerp(item.color, new Color(0, 0, 0, 0), time/until);
       
            }
            yield return new WaitForSeconds(0.05f);
        }
        _instance.gameObject.SetActive(false);
    }
}
