using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TomEasterEggController : MonoBehaviour
{
    private Animator mAnimator;
    private bool wasFound;

    //------------------------------------------------------

    void Awake()
    {
        //Obtenemos referencial al cnaimator
        mAnimator = GetComponent<Animator>();

        //Iniciamos con el Flag de Checkpoint desactivado
        wasFound = false;
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        //Si aun no se ha activado el checkpoint, y el objeto con el que colisionamos es el Player...
        if (!wasFound && collision.CompareTag("Player"))
        {
            //Activamos el Flagb de activado
            wasFound = true;
            //Reproduc9imola Animacion para que se muestre
            mAnimator.Play("Celebrate");
            //Reproducimos sonido tierno
            AudioManager.instance.PlaySfx("CuteSound");
        }

    }
}
