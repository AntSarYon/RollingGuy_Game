using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UIElements;

public class EnemyIA : MonoBehaviour
{
    private GameManager gameManager;

    //Velocidad
    //[SerializeField]
    //private float speed = -1f;

    private float vida = 15f;

    private float recievedDamage;

    private float attackDamage;

    //Distancia del Rayo
    //[SerializeField]
    //private float rayDistance = 4f;

    [SerializeField]
    private float distanciaDeteccion;
    
    //Referencias al RigidBody y al Collider
    private Rigidbody2D mRb;
    private BoxCollider2D mCollider;
    private Animator mAnimator;
    private AudioSource mAudioSource;
    private SpriteRenderer mSpriteRenderer;

    [Header("Sonido de Damage")]
    [SerializeField]
    private AudioClip clipDamage;

    [Header("Sonido de Muerte")]
    [SerializeField]
    private AudioClip clipDead;

    //Flag de Atacado
    //[SerializeField]
    //private bool atacado = false;

    //Flag de Recibiendo da�o
    [SerializeField]
    private bool isBeingDamage = false;

    //Controlamos el tiempo de Da�o
    private bool takeDamageTime;
    private float actualDamageTime;
    private float maxDamageTime;

    //------------------------------------------------------------------------------

    private void Awake()
    {
        //Obtenci�n de componentes referencia
        mRb = GetComponent<Rigidbody2D>();
        mCollider = GetComponent<BoxCollider2D>();
        mAnimator = GetComponent<Animator>();
        mSpriteRenderer = GetComponent<SpriteRenderer>();
        mAudioSource = GetComponent<AudioSource>();

        //Inicializamos
        distanciaDeteccion = 4.5f;

        attackDamage = 10;

        takeDamageTime = false;
        actualDamageTime = 0f;
        maxDamageTime = 0.4f;
    }

    //---------------------------------------------------------------------------------
    private void Update()
    {
        //Si no ha sido atacado...
        if (!isBeingDamage)
        {
            //Detectamos al jugador
            DetectarYAtacar();
        }
        //si lo han atacado
        else
        {
            //Si su vida despu�s del ataque es 0; entonces Muere.
            if (vida <= 0)
            {
                //ControlarRecibimientoDeDa�o();
                mAnimator.SetBool("IsDying", true);
                //mAudioSource.PlayOneShot(clipDead, 0.55f);
                mCollider.isTrigger=true;
                
            }
            //Si aun sigue con vida...
            else
            {
                mAnimator.SetBool("IsGettingHurt", true);
                ControlarRecibimientoDeDa�o();
            }
        }

    }
    //------------------------------------------------------------------------------------
    /*private bool VerificarCaida()
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
    }*/

    //---------------------------------------------------------------------

    private void OnCollisionEnter2D(Collision2D collision)
    {
        //Si he colisionado con el Jugador...
        if (collision.transform.CompareTag("Player"))
        {
            // Si el jugador me estaba atacando...
            if (collision.gameObject.GetComponent<PlayerMovement>().IsAttacking)
            {
                //Activamos los Flag de Atacado
                isBeingDamage = true;
                takeDamageTime = true;

                //Almaceno su da�o de ataque
                recievedDamage = collision.gameObject.GetComponent<PlayerMovement>().AttackDamage;
                
                //Llamo al Evento EnemyDamage para aumentar la Barra de ataque
                GameManager.Instance.EnemyDamage();

                //Disminuimos la Vida en base al Da�o recibido
                vida -= recievedDamage;

                mAudioSource.PlayOneShot(clipDamage, 0.75f);
            }
            //Si no me esta atacando; llamo al Evento de hacer Da�o
            else
            {
                //Seteamos el Da�o que ser� aplicado al jugador
                GameManager.Instance.DamageReceivedInProgress = attackDamage;

                //Llamamos al Evento de Jugador da�ado
                GameManager.Instance.PlayerDamage();
            }
        }
    }

    //---------------------------------------------------------------------
    private void OnCollisionExit2D(Collision2D collision)
    {
        //Si he colisionado con el Jugador...
        if (collision.transform.CompareTag("Player"))
        {

        }
    }

    //---------------------------------------------------------------------
    private void OnCollisionStay2D(Collision2D collision)
    {
        //Si me mantengo colisionando con el Jugador...
        if (collision.transform.CompareTag("Player"))
        {
            //Si el Jugador NO ESTA ATACANDO
            if (!collision.gameObject.GetComponent<PlayerMovement>().IsAttacking)
            {
                //Lanzo el evento de Hacer Da�o
                //GameManager.Instance.PlayerDamage(); <------ Con el retroceso de Impacto esto ya no es necesario
            }
                
        }
    }

    //----------------------------------------------------

    private void ControlarRecibimientoDeDa�o()
    {
        //Si debemos calcular el tiempo desde el golpe
        if (takeDamageTime)
        {
            //Le vamos agregamos el DeltaTime;
            actualDamageTime += Time.deltaTime;

            //Si el tiempo de Da�o actual excede al tiempo maximo.
            if (actualDamageTime >= maxDamageTime)
            {
                //Reiniciamos todos los valores y Flag
                mAnimator.SetBool("IsGettingHurt", false);
                
                takeDamageTime = false;
                isBeingDamage = false;

                //Devolvemos el tiempo de Retroceso actual a 0
                actualDamageTime = 0f;
            }
        }
    }

    //------------------------------------------------------------------
    /*private void Mover()
    {
        //Actualiza la velocidad
        mRb.velocity = new Vector2(speed, mRb.velocity.y);
    }*/

    //------------------------------------------------------------------
    /*private void ControlarGirosEnCorniza()
    {
        //Si detecta que hay una caida en esa direcci�n...
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
    }*/

    //------------------------------------------------------------------
    private void DetectarYAtacar()
    {
        //Invocamos 2 Raycasts que cubran un ancho considerable
        //desde la posicion del enemigo
        
        RaycastHit2D raycastLeft = Physics2D.Raycast(
            new Vector2(
                transform.position.x,
                transform.position.y - 1.25f
            ),
            Vector2.left,
            distanciaDeteccion,
            LayerMask.GetMask("Player")
        );
        RaycastHit2D raycastRight = Physics2D.Raycast(
            new Vector2(
                transform.position.x,
                transform.position.y - 1.25f
            ),
            Vector2.right,
            distanciaDeteccion,
            LayerMask.GetMask("Player")
       );

            if (raycastLeft)
            {
                //Le invertimos la escala en X si fuera necesario
                if (transform.localScale.x < 0)
                {
                    transform.localScale = new Vector3(
                        -transform.localScale.x,
                        transform.localScale.y,
                        transform.localScale.z
                        );
                }
                //Activamos la animacion de Ataque
                mAnimator.SetBool("IsAttacking1", true);
            }
            else if (raycastRight)
            {
                //Le invertimos la escala en X si fuera necesario
                if (transform.localScale.x > 0)
                {
                    transform.localScale = new Vector3(
                        -transform.localScale.x,
                        transform.localScale.y,
                        transform.localScale.z
                        );
                }
                //Activamos la animacion de Ataque
                mAnimator.SetBool("IsAttacking2", true);
            }
            else
            {
                mAnimator.SetBool("IsAttacking1", false);
                mAnimator.SetBool("IsAttacking2", false);
            }
    }
}
