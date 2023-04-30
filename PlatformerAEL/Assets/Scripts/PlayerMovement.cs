using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerMovement : MonoBehaviour
{
    //Velocidad de Movimiento
    [SerializeField]
    private float runSpeed = 4f;

    //Velocidad del Salto
    [SerializeField]
    private float jumpSpeed = 10f;
    private float secondJumpSpeed = 20f;

    //Vector Input de movimiento
    private Vector2 mMoveInput;

    //Referencias a componentes
    private Rigidbody2D mRb;
    private Animator mAnimator;
    private CapsuleCollider2D mCollider;
    private AudioSource mAudioSource;

    //Clips de Audio
    [SerializeField]
    private AudioClip[] saltos = new AudioClip[2];

    //Capa del terreno a analizar
    [SerializeField]
    private LayerMask capaTerreno;

    private bool enPared;

    private bool canAttack;
    private bool isAttacking;

    //-----------------------------------------------------------

    private void Awake()
    {
        mRb = GetComponent<Rigidbody2D>();
        mAnimator = GetComponent<Animator>();
        mCollider = GetComponent<CapsuleCollider2D>();
        mAudioSource = GetComponent<AudioSource>();

        canAttack = false;
        isAttacking = false;
        enPared = false;
    }

    //------------------------------------------------------------
    private void ActualizarVelocidades()
    {
        //Actualizamos la velocidad en base a los Inputs
        mRb.velocity = new Vector2(
            mMoveInput.x * runSpeed,
            mRb.velocity.y
        );
    }

    private void ControlarMovimientoHorizontal()
    {
        //Si recibimos algun input de movimeinto en X...
        if (Mathf.Abs(mRb.velocity.x) > Mathf.Epsilon)
        {
            //Modificamos la Escala, y la Dirección del Sprite en base a si es (+) o (-)
            transform.localScale = new Vector3(
                Mathf.Sign(mRb.velocity.x),
                transform.localScale.y,
                transform.localScale.z
            );

            //Activamos el FlagDeAnimación para correr
            mAnimator.SetBool("IsRunning", true);
        }
        //En caso no se esté recibiendo ningún input de movimiento en X
        else
        {
            //Desactivamos el FlagDeAnimación para correr
            mAnimator.SetBool("IsRunning", false);
        }
    }

    private void ControlarMovimientoVertical()
    {
            //Si estamos cayendo a gran velocidad y hay suelo proximo
            if (mRb.velocity.y < -11.0f)
            {
                //Desactivamos el FlagDeAnimacion del Salto, y activamos el de Caida
                mAnimator.SetBool("IsFalling", true);
                mAnimator.SetBool("IsJumping", false);
                mAnimator.SetBool("IsDoubleJumping", false);
            }

            //Si estamos cayendo, y estamos cerca a una superficie
            if (mRb.velocity.y <= 0 && HaySueloProximo())
            {
                //Desactivamos las animaciones de salto
                mAnimator.SetBool("IsJumping", false);
                mAnimator.SetBool("IsDoubleJumping", false);
                mAnimator.SetBool("IsFalling", false);

                //Reseteamos el Flag de CanAttack
                canAttack = false;
            }
        //}
    }

    private void ControlarSaltosDePared()
    {
        Vector3 posicionReferencia = new Vector3(transform.position.x, transform.position.y-0.30f, transform.position.z);
        
        RaycastHit2D rcLeft = Physics2D.Raycast(posicionReferencia, Vector2.left, 0.53f, capaTerreno);
        RaycastHit2D rcRight = Physics2D.Raycast(posicionReferencia, Vector2.right, 0.53f, capaTerreno);

        //Si los raycast detectan terreno al cual aferrarse
        if (rcLeft || rcRight)
        {
            print("Chocando con Pared");

            //Activamos los Flag de Pared Próxima
            enPared = true;
            mAnimator.SetBool("WallNear", true);

            //Desactivamos los FlagDeAnimacion de Salto
            mAnimator.SetBool("IsDoubleJumping", false);
            mAnimator.SetBool("IsJumping", false);
        }

        else
        {
            //Caso contrario, lo desactivamos
            enPared = false;
            mAnimator.SetBool("WallNear", false);
        }
            
    }

    //------------------------------------------------------------

    private void Update()
    {
        ActualizarVelocidades();

        ControlarMovimientoHorizontal();

        ControlarSaltosDePared();

        ControlarMovimientoVertical();

    }

    //---------------------------------------------------------------------
    //--- Función OnMove; activada al desplazarnos a la derecha o izquierda
    //---------------------------------------------------------------------
    private void OnMove(InputValue value)
    {
        //Almacenamos el Vector con la unidad de movimiento en X
        mMoveInput = value.Get<Vector2>();

    }

    //---------------------------------------------------------------------
    //--- Función OnJump; activada al oprimir el boton de Salto
    //---------------------------------------------------------------------
    private void OnJump(InputValue value)
    {
        //Si se ha oprimido el Botón
        if (value.isPressed)
        {
            print("AQUI 1");
            // Si aun no ha saltado, 
            if (canAttack == false)
            {
                //Si se encuentra sobre un suelo, o pegado a una pared
                if (HaySueloProximo() || enPared == true)
                {
                    // Saltamos (agregamos velocidad en Y)
                    mRb.velocity = new Vector2(
                        mRb.velocity.x,
                        jumpSpeed
                    );

                    //Activamos el FlagDeAnimacion de Jumping
                    mAnimator.SetBool("IsJumping", true);
                    mAudioSource.PlayOneShot(saltos[0], 0.75f);

                    //Activamos el Flag para indicar que
                    //es posible Atacar
                    canAttack = true;
                }
            }

            //Si ya realizó su primer salto, y puede ejecutar un segundo...
            if (canAttack)
            {;
                //Si se encuentra a una distancia considerable del suelo, y no está cerca de ninguna pared
                if (HaySueloProximo()== false && enPared==false)
                {
                    print("AQUI 2");
                    //Le daremos otro salto
                    mRb.velocity = new Vector2(
                        mRb.velocity.x,
                        secondJumpSpeed
                    );

                    //Activamos el FlagDeAnimacion de DobleJumping
                    mAnimator.SetBool("IsDoubleJumping", true);
                    mAudioSource.PlayOneShot(saltos[1], 0.75f);

                    //Activamos el Flag para indicar que esta atacando
                    isAttacking = true;

                    //Desactivamos el Flag para que ya no pueda efectuar otro ataque
                    canAttack = false;
                }

                //Si está cerca de una pared, sea cual sea su altura
                else if ((HaySueloProximo() == false && enPared == true) || (HaySueloProximo() && enPared))
                {
                    //Le permitimos hacer un salto normal
                    mRb.velocity = new Vector2(
                        mRb.velocity.x,
                        jumpSpeed
                    );

                    //Activamos el FlagDeAnimacion de Jumping
                    mAnimator.SetBool("IsJumping", true);
                    mAudioSource.PlayOneShot(saltos[0], 0.75f);

                    //Desctivamos el Flag para indicar que No esta atacando
                    isAttacking = false;

                    //Dejamos el Flag activado para que pueda efectuar un ataque
                    canAttack = true;
                }
            }
        }
            
    }

    //------------------------------------------------------------

    private bool HaySueloProximo()
    {
        //Usamos un RayCast para determinar si debajo nuestro hay un suelo cercano
        return Physics2D.Raycast(transform.position, Vector2.down, 1.50f, capaTerreno);
    }

    private void OnDrawGizmos()
    {
        Gizmos.DrawRay(transform.position, Vector2.down * 1.50f);

        Vector3 posicionReferencia = new Vector3(transform.position.x, transform.position.y - 0.30f, transform.position.z);
        Gizmos.DrawRay(posicionReferencia, Vector2.left * 0.53f);
        Gizmos.DrawRay(posicionReferencia, Vector2.right * 0.53f);
    }
}