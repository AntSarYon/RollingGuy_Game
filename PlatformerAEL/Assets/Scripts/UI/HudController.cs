using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class HudController : MonoBehaviour
{
    public static HudController Instance;

    private Animator mAnimator;

    [Header("TXTs de Timer")]
    [SerializeField] private TextMeshProUGUI txtTimer1;
    [SerializeField] private TextMeshProUGUI txtTimer2;

    [Header("TXTs de Record de Tiempo")]
    [SerializeField] private TextMeshProUGUI txtRecord1;
    [SerializeField] private TextMeshProUGUI txtRecord2;

    [Header("TXTs de Puntos")]
    [SerializeField] private TextMeshProUGUI txtPoints1;
    [SerializeField] private TextMeshProUGUI txtPoints2;

    [Header("TXTs de Record de Puntos")]
    [SerializeField] private TextMeshProUGUI txtRecordPoints1;
    [SerializeField] private TextMeshProUGUI txtRecordPoints2;

    [Header("UI Menu de Victoria")]
    [SerializeField] private GameObject VictoryMenu;

    //------------------------------------------------

    void Awake()
    {
        //Asignamos Instancia
        Instance = this;
        //Obtenemos Referencia al Animator
        mAnimator = GetComponent<Animator>();
    }

    //--------------------------------------------------------------------------------------

    void Start()
    {
        //Lo hacmeos Delegado del Evento de Victoria
        GameManager.Instance.OnVictoryAchieved += OnVicoryAchievedDelegate;

        //Ocultamos el Panel de Victoria
        VictoryMenu.SetActive(false);

        //Acualizamos al inicio de la Partida EL TXT de tiempo Record
        SetRecord();

        //Actualizamos el Puntaje Record;
        SetRecordPoints();

        //Actualizamos la musica al Blank
        AudioManager.instance.updateMusic("BackgroundMusic");

    }

    //--------------------------------------------------------------------------------------

    private void OnVicoryAchievedDelegate()
    {
        //Mostramos el Panel de Victoria
        VictoryMenu.SetActive(true);
    }

    //--------------------------------------------------------------------------------------

    void Update()
    {
        //Acualizamos constantemente los TXT de tiempo
        UpdateTimer();

        //Acualizamos constantemente los Puntos
        UpdatePoints();
    }

    //--------------------------------------------------------------------------------------

    private void UpdateTimer()
    {
        if (Timer.Instance.timer < 10)
        {
            txtTimer1.text = $"{Timer.Instance.minutes}:0{(int)Timer.Instance.timer}";
            txtTimer2.text = $"{Timer.Instance.minutes}:0{(int)Timer.Instance.timer}";
        }
        else
        {
            txtTimer1.text = $"{Timer.Instance.minutes}:{(int)Timer.Instance.timer}";
            txtTimer2.text = $"{Timer.Instance.minutes}:{(int)Timer.Instance.timer}";
        }
    }


    private void SetRecord()
    {
        
        if (RecordManager.Instance.secondsRecord < 10)
        {
            txtRecord1.text = $"{RecordManager.Instance.minutesRecord}:0{(int)RecordManager.Instance.secondsRecord}";
            txtRecord2.text = $"{RecordManager.Instance.minutesRecord}:0{(int)RecordManager.Instance.secondsRecord}";
        }
        else
        {
            txtRecord1.text = $"{RecordManager.Instance.minutesRecord}:{(int)RecordManager.Instance.secondsRecord}";
            txtRecord2.text = $"{RecordManager.Instance.minutesRecord}:{(int)RecordManager.Instance.secondsRecord}";
        }
    }

    //--------------------------------------------------------------------------------------

    public void UpdatePoints()
    {
        txtPoints1.text = PointsManager.Instance.actualPoints.ToString();
        txtPoints2.text = PointsManager.Instance.actualPoints.ToString();
    }

    public void SetRecordPoints()
    {
        txtRecordPoints1.text = RecordManager.Instance.pointsRecord.ToString() + " points";
        txtRecordPoints2.text = RecordManager.Instance.pointsRecord.ToString() + " points";
    }

    //--------------------------------------------------------------------------------------

    public void PlayFadeIn()
    {
        mAnimator.Play("FadeIn");
    }

    public void PlayFadeOut()
    {
        mAnimator.Play("FadeOut");
    }

    public void PlayVictoryFadeIn()
    {
        mAnimator.Play("FadeInVictory");
    }

    //-------------------------------------------------

    public void Call_Resurrection()
    {
        GameManager.Instance.PlayerBeingResurrected();
    }
}
