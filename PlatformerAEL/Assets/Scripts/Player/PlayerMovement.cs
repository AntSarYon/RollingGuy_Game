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

    public static PlayerMovement Instance;

    //Variables de Velocidad
    [Header("Velocidad de Movimiento")]
    [SerializeField]
    private float runSpeed = 8.5f;
    //- - - - - - - 
    [Header("Velocidad de Salto")]
    [SerializeField]
    private float jumpSpeed = 25f;
    private float secondJumpSpeed = 20f;
    //- - - - - - - 
    [Header("Velocidad de Retroceso")]
    [SerializeField]
    private float BackSpeed = 5f;
    private float BackInAirSpeed = 10f;

    //Vector Input de movimiento
    private Vector2 mMoveInput;

    //Referencias a componentes
    private Rigidbody2D mRb;
    private Animator mAnimator;
    private CapsuleCollider2D mCollider;
    private AudioSource mAudioSource;

    //Clips de Audios para acciones del personaje
    [Header("Sonidos de Salto")]
    [SerializeField]
    private AudioClip[] saltos = new AudioClip[2];
    //- - - - - - - 
    [Header("Sonido de Ataque")]
    [SerializeField]
    private AudioClip clipAtaque;
    //- - - - - - - 
    [Header("Sonido al hacerse Daño")]
    [SerializeField]
    private AudioClip clipImpacto;

    //Capas con las cuales interactuamos
    [Header("Capa de las plataformas")]
    [SerializeField]
    private LayerMask capaTerreno;

    //Capas con las cuales interactuamos
    [Header("Capa de los Enemigos")]
    [SerializeField]
    private LayerMask capaEnemigos;

    //FLAGS de ESTADOS
    private bool isAlive;

    private bool enPared;
    private bool canDoubleJump;
    private bool isAttacking;
    private bool isBeingDamage;
    private bool isImpactingEnemy;

    //Componentes del ataque.
    private float attackMoveMultiplier;
    private float attackDamage;

    //Controlamos el tiempo que emplea para atacar
    private bool takeAttackTime;
    private float actualAttackTime;
    private float maxAttackTime;

    //Controlamos el tiempo que emplea para Retroceder por Daño
    private bool takeHitTime;
    private float actualHitTime;
    private float maxHitTime;

    //Controlamos el tiempo que emplea para Retroceder por Impacto de Ataque
    private bool takeAttackImpactTime;
    private float actualAttackImpactTime;
    private float maxAttackImpactTime;

    //Última dirección en que se estaba moviendo
    private float ultimaDirección;


    // GETTERS y SETTERS
    public bool IsAttacking { get => isAttacking; }
    public float AttackDamage { get => attackDamage; }
    public Animator MAnimator { get => mAnimator; }
    public AudioSource MAudioSource { get => mAudioSource; set => mAudioSource = value; }
    public bool IsAlive { get => isAlive; set => isAlive = value; }
    public Rigidbody2D MRb { get => mRb; set => mRb = value; }

    //-----------------------------------------------------------

    private void Awake()
    {
        Instance = this;

        //Obtenemos componentes
        mRb = GetComponent<Rigidbody2D>();
        mAnimator = GetComponent<Animator>();
        mCollider = GetComponent<CapsuleCollider2D>();
        mAudioSource = GetComponent<AudioSource>();

        //Obtenemos referencia al GameManager
        gameManager = GameManager.Instance;

        //Inicializamos

        IsAlive = true;

        ResetFlags();

        attackMoveMultiplier = 1f;
        attackDamage = 10;

        actualAttackTime = 0.0f;
        maxAttackTime = 0.45f;

        actualHitTime = 0.0f;
        maxHitTime = 0.5f;

        actualAttackImpactTime = 0.0f;
        maxAttackImpactTime = 0.35f;
    }

    //--------------------------------------------------------------

    private void ResetFlags()
    {
        //Inicializamos
        isAlive = true;
        canDoubleJump = false;
        enPared = false;

        isBeingDamage = false;
        isAttacking = false;
        isImpactingEnemy = false;

        takeAttackTime = false;
        takeHitTime = false;
        takeAttackImpactTime = false;
    }

    private void ResetAnimationFlags()
    {
        mAnimator.SetBool("IsDoubleJumping", false);
        mAnimator.SetBool("IsJumping", false);
        mAnimator.SetBool("IsRunning", false);
        mAnimator.SetBool("IsFalling", false);
        mAnimator.SetBool("IsAttacking", false);
        mAnimator.SetBool("WallNear", false);
        mAnimator.SetBool("IsBeingHit", false);
        mAnimator.SetBool("IsTeleporting", false);
    }

    //--------------------------------------------------------------

    void Start()
    {
        //Añadimos como observador de los Eventos a este Script.
        gameManager.OnPlayerDamage += OnPlayerDamageDelegate;
        gameManager.OnEnemyDamage += OnEnemyDamageDelegate;
        gameManager.OnPlayerDeath += OnPlayerDeathDelegate;
        gameManager.OnPlayerBeingResurrected += OnPlayerBeingResurrectedDelegate;
    }

    //--------------------------------------------------------------

    #region DELEGADOS DE EVENTOS 
    private void OnPlayerDeathDelegate()
    {
        
    }

    //--------------------------------------------------------------

    private void OnPlayerBeingResurrectedDelegate()
    {
        //Teletransortamos al JUGADOR a las coordenadas del ultimo CheckPoint.
        transform.position = GameManager.Instance.UltimoCheckpoint;

        //Reactivamos el Flag de está vivo
        IsAlive = true;

        //Reiniciamos los Flags
        ResetFlags();
        ResetAnimationFlags();

        //Desactivamos la colision del Player
        attackMoveMultiplier = 1.0f;
        mCollider.enabled = true;
        mRb.isKinematic = false;
        mRb.gravityScale = 8;

        mRb.velocity = Vector2.zero;
    }

    //------------------------------------------------------------
    // Comportamiento cuando un Enemigo sufra daño
    private void OnEnemyDamageDelegate()
    {
        //Obtenemos la dirección en que nos dirigiamos al golpear al enemigo
        ultimaDirección = MathF.Sign(mRb.velocity.x);
    }

    //------------------------------------------------------------
    // Comportamiento cuando YO sufra daño
    private void OnPlayerDamageDelegate()
    {
        //Controlamos que el sonido solo de dispare 1 vez
        if (!isBeingDamage)
        {
            mAudioSource.PlayOneShot(clipImpacto, 0.60f); 
        }

        //Activamos el Flag de recibiendo daño
        isBeingDamage = true;

        //Obtenemos la ultima direccion a la cual nos dirigiamos antes de impactar
        ultimaDirección = MathF.Sign(mRb.velocity.x);

        //Modificamos el Flag para indicar que debemos tomar el tiempo desde ahora 
        takeHitTime = true;
        
    }

    #endregion

    //------------------------------------------------------------
    //Funcion: Invocar al Evento de Muerte de Jugador
    public void Call_PlayerDeath()
    {
        GameManager.Instance.PlayerDeath();
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

    //--------------------------------------------------------------
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

    //--------------------------------------------------------------

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

    //--------------------------------------------------------------

    private void ControlarSaltosDePared()
    {
        //Obtenemos una posicion referencial sobre el centro de nuestro personaje
        Vector3 posicionReferencia1 = new Vector3(transform.position.x, transform.position.y-0.30f, transform.position.z);
        Vector3 posicionReferencia2 = new Vector3(transform.position.x, transform.position.y - 0.50f, transform.position.z);
        Vector3 posicionReferencia3 = new Vector3(transform.position.x, transform.position.y, transform.position.z);

        //Creamos 2 raycast hacia ambos lados para comprobar si hay una plataforma (muro) cerca
        RaycastHit2D rcLeft1 = Physics2D.Raycast(posicionReferencia1, Vector2.left, 0.53f, capaTerreno);
        RaycastHit2D rcRight1 = Physics2D.Raycast(posicionReferencia1, Vector2.right, 0.53f, capaTerreno);

        RaycastHit2D rcLeft2 = Physics2D.Raycast(posicionReferencia2, Vector2.left, 0.53f, capaTerreno);
        RaycastHit2D rcRight2 = Physics2D.Raycast(posicionReferencia2, Vector2.right, 0.53f, capaTerreno);

        RaycastHit2D rcLeft3 = Physics2D.Raycast(posicionReferencia3, Vector2.left, 0.53f, capaTerreno);
        RaycastHit2D rcRight3 = Physics2D.Raycast(posicionReferencia3, Vector2.right, 0.53f, capaTerreno);

        //Si los raycast detectan terreno al cual aferrarse
        if (rcLeft1 || rcRight1 || rcLeft2 || rcRight2 || rcLeft3 || rcRight3)
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

    //--------------------------------------------------------------
    private void ControlarAtaque()
    {
        //Si oprime Ctrl, y no se esta tomando el tiempo de ataque
        if (Input.GetKeyDown(KeyCode.C) && takeAttackTime == false)
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
            //Le agregamos el DeltaTime al tiempo de Ataqueactual;
            actualAttackTime += Time.deltaTime;
            
            //Si fuera necesario, controlamos el Retroceso Por Impacto de Ataque
            //ControlarTiempoDeRetrocesoPorImpactoDeAtaque();

            //Si no hemos Impactado nada, el tiempo de ataque actual excede al Máximo.
            if (actualAttackTime >= maxAttackTime)// takeAttackTime &&)
            {
                //Reiniciamos todos los valores y Flag
                attackMoveMultiplier = 1f;
                mAnimator.SetBool("IsAttacking", false);

                //Desactivamo el Flag para calcular el tiempo
                takeAttackTime = false;

                //Desactivamos el Flag de AtaqueEnProceso
                isAttacking = false;

                //Devolvemos el tiempo de ataque actual a 0
                actualAttackTime = 0f;
            }
        }
    }

    //--------------------------------------------------------------

    private void ControlarTiempoDeRetrocesoPorDaño()
    {
        //Si debemos calcular el tiempo desde el golpe
        if (takeHitTime)
        {
            //Le vamos agregamos el DeltaTime;
            actualHitTime += Time.deltaTime;

            //Si el tiempo de ataque actual excede al medio segundo.
            if (actualHitTime >= maxHitTime)
            {
                // - - - -  - - - - -- N U E V O - - - -- -  - -- - - -  -- - - - --
                //Si su vida llegó a 0 (Murió)
                if (!IsAlive)
                {
                    //Disparamos el Trigger de la Animacion de Muerte
                    MAnimator.SetTrigger("Death");

                    AudioManager.instance.PlaySfx("Damage2");

                    // Dentro de la Animacion se invoca una Funcion que
                    // hará el resto del Trabajo...

                    //Hacemos que ya no tome el tiempo de retroceso
                    takeHitTime = false;
                    isBeingDamage = false;

                    //Devolvemos el tiempo de Retroceso actual a 0
                    actualHitTime = 0f;

                    //Desactivamos la colision del Player y su RigidBody
                    mCollider.enabled = false;
                    mRb.velocity = Vector2.zero;
                    mRb.isKinematic = true;
                    mRb.gravityScale = 0;
                }

                // - - - -  - - - - -- N U E V O - - - -- -  - -- - - -  -- - - - --

                //En caso su visa no haya llegado a 0
                else
                {
                    //Reiniciamos todos los valores y Flags
                    mAnimator.SetBool("IsBeingHit", false);
                    takeHitTime = false;
                    isBeingDamage = false;

                    //Devolvemos el tiempo de Retroceso actual a 0
                    actualHitTime = 0f;
                }
                
            }
        }
    }

    //--------------------------------------------------------------

    private void ControlarTiempoDeRetrocesoPorImpactoDeAtaque()
    {
        //Si el Flag para calcular el tiempo de Impacto (retroceso) esta activo
        if (takeAttackImpactTime)
        {
            //Le agregamos el DeltaTime al tiempo de Impacto actual ;
            actualAttackImpactTime += Time.deltaTime;

            //Si el tiempo de ataque actual excede al medio segundo.
            if (actualAttackImpactTime >= maxAttackImpactTime)
            {
                //Reiniciamos todos los valores y Flag
                mAnimator.SetBool("IsAttacking", false);
                takeAttackImpactTime = false;
                isAttacking = false;
                isImpactingEnemy = false;

                //Tambien reiniciamos los valores y Flag del Controlador del Ataque
                attackMoveMultiplier = 1f;
                takeAttackTime = false;
                isAttacking = false;
                actualAttackImpactTime = 0f;
            }
        }
    }

    //------------------------------------------------------------

    void Update()
    {
        //Le desactivamos las colisiones
        mCollider.enabled = true;

        //Lo convertimos en Kinemático
        mRb.isKinematic = false;

        //Si el jugador está Vivo... NO esta siendo atacado, y aun no ganamos
        if (isAlive && !isBeingDamage && !isImpactingEnemy && !VictoryCheck.Instance.Victory)
        {
                //Ejectamos sus funciones de movimiento con normalidad

                ControlarAtaque();

                ActualizarVelocidadEnX();

                ControlarMovimientoHorizontal();

                ControlarSaltosDePared();

                ControlarMovimientoVertical();
        }

            //En caso de que si esté siendo atacado
            else if (isBeingDamage)
            {
                //Desactivamos todas sus animaciones, y activamos la de HIT
                mAnimator.SetBool("IsDoubleJumping", false);
                mAnimator.SetBool("IsJumping", false);
                mAnimator.SetBool("IsRunning", false);
                mAnimator.SetBool("IsFalling", false);
                mAnimator.SetBool("IsAttacking", false);
                mAnimator.SetBool("WallNear", false);
                mAnimator.SetBool("IsBeingHit", true);

                if (HayEnemigoDebajo())
                {
                    //Actualizamos la velocidad en base al retroceso
                    mRb.velocity = new Vector2(
                        ultimaDirección * -1f * BackSpeed,
                        BackInAirSpeed
                    );
                }
                else
                {
                    //Actualizamos la velocidad en base al retroceso
                    mRb.velocity = new Vector2(
                        ultimaDirección * -1f * BackSpeed,
                        mRb.velocity.y
                    );
                }

                //Controlamos el tiempo de retroceso para no atascarnos
                ControlarTiempoDeRetrocesoPorDaño();
            }
            else if (isImpactingEnemy)
            {
                mRb.velocity = new Vector2(
                    ultimaDirección * -1f * BackSpeed,
                    mRb.velocity.y
                );
                ControlarTiempoDeRetrocesoPorImpactoDeAtaque();
            }
    }

    //----------------------------------------------------
    private void MoveTP(float movX, float movY)
    {
        //Movimeinto de la dirección mediante una Traslación respecto al Mundo
        transform.Translate(
            new Vector3(
                movX,
                movY,
                0f
                ).normalized * runSpeed * Time.deltaTime,
            Space.World
            );
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
                        //es posible saltar otra vez
                        canDoubleJump = true;
                    }
                }

                //Si ya realiz� su primer salto, y puede ejecutar un segundo...
                if (canDoubleJump)
                {
                    ;
                    //Si se encuentra a una distancia considerable del suelo, y no est� cerca de ninguna pared
                    if (HaySueloProximo() == false && enPared == false)
                    {
                        //Le daremos otro salto
                        mRb.velocity = new Vector2(
                            mRb.velocity.x,
                            secondJumpSpeed
                        );

                        //Activamos el FlagDeAnimacion de DobleJumping
                        mAnimator.SetBool("IsDoubleJumping", true);
                        mAudioSource.PlayOneShot(saltos[1], 0.75f);

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
                        //isAttacking = false;

                        //Dejamos el Flag activado para que pueda efectuar un ataque
                        canDoubleJump = true;
                    }
                }
            }
            
    }

    //------------------------------------------------------------

    #region RAYCASTs

    private bool HaySueloProximo()
    {
        //Obtenemos posiciones de referencia para ambas piernas
        Vector3 posicionReferenciaX1 = new Vector3(transform.position.x - 0.35f, transform.position.y - 0.30f, transform.position.z);
        Vector3 posicionReferenciaX2 = new Vector3(transform.position.x + 0.35f, transform.position.y - 0.30f, transform.position.z);

        //Usamos un RayCast para determinar si debajo nuestro hay un suelo cercano
        RaycastHit2D rc1 = Physics2D.Raycast(posicionReferenciaX1, Vector2.down, 0.72f, capaTerreno);
        RaycastHit2D rc2 = Physics2D.Raycast(posicionReferenciaX2, Vector2.down, 0.72f, capaTerreno);

        //Bastar� con que solo haya 1 ray chocando para ser Verdad
        return (rc1 || rc2);
    }

    //-------------------------------------------------------------
    private bool HayEnemigoDebajo()
    {
        //Obtenemos posiciones de referencia para ambas piernas
        Vector3 posicionReferenciaX1 = new Vector3(transform.position.x - 0.35f, transform.position.y - 0.30f, transform.position.z);
        Vector3 posicionReferenciaX2 = new Vector3(transform.position.x + 0.35f, transform.position.y - 0.30f, transform.position.z);

        //Usamos un RayCast para determinar si debajo nuestro hay un suelo cercano
        RaycastHit2D rc1 = Physics2D.Raycast(posicionReferenciaX1, Vector2.down, 0.80f, capaEnemigos);
        RaycastHit2D rc2 = Physics2D.Raycast(posicionReferenciaX2, Vector2.down, 0.80f, capaEnemigos);

        //Bastar� con que solo haya 1 ray chocando para ser Verdad
        return (rc1 || rc2);
    }

    //--------------------------------------------------------------

    private void OnDrawGizmos()
    {
        Vector3 posicionReferenciaX1 = new Vector3(this.transform.position.x-0.35f, transform.position.y - 0.30f, transform.position.z);
        Vector3 posicionReferenciaX2 = new Vector3(this.transform.position.x+0.35f, transform.position.y - 0.30f, transform.position.z);

        Gizmos.DrawRay(posicionReferenciaX1, Vector2.down * 0.75f);
        Gizmos.DrawRay(posicionReferenciaX2, Vector2.down * 0.75f);

        Vector3 posicionReferencia = new Vector3(this.transform.position.x, transform.position.y - 0.30f, transform.position.z);

        Vector3 posicionReferencia2 = new Vector3(transform.position.x, transform.position.y - 0.50f, transform.position.z);
        Vector3 posicionReferencia3 = new Vector3(transform.position.x, transform.position.y, transform.position.z);

        Gizmos.DrawRay(posicionReferencia, Vector2.left * 0.53f);
        Gizmos.DrawRay(posicionReferencia, Vector2.right * 0.53f);
        Gizmos.DrawRay(posicionReferencia2, Vector2.left * 0.53f);
        Gizmos.DrawRay(posicionReferencia2, Vector2.right * 0.53f);
        Gizmos.DrawRay(posicionReferencia3, Vector2.left * 0.53f);
        Gizmos.DrawRay(posicionReferencia3, Vector2.right * 0.53f);
    }

    #endregion

    //--------------------------------------------------------------

    private void OnTriggerEnter2D(Collider2D collision)
    {
            //Si chocamos con un objeto de la capa Checkpoint
            if (mCollider.IsTouchingLayers(LayerMask.GetMask("Checkpoint")))
            {
                //Evalua nuestra posicion actual para ser considerado como nuevo Checkpoint
                gameManager.EvaluarYActualizarCheckpoint(transform.position);
            }

        //Si chocamos con un objeto de la capa Water o Pikes
        if (mCollider.IsTouchingLayers(LayerMask.GetMask("Water")))
        {
            //Disparamos el Trigger de la Animacion de Muerte
            MAnimator.SetTrigger("Death");

            //Reproducimos sonido de Agua
            AudioManager.instance.PlaySfx("FallingWater");

            //Desactivamos el Flag de Vivo
            IsAlive = false;
        }
    }

    //--------------------------------------------------------------

    private void OnCollisionEnter2D(Collision2D collision)
    {
        //Si chocamos con un objeto de la capa Enemigo
        if (mCollider.IsTouchingLayers(LayerMask.GetMask("Enemy")))
        {
            //Si estamos ejecutando un ataque
            if (isAttacking)
            {
                //Activamos el Flag de Impactando Enemigo
                isImpactingEnemy = true;

                //Activamos el flag para empezar a calcular el tiempo de retroceso por impacto de Ataque
                takeAttackImpactTime = true;
            }
        }

        if (mCollider.IsTouchingLayers(LayerMask.GetMask("Pikes")))
        {
            //Disparamos el Trigger de la Animacion de Muerte
            MAnimator.SetTrigger("Death");

            //Reproducimos sonido de Agua
            AudioManager.instance.PlaySfx("FallingPikes");

            //Desactivamos el Flag de Vivo
            IsAlive = false;
        }
    }
}