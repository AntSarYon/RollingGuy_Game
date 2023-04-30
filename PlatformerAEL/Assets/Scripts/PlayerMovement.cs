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

    //Vector Input de movimiento
    private Vector2 mMoveInput;

    //Referencias a componentes
    private Rigidbody2D mRb;
    private Animator mAnimator;
    private CapsuleCollider2D mCollider;

    //Capa del terreno a analizar
    [SerializeField]
    private LayerMask capaTerreno;

    private bool canAttack;
    private bool isAttacking;

    //-----------------------------------------------------------

    private void Awake()
    {
        mRb = GetComponent<Rigidbody2D>();
        mAnimator = GetComponent<Animator>();
        mCollider = GetComponent<CapsuleCollider2D>();
    }

    //------------------------------------------------------------

    private void Start()
    {

    }

    //------------------------------------------------------------

    private void Update()
    {
        //Actualizamos la velocidad en base a los Inputs
        mRb.velocity = new Vector2(
            mMoveInput.x * runSpeed,
            mRb.velocity.y
        );

        // - - - - - - - - - - - - - - - - - - - - - - - -

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
        else {
            //Desactivamos el FlagDeAnimación para correr
            mAnimator.SetBool("IsRunning", false);
        }

        // - - - - - - - - - - - - - - - - - - - - - - -

        //Si estamos descendiendo, y no estamos en contacto con el suelo
        if (mRb.velocity.y < 0 && !mCollider.IsTouchingLayers(LayerMask.GetMask("Ground")))
        {
            //Desactivamos el FlagDeAnimacion del Salto, y activamos el de Caida
            mAnimator.SetBool("IsFalling", true);
            mAnimator.SetBool("IsJumping", false);
            mAnimator.SetBool("IsDoubleJumping", false);
            
            /*
            //Si el RayCast detecta que estamos muy cerca del suelo
            if (raycastSuelo == true)
            {
                print("Piso muy cerca");

                //Desactivamos los FlagDeAnimación de Salto y Activamos el de Caida 
                mAnimator.SetBool("IsJumping", false);
                mAnimator.SetBool("IsDoubleJumping", false);
                mAnimator.SetBool("IsFalling", true);
            }*/
        }

        // - - - - - - - - - - - - - - - - - - - - - - -

        //Si el Collider colisiona con una plataforma
        if (mCollider.IsTouchingLayers(LayerMask.GetMask("Ground")))
        {
            // Toco el suelo, y desactivo el FlagDeAnimacion de Caida
            mAnimator.SetBool("IsFalling", false);

        }
        
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
            // Si estamos tocando el suelo...
            if (mCollider.IsTouchingLayers(LayerMask.GetMask("Ground")))
            {
                // Saltamos (agregamos velocidad en Y)
                mRb.velocity = new Vector2(
                    mRb.velocity.x,
                    jumpSpeed
                );

                //Activamos el FlagDeAnimacion de Jumping
                mAnimator.SetBool("IsJumping", true);

                //Activamos el Flag para indicar que
                //es posible Atacar
                canAttack = true;
            }

            //Si no está tocando el suelo
            else
            {
                // - - - - - - - - - - - - - - - - - - - - - - -
                //RAYCAST hacia el suelo --> Comprobamos si se esta a una distancia corta del suelo

                //Si ya está saltando, y se encuentra a una distancia considerable del suelo
                if (
                    mAnimator.GetBool("IsJumping") 
                    && canAttack 
                    && Physics2D.Raycast(transform.position, Vector2.down, 1.35f, capaTerreno) == false)
                {
                    //Le daremos velocidad de salto
                    mRb.velocity = new Vector2(
                        mRb.velocity.x,
                        jumpSpeed
                    );

                    //Activamos el FlagDeAnimacion de DobleJumping
                    mAnimator.SetBool("IsDoubleJumping", true);

                    //Activamos el Flag de ataque
                    isAttacking = true;

                    //Desactivamos el Flag para que ya no pueda efectuar otro ataque
                    canAttack = false;
                }
            }
        }
    }

    private void OnDrawGizmos()
    {
        Gizmos.DrawRay(transform.position, Vector2.down * 1.35f);
    }
}