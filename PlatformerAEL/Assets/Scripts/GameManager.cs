using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    //Instancia estática
    public static GameManager Instance;

    //Coordenadas del último Checkpoint
    private Vector3 ultimoCheckpoint;

    //Creamos un Manejador de Eventos para los Eventos de Daño.
    public event EventHandler OnPlayerDamage;
    public event EventHandler OnEnemyDamage;

    private Transform player;
    private Vector3 coorLeftLimit;
    private Vector3 coorRightLimit;

    private float damageReceivedInProgress;

    // GETTER Y SETTER
    public Vector3 UltimoCheckpoint { get => ultimoCheckpoint; set => ultimoCheckpoint = value; }
    public float DamageReceivedInProgress { get => damageReceivedInProgress; set => damageReceivedInProgress = value; }

    //------------------------------------------------------

    void Awake()
    {
        //Controlamos la única isntancia del GameManager a lo largo del juego
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(this.gameObject);
        }
        else
        {
            DestroyObject(this.gameObject);
        }

    }

    //------------------------------------------------------

    void Start()
    {
        //Obtenemos Referencia al Player
        player = GameObject.Find("Player").transform;
        coorRightLimit = GameObject.Find("MapLimits").transform.Find("RightLimitCoor").transform.position;
        coorLeftLimit = GameObject.Find("MapLimits").transform.Find("LeftLimitCoor").transform.position;

        //Seteamos el primer Checkpoint como la posición de inicio del jugador.
        UltimoCheckpoint = player.position;
    }

    //-------------------------------------------------------------

    private void Update()
    {
        LimitarMovimientoHorizontal();
    }

    //-------------------------------------------------------------

    public void PlayerDamage()
    {
        //Si se ejecuta el Evento, este objeto disparará una observación, con argumentos vacios
        OnPlayerDamage?.Invoke(this, EventArgs.Empty);
    }

    //-------------------------------------------------------------

    public void EnemyDamage()
    {
        //Si se ejecuta el Evento, este objeto disparará la observación, con arhumentos vacios
        OnEnemyDamage?.Invoke(this, EventArgs.Empty);
    }

    //-------------------------------------------------------------

    public void EvaluarYActualizarCheckpoint(Vector3 nuevoCheckpoint)
    {
        //Si este Checkpoint está más adelante que el anteriormente guardado
        if (nuevoCheckpoint.x > UltimoCheckpoint.x)
        {
            //Hacemos que el GameManager registre este Checkpoint como el último.
            UltimoCheckpoint = nuevoCheckpoint;
        }
    }

    //-------------------------------------------------------------

    public void LimitarMovimientoHorizontal()
    {

        print($"eL LIMTE IZQUIERDO ES DE:{coorLeftLimit.x}, mientras que el LIMITE DERECHO ES DE: {coorRightLimit.x}");

        //Aplicar un Mathf.Clamp en el Eje X
        player.position = new Vector3(
            Mathf.Clamp(player.position.x, coorLeftLimit.x, coorRightLimit.x),
            player.position.y,
            player.position.z
            );

        //Utilizar las coordenadas (en X) de los limites como valores 
    }
}
