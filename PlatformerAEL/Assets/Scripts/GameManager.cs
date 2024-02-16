using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class GameManager : MonoBehaviour
{
    //Instancia estática
    public static GameManager Instance;

    //Coordenadas del último Checkpoint
    private Vector3 ultimoCheckpoint;

    //Creamos un Manejador de Eventos para los Eventos de Daño.
    public event UnityAction OnPlayerDamage;
    public event UnityAction OnEnemyDamage;
    public event UnityAction OnPlayerDeath;
    public event UnityAction OnEnemyDeath;
    public event UnityAction OnPlayerBeingResurrected;
    public event UnityAction OnVictoryAchieved;

    //Referencia al Transform del Player
    private PlayerMovement player;

    //Vectorees de Coordenadas para conocer los limites del Mapa
    private Vector3 coorLeftLimit; //(Limite izquierdo)
    private Vector3 coorRightLimit; //(Limite Derecho)

    private float damageReceivedInProgress;

    public bool InPause;

    #region GETTERS Y SETTERS
    public Vector3 UltimoCheckpoint { get => ultimoCheckpoint; set => ultimoCheckpoint = value; }
    public float DamageReceivedInProgress { get => damageReceivedInProgress; set => damageReceivedInProgress = value; }
    public PlayerMovement Player { get => player; set => player = value; }

    #endregion
    //---------------------------------------------------------------------------------------

    void Awake()
    {
        //Declaramos este Script como Instancia
        Instance = this;       
    }

    //---------------------------------------------------------------------------------------------------------

    void Start()
    {
        //Obtenemos Referencia al Script del Player
        Player = GameObject.Find("Player").GetComponent<PlayerMovement>();

        //Iniciamos con el Flag de Pausa en Falso
        InPause = false;

        //Obtenemos Coordenadas del Limite del Mapa
        coorRightLimit = GameObject.Find("RightLimitCoor").transform.position;
        coorLeftLimit = GameObject.Find("LeftLimitCoor").transform.position;

        //Seteamos el primer Checkpoint como la posición de inicio del jugador.
        UltimoCheckpoint = Player.transform.position;
    }

    //-----------------------------------------------------------------------------------

    #region DISPARO DE EVENTOS

    public void PlayerDamage()
    {
        //**Llamamos a los delegados**
        OnPlayerDamage?.Invoke();
    }

    public void EnemyDamage()
    {
        //**Llamamos a los delegados**
        OnEnemyDamage?.Invoke();
    }

    public void PlayerDeath()
    {
        //HUD Inicia el FadeIn
        HudController.Instance.PlayFadeIn();

        //**Llamamos a los delegados**
        OnPlayerDeath?.Invoke();
    }

    public void EnemyDeath()
    {
        //Incrementamos 100 puntos
        PointsManager.Instance.actualPoints += 100;

        //**Llamamos a los delegados**
        OnEnemyDeath?.Invoke();
    }

    public void PlayerBeingResurrected()
    {
        //**Llamamos a los delegados**
        OnPlayerBeingResurrected?.Invoke();
    }

    public void VictoryAchieved()
    {
        //**Llamamos a los delegados**
        OnVictoryAchieved?.Invoke();
    }

    #endregion

    //-------------------------------------------------------------

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape) || Input.GetKeyDown(KeyCode.P))
        {
            if(InPause)
                InPause = false;
            else
                InPause = true;
        }

        if (InPause)
        {
            //Pasamos la escala de tiempo a 0
            Time.timeScale = 0;
        }
        else
        {
            //Pasamos la escala a 1 (normal)
            Time.timeScale = 1;
            LimitarMovimientoHorizontal();
        }
       
    }

    //-------------------------------------------------------------

    public void LimitarMovimientoHorizontal()
    {
        //Aplicar un Mathf.Clamp en el Eje X del Jugador -> Lo condicionamos a permanecer dentro de los limites del Mapa
        Player.transform.position = new Vector3(
            Mathf.Clamp(Player.transform.position.x, coorLeftLimit.x, coorRightLimit.x),
            Player.transform.position.y,
            Player.transform.position.z
            );
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

}
