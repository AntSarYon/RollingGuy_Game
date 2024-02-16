using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class RecordManager : MonoBehaviour
{
    public static RecordManager Instance;

    [HideInInspector] public int minutesRecord;
    [HideInInspector] public float secondsRecord;
    [HideInInspector] public int pointsRecord;

    public bool timeRecordUpdated;
    public bool pointsRecordUpdated;

    //-----------------------------------------------------

    void Awake()
    {
        ControlarUnicaInstancia();

        minutesRecord = 4;
        secondsRecord = 30;
        pointsRecord = 0;

        //Iniciamos desactivando los Flags de RecordsActualizados
        timeRecordUpdated = false;
        pointsRecordUpdated = false;
    }

    //--------------------------------------------------------------------------

    void Start()
    {
        //Asignamos delegados a Eventos
        SceneManager.sceneLoaded += SceneLoadedDelegate;
        GameManager.Instance.OnVictoryAchieved += OnVicoryAchievedDelegate;
    }

    //---------------------------------------------------------------------------

    private void SceneLoadedDelegate(Scene newScene, LoadSceneMode arg1)
    {
        //Si la escena cargada es el Level1
        if (newScene.name == "Level1")
        {
            //Asignamos el Delegado del Evento de Victoria del GameManager
            GameManager.Instance.OnVictoryAchieved += OnVicoryAchievedDelegate;
        }
        
    }

    //---------------------------------------------------------------------------

    private void OnVicoryAchievedDelegate()
    {
        //Si los minutos transcurridos son menores a los minutos del Record
        if (Timer.Instance.minutes < minutesRecord)
        {
            minutesRecord = Timer.Instance.minutes;
            secondsRecord = Timer.Instance.timer;

            //Activamos el Flag de TiempoRecord Actualizado
            timeRecordUpdated = true;
        }
        //Si los minutos transcurridos son iguales a los minutos del Record, pero los segundos son menores al Record...
        else if (Timer.Instance.minutes == minutesRecord && Timer.Instance.timer < secondsRecord)
        {
            minutesRecord = Timer.Instance.minutes;
            secondsRecord = Timer.Instance.timer;

            //Activamos el Flag de TiempoRecord Actualizado
            timeRecordUpdated = true;
        }

        if (PointsManager.Instance.actualPoints > pointsRecord)
        {
            //Actualizamos los Puntos Record
            pointsRecord = PointsManager.Instance.actualPoints;

            //Activamos el Flag de PuntosRecordActualizado
            pointsRecordUpdated = true;
        }

    }

    //-----------------------------------------------------

    private void ControlarUnicaInstancia()
    {
        if (Instance!=null)
        {
            Destroy(this.gameObject);
        }
        else
        {
            Instance = this;
            DontDestroyOnLoad(this.gameObject);
        }
    }

    //-----------------------------------------------------
}
