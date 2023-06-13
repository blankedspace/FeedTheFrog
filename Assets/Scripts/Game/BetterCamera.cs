using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BetterCamera : MonoBehaviour
{
    private Level _level;
    private Camera _camera;
    private Vector2 _left = new Vector2(99,0);
    private Vector2 _right = new Vector2(0,0);

    private Vector2 _top = new Vector2(0, -99);
    private Vector2 _bottom = new Vector2(0, 99);
   
    public Transform Background;
    void Awake()
    {
        _level = FindObjectOfType<Level>();
        _camera = GetComponent<Camera>();
       
    }


    public void SetCameraPositionAndSize()
    {
        GetCorners();
        Vector2 center = (_right + _left) / 2;
        float y = ((_top + _bottom)/2).y;
        transform.position = (Vector3)center + Vector3.back*10f;
        transform.position = new Vector3(transform.position.x, y, -10);
        var width = (_right.x - _left.x) + 1f;

        if(_camera.aspect > 0.6)
        {
           width = (_right.x - _left.x) + 2.5f;
           transform.position = new Vector3(transform.position.x, transform.position.y + 1, -10);
        }
        var height = width / _camera.aspect;
        Background.localScale *= width/6;
        _camera.orthographicSize = height / 2;
    }

    private void GetCorners()
    {
        foreach (var item in _level.Elements)
        {

            if ((item.Key.x < (_left.x)))
            {
                _left = item.Key;
            }
            if ((item.Key.x  > (_right.x)))
            {
                _right = item.Key;
            }

            if ((item.Key.y > (_top.y)))
            {
                _top = item.Key;
            }
            if ((item.Key.y < (_bottom.y)))
            {
                _bottom = item.Key;
            }
        }
    }

}
