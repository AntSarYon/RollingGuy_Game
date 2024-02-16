using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InstructionsController : MonoBehaviour
{
    private Animator mAnimator;
    private bool wasActivated;

    //------------------------------------------------------

    void Awake()
    {
        //Obtenemos referencial al cnaimator
        mAnimator = GetComponent<Animator>();

        //Iniciamos con el Flag de Checkpoint desactivado
        wasActivated = false;
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        //Si aun no se ha activado el checkpoint, y el objeto con el que colisionamos es el Player...
        if (!wasActivated && collision.CompareTag("Player"))
        {
            //Activamos el Flagb de activado
            wasActivated = true;
            //Reproduc9imola Animacion para que se muestre
            mAnimator.Play("Show");
        }

    }
}
