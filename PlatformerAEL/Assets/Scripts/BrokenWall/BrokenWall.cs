using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BrokenWall : MonoBehaviour
{
    private bool broken;
    private Collider2D mCollider;
    private Animator mAnimator;

    //-----------------------------------------------------------

    void Awake()
    {
        //Iniciamos con el Flag de Broken desactivado
        broken = false;

        //Obtenemos referencia a Componentes
        mCollider = GetComponent<Collider2D>();
        mAnimator = GetComponent<Animator>();
    }

    //-----------------------------------------------------------

    void Update()
    {
        //Si el muro ya esta roto...
        if (broken)
        {
            mCollider.isTrigger = true;
        }
        //Si no esta roto...
        else 
        {
            //Si el jugador esta atacando, y se está moviendo horizontalmente
            if (PlayerMovement.Instance.IsAttacking) // && (PlayerMovement.Instance.MRb.velocity.x > 20 || PlayerMovement.Instance.MRb.velocity.x < -20))
            {
                mCollider.isTrigger = true;
            }
            else
            {
                //Hacemos el Muro solido
                mCollider.isTrigger = false;
            }
        }
    }

    //-----------------------------------------------------------
    /*
    private void OnTriggerEnter2D(Collider2D collision)
    {
        //Si el jugador entra en xontacto con nosotros...
        if (collision.CompareTag("Player"))
        {
            //Si aun no esta roto
            if (!broken)
            {
                //Activamos el Flag de Broken
                broken = true;

                //Reproducimos el ruido de algo rompiendose
                AudioManager.instance.PlaySfx("Damage3");

                //Reproducimos la Animacion de romperse
                mAnimator.Play("Break");
            }

        }
    }*/

    private void OnTriggerStay2D(Collider2D collision)
    {
        //Si el jugador entra en xontacto con nosotros...
        if (collision.CompareTag("Player"))
        {
            //Si aun no esta roto
            if (!broken)
            {
                //Activamos el Flag de Broken
                broken = true;

                //Reproducimos el ruido de algo rompiendose
                AudioManager.instance.PlaySfx("Damage3");

                //Reproducimos la Animacion de romperse
                mAnimator.Play("Break");
            }

        }
    }
}
