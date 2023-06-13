using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class Frog : MonoBehaviour
{

    private SpriteRenderer _spriteRenderer;
    private float _animationTimer = 0f;


    private float _distanceNow = 2;
    private float _distanceFromPrev = 0;
    private float _timePetted = 0;
    private Vector2 _prevPosFinger;

    private float _timerFoodAndLove = 0;
    private bool _feeding = false;

    public Sprite FeedingTime;
    public Sprite Sitting;
    public Sprite Blink;

  
    public GameObject FoodPrefab;
    public GameObject HeartsPrefab;
    public TextMeshProUGUI AdditionalMovesText;

    public Image LoveBar;
    public Image FoodBar;

    public Input.processDrag ProcessDrag;


    private void Awake()
    {
        _spriteRenderer = GetComponent<SpriteRenderer>();
        Sitting = _spriteRenderer.sprite;
        StartCoroutine("Blinking");
        FindObjectOfType<Input>().ProcessDrag = PetTheFrog;
        FindObjectOfType<Input>().QuickDrag = PetEnded;

        transform.localScale = new Vector3(2,2,1) + new Vector3(0.03f, 0.03f, 0.03f) * GameMaster.Data.size * 7;

        int AdditionalMoves = (int)((GameMaster.Data.love + GameMaster.Data.food) / 0.45f);
        AdditionalMovesText.text = "+" + AdditionalMoves + " Bonus moves";
    }
    public void PetTheFrog(Vector2 pos, bool first)
    {

        _spriteRenderer.sprite = Blink;
        Vector2 position = Camera.main.ScreenToWorldPoint(pos);

        if (((Vector2)transform.position - position).magnitude < _distanceNow && (position - _prevPosFinger).magnitude > _distanceFromPrev)
        {
            _timePetted += Time.deltaTime;
        }
        else
        {
            _timePetted -= Time.deltaTime/10f;
            if (_timePetted < 0)
            {
                _timePetted = 0;
            }
        }
        if (_timePetted > 1f)
        {
            _timePetted = 0;
            var temp = Instantiate(HeartsPrefab);
            GameMaster.Data.AddLove(0.15f);
            LoveBar.fillAmount = GameMaster.Data.love;
            temp.transform.position = new Vector3(transform.position.x + (UnityEngine.Random.value - 0.5f) * 4f, transform.position.y + UnityEngine.Random.value * 2f);
            Destroy(temp, 2f);

        }
        else if (_timePetted < 0.3f)
        {

            _spriteRenderer.sprite = Sitting;
        }
        _prevPosFinger = position;
    }
    public void PetEnded(Vector2 startpos, Vector2 pos)
    {
        _spriteRenderer.sprite = Sitting;
        _timePetted = 0;
    }

  
    public void Update()
    {

        _timerFoodAndLove += Time.deltaTime;
        if (_timerFoodAndLove > 0.3f && GameMaster.Data.CurrentLevel <= 20)
        {
            _timerFoodAndLove = 0;
            GameMaster.Data.AddLove(-0.0025f);
            LoveBar.fillAmount = GameMaster.Data.love;
            GameMaster.Data.AddFood(-0.0025f);
            FoodBar.fillAmount = GameMaster.Data.food;
            int AdditionalMoves = (int)((GameMaster.Data.love + GameMaster.Data.food) / 0.45f);
            AdditionalMovesText.text = "+" + AdditionalMoves + " Bonus moves";
        }
    }
    public IEnumerator Blinking()
    {

        while (true)
        {
            _spriteRenderer.sprite = Blink;
            yield return new WaitForSeconds(0.15f + UnityEngine.Random.value *0.25f);
            _spriteRenderer.sprite = Sitting;
            yield return new WaitForSeconds(3f + UnityEngine.Random.value*3f);
        }
    }

    public void Feed(float Ammount)
    {
        
        _animationTimer = 0;
        for (int i = 0; i < Ammount; i++)
        {
            _feeding = true;
            StartCoroutine("Animation");
            _animationTimer += 0.6f;
        }
        StartCoroutine("SetSize");
       
    }

    public IEnumerator SetSize()
    {
        yield return new WaitForSeconds(_animationTimer + 2);
        if (!_feeding)
        {
            transform.localScale = new Vector3(2, 2, 1) + new Vector3(0.03f, 0.03f, 0.03f) * GameMaster.Data.size * 7;
            _spriteRenderer.sprite = Sitting;
        }
    
    }

    public IEnumerator Animation()
    {
        yield return new WaitForSeconds(_animationTimer);
       
        float time = 0;
        var food = Instantiate(FoodPrefab);
        food.transform.position = transform.position + Vector3.up * 4f  + Vector3.right * Random.Range(-0.2f,0.2f);
        while ((food.transform.position - transform.position).magnitude >= 0.4f)
        {
            _spriteRenderer.sprite = FeedingTime;
            time += Time.deltaTime;
            food.transform.position -= Vector3.up * Time.deltaTime * 3f;
            _feeding = true;
            yield return new WaitForEndOfFrame();
        }
        if (GameMaster.Data.CurrentLevel > 20)
        {
            AdditionalMovesText.text = GameMaster.Data.size + "/100";
        }
        _feeding = false;
        Destroy(food);
        GameMaster.Data.AddFood(0.10f);
        FoodBar.fillAmount = GameMaster.Data.food;
        
        transform.localScale = transform.localScale + new Vector3(0.03f, 0.03f, 0.03f);
    }
}
