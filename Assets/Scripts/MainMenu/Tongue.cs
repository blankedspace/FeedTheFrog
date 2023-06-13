using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Tongue : MonoBehaviour
{
    public Vector3 Target;

    private void Start()
    {
        transform.rotation = new Quaternion(0, 0, 0, 0);
        float DeltaX = Target.x - transform.position.x;
        float DeltaY = Target.y - transform.position.y;
        float Angle = Mathf.Rad2Deg * Mathf.Atan2(DeltaY, DeltaX);
        transform.Rotate(0, 0, 90 + Angle);

        float speed = 4f;
        transform.LeanScaleY((Target - transform.position).magnitude * 2, 1f/speed);

        transform.LeanScaleX(1 / (Target - transform.position).magnitude * 2, 1f/speed);

        this.Invoke(() =>  
        {
            transform.LeanScaleY(0, 1f / speed);
            transform.LeanScaleX(1, 1f / speed);
        }, 1f / speed);
        transform.LeanScaleY((Target - transform.position).magnitude * 2, 1f / speed);

        transform.LeanScaleX(5 / (Target - transform.position).magnitude * 2, 1f / speed);
    }

}
