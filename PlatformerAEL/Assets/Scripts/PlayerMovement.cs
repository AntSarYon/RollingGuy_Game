using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerMovement : MonoBehaviour
{
    [Header("Velocidad de Movimiento")]
    [SerializeField]
    private float runSpeed;

    [Header("Velocidad de Salto")]
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

    [Header("Sonidos de Salto")]
    [SerializeField]
    private AudioClip[] saltos = new AudioClip[2];

    [Header("Capa de las plataformas")]
    [SerializeField]
    private LayerMask capaTerreno;

    private bool enPared;

    private bool canAttack;
    private bool isAttacking;

    //-----------------------------------------------------------

    private void Awake()
    {
        //Obtenemos componentes
        mRb = GetComponent<Rigidbody2D>();
        mAnimator = GetComponent<Animator>();
        mCollider = GetComponent<CapsuleCollider2D>();
        mAudioSource = GetComponent<AudioSource>();

        //Inicializamos
        canAttack = false;
        isAttacking = false;
        enPared = false;
    }

    //------------------------------------------------------------
    private void ActualizarVelocidadEnX()
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
            //Modificamos la Escala, y la Direcci�n del Sprite en base a si es (+) o (-)
            transform.localScale = new Vector3(
                Mathf.Sign(mRb.velocity.x),
                transform.localScale.y,
                transform.localScale.z
            );

            //Activamos el FlagDeAnimaci�n para correr
            mAnimator.SetBool("IsRunning", true);
        }
        //En caso no se est� recibiendo ning�n input de movimiento en X
        else
        {
            //Desactivamos el FlagDeAnimaci�n para correr
            mAnimator.SetBool("IsRunning", false);
        }
    }

    private void ControlarMovimientoVertical()
    {
        //Si estamos cayendo a gran velocidad
        if (mRb.velocity.y < -11.0f)
            {
                //Desactivamos el FlagDeAnimacion del Salto, y activamos el de Caida
                mAnimator.SetBool("IsFalling", true);
                mAnimator.SetBool("IsJumping", false);
                mAnimator.SetBool("IsDoubleJumping", false);
            }

            //Si estamos cayendo, y estamos cerca a una superficie
            if (mRb.velocity.y <= 0.01f && HaySueloProximo()) //Aqui era <= <------------------------------
            {
                //Desactivamos las animaciones de salto
                mAnimator.SetBool("IsJumping", false);
                mAnimator.SetBool("IsDoubleJumping", false);
                mAnimator.SetBool("IsFalling", false);

                //Reseteamos el Flag de CanAttack
                canAttack = false;
            }
    }

    private void ControlarSaltosDePared()
    {
        //Obtenemos una posicion referencial sobre el centro de nuestro personaje
        Vector3 posicionReferencia = new Vector3(transform.position.x, transform.position.y-0.30f, transform.position.z);
        
        //Creamos 2 raycast hacia ambos lados para comprobar si hay una plataforma (muro) cerca
        RaycastHit2D rcLeft = Physics2D.Raycast(posicionReferencia, Vector2.left, 0.53f, capaTerreno);
        RaycastHit2D rcRight = Physics2D.Raycast(posicionReferencia, Vector2.right, 0.53f, capaTerreno);

        //Si los raycast detectan terreno al cual aferrarse
        if (rcLeft || rcRight)
        {
            //Activamos los Flag de Pared Pr�xima
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
        ActualizarVelocidadEnX();

        ControlarMovimientoHorizontal();

        ControlarSaltosDePared();

        ControlarMovimientoVertical();

    }

    //---------------------------------------------------------------------
    //--- Funci�n OnMove; activada al detectarse Inputs para el eje X
    //---------------------------------------------------------------------
    private void OnMove(InputValue value)
    {
        //Almacenamos el Vector con la unidad de movimiento en X
        mMoveInput = value.Get<Vector2>();

    }

    //---------------------------------------------------------------------
    //--- Funci�n OnJump; activada al oprimir el boton de Salto
    //---------------------------------------------------------------------
    private void OnJump(InputValue value)
    {
        //Si se ha oprimido el Bot�n
        if (value.isPressed)
        {
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

            //Si ya realiz� su primer salto, y puede ejecutar un segundo...
            if (canAttack)
            {;
                //Si se encuentra a una distancia considerable del suelo, y no est� cerca de ninguna pared
                if (HaySueloProximo()== false && enPared==false)
                {
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

                //Si est� cerca de una pared, sea cual sea su altura
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
        //Obtenemos posiciones de referencia para ambas piernas
        Vector3 posicionReferenciaX1 = new Vector3(transform.position.x - 0.35f, transform.position.y - 0.30f, transform.position.z);
        Vector3 posicionReferenciaX2 = new Vector3(transform.position.x + 0.35f, transform.position.y - 0.30f, transform.position.z);

        //Usamos un RayCast para determinar si debajo nuestro hay un suelo cercano
        RaycastHit2D rc1 = Physics2D.Raycast(posicionReferenciaX1, Vector2.down, 0.71f, capaTerreno);
        RaycastHit2D rc2 = Physics2D.Raycast(posicionReferenciaX2, Vector2.down, 0.71f, capaTerreno);

        //Bastar� con que solo haya 1 ray chocando para ser Verdad
        return (rc1 || rc2);
    }

    private void OnDrawGizmos()
    {
        Vector3 posicionReferenciaX1 = new Vector3(transform.position.x-0.35f, transform.position.y - 0.30f, transform.position.z);
        Vector3 posicionReferenciaX2 = new Vector3(transform.position.x+0.35f, transform.position.y - 0.30f, transform.position.z);

        Gizmos.DrawRay(posicionReferenciaX1, Vector2.down * 0.71f);
        Gizmos.DrawRay(posicionReferenciaX2, Vector2.down * 0.71f);

        Vector3 posicionReferencia = new Vector3(transform.position.x, transform.position.y - 0.30f, transform.position.z);
        Gizmos.DrawRay(posicionReferencia, Vector2.left * 0.53f);
        Gizmos.DrawRay(posicionReferencia, Vector2.right * 0.53f);
    }
}