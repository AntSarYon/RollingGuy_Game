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

    [Header("TXTs de Record")]
    [SerializeField] private TextMeshProUGUI txtRecord1;
    [SerializeField] private TextMeshProUGUI txtRecord2;

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
        //Acualizamos al inicio de la Partida EL TXT de tiempo Record
        SetRecord();
    }

    //--------------------------------------------------------------------------------------

    void Update()
    {
        //Acualizamos constantemente los TXT de tiempo
        UpdateTimer();
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

    //--------------------------------------------------------------------------------------

    private void SetRecord()
    {
        
        if (Timer.Instance.recordSeconds < 10)
        {
            txtRecord1.text = $"{Timer.Instance.recordMinutes}:0{(int)Timer.Instance.recordSeconds}";
            txtRecord2.text = $"{Timer.Instance.recordMinutes}:0{(int)Timer.Instance.recordSeconds}";
        }
        else
        {
            txtRecord1.text = $"{Timer.Instance.recordMinutes}:{(int)Timer.Instance.recordSeconds}";
            txtRecord2.text = $"{Timer.Instance.recordMinutes}:{(int)Timer.Instance.recordSeconds}";
        }
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

    //-------------------------------------------------

    public void Call_Resurrection()
    {
        GameManager.Instance.PlayerBeingResurrected();
    }
}
