using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LabelOrientationController : MonoBehaviour
{
    float defaultOrientation;

    void Start()
    {
        defaultOrientation = transform.localScale.x;
    }

    void Update()
    {
        if (transform.parent.localScale.x < 0)
        {
            transform.localScale = new Vector3(
                -defaultOrientation, 
                transform.localScale.y, 
                transform.localScale.z
                );
        }
        else
        {
            transform.localScale = new Vector3(
                defaultOrientation,
                transform.localScale.y,
                transform.localScale.z
                );
        }
    }
}
