using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Tile : MonoBehaviour
{
    private bool _destroyed = false;
    public int Type;
    public bool IsMoving = false;

    public System.Action OnMoveAnimEnd;
    public System.Action OnDestroyAnimEnd;

    public bool CanBeSwapped()
    {
        if (Type == 2000)
            return false;

        return true;
    }
    public bool CanBeDestroyed()
    {
        if (Type == 2000)
            return false;

        return true;
    }

    public bool CompareTiles(Tile CompareTo)
    {
        if (CompareTo == null)
        {
            return false;
        }
        if (CompareTo.Type > 1000 || Type > 1000)
        {
            return false;
        }

        if (Type == CompareTo.Type)
        {
            return true;
        }
        return false;
    }

    public void Destroy()
    {
        if (_destroyed)
        {
            return;
        }
        GetComponent<SpriteRenderer>().sortingOrder = 1000;
        SoundMaster.PlayOneShot("Swap");
        LeanTween.scale(gameObject, new Vector3(0,0,0) , 0.5f).setOnComplete( () => 
        {
            if (OnDestroyAnimEnd != null)
            {
                OnDestroyAnimEnd();
            }         
            Destroy(gameObject);
           


        }).setEaseInOutBack();
        _destroyed = true;

    }
    public void DestroyAndMove(Vector2 MoveTo)
    {
        if (_destroyed)
        {
            return;
        }
        GetComponent<SpriteRenderer>().sortingOrder = 1000;
        SoundMaster.PlayOneShot("Swap");
        LeanTween.move(gameObject, MoveTo, 0.5f).setOnComplete(() =>
        {
            if (OnDestroyAnimEnd != null)
            {
                OnDestroyAnimEnd();
            }
            Destroy(gameObject);
            

        }).setEaseInOutBack();
        _destroyed = true;
    }
    public void MoveTo(Vector2 pos)
    {
        float speed = 14f;
        float distance = (transform.position - (Vector3)pos).magnitude;
        float time = distance/speed;
        IsMoving = true;


        LeanTween.move(gameObject, pos, time).setOnComplete(onMoveAnimEnd).setEaseInCubic(); 
        
    }

    private void onMoveAnimEnd()
    {
        SoundMaster.PlayOneShot("Clack");
        IsMoving = false;
        OnMoveAnimEnd?.Invoke();
        OnMoveAnimEnd = null;
    }
}
