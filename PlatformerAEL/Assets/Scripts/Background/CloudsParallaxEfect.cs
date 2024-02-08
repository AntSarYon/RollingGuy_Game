using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CloudsParallaxEfect : MonoBehaviour
{
    void Update()
    {
        //Si la posicion en X llega a las 6 unidades
        if (transform.localPosition.x <= 6)
        {
            //Lo regresamos a su posicion original
            transform.localPosition = new Vector3(
            37,
            transform.localPosition.y,
            transform.localPosition.z
            );
        }
        else
        {
            //Desplazamos las nubes hacia la  izquierda constantemente
            transform.localPosition = new Vector3(
            transform.localPosition.x - (0.25f * Time.deltaTime),
            transform.localPosition.y,
            transform.localPosition.z
            );
        }

    }
}
