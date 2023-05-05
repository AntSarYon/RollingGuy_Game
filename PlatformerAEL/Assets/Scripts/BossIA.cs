using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BossIA : MonoBehaviour
{

    [SerializeField]
    private float speed = 3f;

    [Header("Run Speed")]
    [SerializeField]
    private float runSpeed = 10f;

    [SerializeField]
    private float rayDistance = 3.6f;
    [SerializeField]
    private float rayDistanceFromPlayer = 14f;
    [SerializeField]
    private float rayDistanceToAttack = 3.5f;
    private Rigidbody2D mRb;
    private BoxCollider2D mCollider;
    private Animator mAnimator;
    private bool isAttacking1;
    private bool isAttacking2;
    private bool isRunning;
    private bool isStartingToRun;
    private bool isDying;
    private bool isGettingHurt;


    private void Start()
    {
        //Obtenemos componentes
        mRb = GetComponent<Rigidbody2D>();
        mCollider = GetComponent<BoxCollider2D>();
        mAnimator = GetComponent<Animator>();

        //Inicializamos
        isAttacking1 = false;
        isAttacking2 = false;
        isRunning = false;
        isStartingToRun = false;
        isGettingHurt = false;
        isDying = false;
    }

    private void Update()
    {
        mRb.velocity = new Vector2(speed, mRb.velocity.y);
        if (!VerificarPared())
        {
            // Cambiar direccion
            speed *= -1;
            runSpeed *= -1;
            transform.localScale = new Vector3(
                Mathf.Sign(mRb.velocity.x),
                transform.localScale.y,
                transform.localScale.z
            );
        }

        ControlarIsRunning(AcercarseAlJugador());
        ControlarAtaque(AtacarAlJugador());
    }

    private bool VerificarPared()
    {
        //Lanzar raycast
        var hit = Physics2D.Raycast(
            transform.position,
            new Vector2(
                mRb.velocity.x < 0f ? -1 : 1,
                0f
            ).normalized,
            rayDistance,
            LayerMask.GetMask("Ground")
        );
        if(hit) return false;
        else return true;
    }

    private bool AtacarAlJugador()
    {
        //Raycast que detecte al jugador de cerca
        var hit2 = Physics2D.Raycast(
            new Vector2(
                transform.position.x,
                -0.3f
            ),
            new Vector2(
                mRb.velocity.x < 0f ? -1 : 1,
                0f
            ).normalized,
            rayDistanceToAttack,
            LayerMask.GetMask("Player")
            );
        
        if (hit2) 
        {
            Debug.DrawRay(
                new Vector2(
                transform.position.x,
                -0.3f
            ),
                new Vector2(
                    mRb.velocity.x < 0f ? -1 : 1,
                    0f
                ).normalized * rayDistanceToAttack,
                Color.red
            );
            return true;
        }
        else 
        {
            Debug.DrawRay(
                new Vector2(
                transform.position.x,
                -0.3f
            ),
                new Vector2(
                    mRb.velocity.x < 0f ? -1 : 1,
                    0f
                ).normalized * rayDistanceToAttack,
                Color.green
            );
            return false;
        }
    }
    private bool AcercarseAlJugador()
    {
        //Raycast que solo detecte al jugador de lejos
        var hit1 = Physics2D.Raycast(
            new Vector2(
                transform.position.x + (transform.localScale.x < 0f ? 3.7f:-3.7f),
                -0.4f
            ),
            new Vector2(
                mRb.velocity.x < 0f ? -1 : 1,
                0f
            ).normalized,
            rayDistanceFromPlayer,
            LayerMask.GetMask("Player")
        );

        /*Dibujar el raycast
        if (hit1) 
        {
            Debug.DrawRay(
                new Vector2(
                transform.position.x + (transform.localScale.x < 0f ? 3.7f:-3.7f),
                -0.4f
            ),
                new Vector2(
                    mRb.velocity.x < 0f ? -1 : 1,
                    0f
                ).normalized * rayDistanceFromPlayer,
                Color.red
            );
            return true;
        }
        else 
        {
            Debug.DrawRay(
                new Vector2(
                transform.position.x + (transform.localScale.x < 0f ? 3.7f:-3.7f),
                -0.4f
            ),
                new Vector2(
                    mRb.velocity.x < 0f ? -1 : 1,
                    0f
                ).normalized * rayDistanceFromPlayer,
                Color.green
            );
            return false;
        }*/
        if (hit1) return true;
        else return false;
    }

    private void ControlarIsRunning(bool indicador)
    {
        //Cuando BuscarPlayer es verdadero activamos el flag isRunning
        if (indicador)
        {
            isRunning = true;
            isStartingToRun = true;
            mAnimator.SetBool("IsRunning", true);
            mAnimator.SetBool("IsStartingToRun", true);
            mRb.velocity = new Vector2(runSpeed, mRb.velocity.y);
        }
        else
        {
            isRunning = false;
            isStartingToRun = false;
            mAnimator.SetBool("IsStartingToRun", false); 
            mAnimator.SetBool("IsRunning", false);
            mRb.velocity = new Vector2(speed, mRb.velocity.y);
        }
        
    }
    private void ControlarAtaque(bool indicador)
    {
        // Cuando AtacarAlJugador es verdadero activamos el flag isAttacking
        if (indicador)
        {

            isAttacking2 = false;
            isAttacking1 = true;
            mAnimator.SetBool("IsAttacking2",false);
            mAnimator.SetBool("IsAttacking1",true);
            
            
        }
    }
}
