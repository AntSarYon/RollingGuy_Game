using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UIElements;

public class PlayerMovement : MonoBehaviour
{
    //Referencia al GameManager
    private GameManager gameManager;

    [Header("Velocidad de Movimiento")]
    [SerializeField]
    private float runSpeed = 7f;

    [Header("Velocidad de Salto")]
    [SerializeField]
    private float jumpSpeed = 10f;
    private float secondJumpSpeed = 20f;

    [Header("Velocidad de Retroceso")]
    [SerializeField]
    private float BackSpeed = 12f;

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

    [Header("Sonido de Ataque")]
    [SerializeField]
    private AudioClip clipAtaque;

    [Header("Sonido de Ataque")]
    [SerializeField]
    private AudioClip clipImpacto;

    [Header("Capa de las plataformas")]
    [SerializeField]
    private LayerMask capaTerreno;

    private bool enPared;

    private bool canDoubleJump;

    //Flag cuando está atacando
    private bool isAttacking;

    //Componentes del ataque.
    private float attackMoveMultiplier;
    private float attackDamage;

    //Flag cuando esta siendo atacado
    private bool isBeingDamage;

    //Controlamos el tiempo que emplea para atacar
    private bool takeAttackTime;
    private float actualAttackTime;
    private float maxAttackTime;

    //Controlamos el tiempo que emplea para Retroceder
    private bool takeHitTime;
    private float actualHitTime;
    private float maxHitTime;

    //contenedor de su ultima dirección
    private float ultimaDirección;

    public bool IsAttacking { get => isAttacking; set => isAttacking = value; }
    public float AttackDamage { get => attackDamage; set => attackDamage = value; }

    //-----------------------------------------------------------

    private void Awake()
    {
        //Obtenemos componentes
        mRb = GetComponent<Rigidbody2D>();
        mAnimator = GetComponent<Animator>();
        mCollider = GetComponent<CapsuleCollider2D>();
        mAudioSource = GetComponent<AudioSource>();

        //Obtenemos referencia al GameManager
        gameManager = GameManager.Instance;

        //Inicializamos
        canDoubleJump = false;

        isBeingDamage = false;
        isAttacking = false;

        attackMoveMultiplier = 1f;
        attackDamage = 100f;

        takeAttackTime = false;
        actualAttackTime = 0.0f;
        maxAttackTime = 0.6f;

        takeHitTime = false;
        actualHitTime = 0.0f;
        maxHitTime = 0.5f;

    enPared = false;
    }

    //--------------------------------------------------------------

    private void Start()
    {
        //Añadimos como observador de los Eventos a este Script.
        GameManager.Instance.OnPlayerDamage += OnPlayerDamageDelegate;
        GameManager.Instance.OnEnemyDamage += OnEnemyDamageDelegate;
    }

    //------------------------------------------------------------
    // Comportamiento cuando un Enemigo sufra daño
    private void OnEnemyDamageDelegate(object sender, EventArgs e)
    {
        print("EnemigoDañado");
    }

    //------------------------------------------------------------
    // Comportamiento cuando YO sufra daño
    private void OnPlayerDamageDelegate(object sender, EventArgs e)
    {
        //Controlamos que el sonido solo de dispare 1 vez
        if (isBeingDamage == false)
        {
            mAudioSource.PlayOneShot(clipImpacto, 0.60f); 
        }

        //Obtenemos la ultima direccion a la cual nos dirigiamos antes de impactar
        ultimaDirección = MathF.Sign(mRb.velocity.x);

        //Activamos el Flag de recibiendo daño
        isBeingDamage = true;

        //Modificamos el Flag para indicar que debemos tomar el tiempo desde ahora 
        takeHitTime = true;
        
    }

    //------------------------------------------------------------
    private void ActualizarVelocidadEnX()
    {
        //Actualizamos la velocidad en base a los Inputs de movimiento y ataque
        mRb.velocity = new Vector2(
            mMoveInput.x * runSpeed * attackMoveMultiplier,
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
                canDoubleJump = false;
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

    private void ControlarAtaque()
    {
        //Si oprime Ctrl, y no se esta tomando el tiempo de ataque
        if (Input.GetKeyDown(KeyCode.LeftControl) && takeAttackTime == false)
        {
            //  Modificamos el multiplicador de velocidad a 3
            attackMoveMultiplier = 3f;
            //Activamos la Animacion de "Atacando"
            mAnimator.SetBool("IsAttacking", true);

            //Activamos el Flag de Ataque
            isAttacking = true;

            //Iniciar el Flag para tomar tiempo de ataque
            takeAttackTime = true;

            mAudioSource.PlayOneShot(clipAtaque, 0.75f);

        }
        //Si nos enocntramos tomando tiempo de ataque
        if (takeAttackTime)
        {
            //Le agregamos el DeltaTime el tiempo de actualAttackTime+=Time.deltaTime;
            actualAttackTime += Time.deltaTime;

            //Si el tiempo de ataque actual excede al segundo y medio...
            if (actualAttackTime >= maxAttackTime)
            {
                //Reiniciamos todos los valores y Flag
                attackMoveMultiplier = 1f;
                mAnimator.SetBool("IsAttacking", false);
                takeAttackTime = false;
                isAttacking = false;

                //Devolvemos el tiempo de ataque actual a 0
                actualAttackTime = 0f;
            }
        }
    }

    private void ControlarTiempoDeRetroceso()
    {
        //Si debemos calcular el tiempo desde el golpe
        if (takeHitTime)
        {
            //Le vamos agregamos el DeltaTime;
            actualHitTime += Time.deltaTime;

            //Si el tiempo de ataque actual excede al medio segundo.
            if (actualHitTime >= maxHitTime)
            {
                //Reiniciamos todos los valores y Flag
                mAnimator.SetBool("IsBeingHit", false);
                takeHitTime = false;
                isBeingDamage = false;

                //Devolvemos el tiempo de Retroceso actual a 0
                actualHitTime = 0f;
            }
        }
    }

    //------------------------------------------------------------

    private void Update()
    {
        //Si el jugador NO esta siendo atacado
        if (!isBeingDamage)
        {
            //Ejectamos sus funciones de movimiento con normalidad

            ControlarAtaque();

            ActualizarVelocidadEnX();

            ControlarMovimientoHorizontal();

            ControlarSaltosDePared();

            ControlarMovimientoVertical();
        }
        //En caso de que si esté siendo atacado
        else
        {
            //Desactivamos todas sus animaciones, y activamos la de HIT
            mAnimator.SetBool("IsDoubleJumping", false);
            mAnimator.SetBool("IsJumping", false);
            mAnimator.SetBool("IsRunning", false);
            mAnimator.SetBool("IsFalling", false);
            mAnimator.SetBool("IsAttacking", false);
            mAnimator.SetBool("WallNear", false);
            mAnimator.SetBool("IsBeingHit", true);

            //Actualizamos la velocidad en base al retroceso
            mRb.velocity = new Vector2(
                ultimaDirección * -1f * BackSpeed,
                mRb.velocity.y
            );

            //Controlamos el tiempo de retroceso para no atascarnos
            ControlarTiempoDeRetroceso();
        }
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
            if (canDoubleJump == false)
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
                    canDoubleJump = true;
                }
            }

            //Si ya realiz� su primer salto, y puede ejecutar un segundo...
            if (canDoubleJump)
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
                    canDoubleJump = false;
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
                    canDoubleJump = true;
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

    private void OnTriggerEnter2D(Collider2D collision)
    {
        //Si chocamos con un objeto de la capa Checkpoint
        if (mCollider.IsTouchingLayers(LayerMask.GetMask("Checkpoint")))
        {
            //Evalua nuestra posicion actual para ser considerado como nuevo Checkpoint
            gameManager.EvaluarYActualizarCheckpoint(transform.position);
        }
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        //Si chocamos con un objeto de la capa Water
        if (mCollider.IsTouchingLayers(LayerMask.GetMask("Water")))
        {
            //Nos teletransortamos al nuevo punto de Checkpoint.
            transform.position = gameManager.UltimoCheckpoint;
        }
    }
}