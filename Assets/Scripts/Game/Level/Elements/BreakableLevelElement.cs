using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BreakableLevelElement : LevelElement
{
    private SpriteRenderer _visual;
    private int _health = 3;

    private void Start()
    {
        _visual = Instantiate(FindObjectOfType<Level>().BreakableLevelElementPrefab,transform).GetComponent<SpriteRenderer>();

        if (FindObjectOfType<BreakLevelElementCondition>() == null)
        {
            Debug.LogError("No Condtion  BreakLevelElementCondition");
        }
    }
    public override bool Interact()
    {
        SoundMaster.PlayOneShot("BreakingIce",2f);
        _health--;
        switch (_health)
        {
            case 2:
                _visual.sprite = SpriteFactory.GetSprite("IceHalfBroken");
                break;
            case 1:
                _visual.sprite = SpriteFactory.GetSprite("IceBroken");
                break;
            case 0:
             
                Destroy(_visual.gameObject);
                return true;
            default:
                break;
        }
        return false;

    }
}
