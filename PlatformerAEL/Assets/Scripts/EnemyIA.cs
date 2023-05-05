using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UIElements;

public class EnemyIA : MonoBehaviour
{
    //Velocidad
    [SerializeField]
    private float speed = 1f;

    private float vida = 10f;

    private float dañoRecibido;

    //Distancia del Rayo
    [SerializeField]
    private float rayDistance = 4f;

    [SerializeField]
    private float distanciaDeteccion = 7.5f;
    
    //Referencias al RigidBody y al Collider
    private Rigidbody2D mRb;
    private BoxCollider2D mCollider;

    private bool atacado = false;

    //------------------------------------------------------------------------------

    private void Start()
    {
        //Obtención de componentes referencia
        mRb = GetComponent<Rigidbody2D>();
        mCollider = GetComponent<BoxCollider2D>();

        GameManager.Instance.OnEnemyDamage += OnEnemyDamageDelegate;

    }

    //------------------------------------------------------------
    // Comportamiento cuando un Enemigo (Yo) sufra daño
    private void OnEnemyDamageDelegate(object sender, EventArgs e)
    {
        //Activo el Flag de Atacado
        atacado = true;

        //Reduzco el daño recibido de su vida
        vida -= dañoRecibido;
        print("Mi vida ahora es" + vida);
    }

    //---------------------------------------------------------------------------------
    private void Update()
    {
        //Si no ha sido atacado...
        if (atacado == false)
        {
            //Movemos al Enemigo
            Mover();

            //Detección del jugador
            DetectarJugador();

            //Controlamos el Giro en Corniza
            ControlarGirosEnCorniza();
        }
        //si lo han atacado
        else
        {
            //Si su vida después del ataque es 0; entonces Muere.
            if (vida <= 0)
            {
                print("Me morí");
                mCollider.isTrigger = true;
                gameObject.SetActive(false);
            }
            //Si aun sigue con vida...
            else
            {

            }
        }

            //Si estoy tocando a un Jugador
            //if (mCollider.IsTouchingLayers(LayerMask.GetMask("Player")))
        //{
            //Invoco al evento PlayerDamage
            //GameManager.Instance.PlayerDamage();
        //}
        
    }
    //------------------------------------------------------------------------------------
    private bool VerificarCaida()
    {
        //Lanzar raycast hacia el suelo delante
        var hit = Physics2D.Raycast(
            transform.position,
            new Vector2(
                mRb.velocity.x < 0f ? -1 : 1,
                -1f
            ).normalized,
            rayDistance,
            LayerMask.GetMask("Ground") //Buscamos un choque con la capa Ground
        );

        //Retornamos el resultado (Impacto o caida)
        return hit;
    }

    //---------------------------------------------------------------------
    /*
    private void OnDrawGizmos()
    {
        //Dibujamos el Gizmo de la detección de precipicios
        mRb = GetComponent<Rigidbody2D>();

        Gizmos.DrawRay(
            transform.position,
            new Vector2(
                mRb.velocity.x < 0f ? -1 : 1,
                -1f
            ).normalized*4f
       );

        //Dibujamos el Gizmo para la deteccion del Player
       Gizmos.DrawRay(
            new Vector2(
                transform.position.x,
                transform.position.y - 0.95f
            ),
            new Vector2(
                mRb.velocity.x < 0f ? -1 : 1,
                0
            ).normalized * 7.5f
       );

    }*/

    private void OnCollisionEnter2D(Collision2D collision)
    {
        //Si he colisionado con el Jugador...
        if (collision.transform.CompareTag("Player"))
        {
            // Si el jugador me estaba atacando...
            if (collision.transform.GetComponent<PlayerMovement>().IsAttacking == true)
            {
                print("Me estas atacando");

                //Almaceno su daño de ataque
                dañoRecibido = collision.transform.GetComponent<PlayerMovement>().AttackDamage;
                print("tu daño es" + dañoRecibido);

                //Llamo al Evento EnemyDamage
                GameManager.Instance.EnemyDamage();
            }
            //Si no me esta atacando; llamo al Evento de hacer Daño
            else
            {
                GameManager.Instance.PlayerDamage();
            }
        }
    }

    //------------------------------------------------------------------
    private void Mover()
    {
        //Actualiza la velocidad
        mRb.velocity = new Vector2(speed, mRb.velocity.y);
    }

    //------------------------------------------------------------------
    private void ControlarGirosEnCorniza()
    {
        //Si detecta que hay una caida en esa dirección...
        if (!VerificarCaida())
        {
            // Cambiar direccion
            speed *= -1;

            //Volteate
            transform.localScale = new Vector3(
                Mathf.Sign(mRb.velocity.x) * transform.localScale.x,
                transform.localScale.y,
                transform.localScale.z
            );
        }
    }

    //------------------------------------------------------------------
    private void DetectarJugador()
    {
        RaycastHit2D raycast = Physics2D.Raycast(
            new Vector2(
                transform.position.x,
                transform.position.y - 0.95f
            ),
            new Vector2(
                mRb.velocity.x < 0f ? -1 : 1,
                0f
                ),
            distanciaDeteccion,
            LayerMask.GetMask("Player")
       );

        if (raycast)
        {
            print("Te estoy viendo, desgraciado");
        }
    }

}
