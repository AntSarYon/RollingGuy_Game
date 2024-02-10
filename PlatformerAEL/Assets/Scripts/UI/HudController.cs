using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HudController : MonoBehaviour
{
    public static HudController Instance;

    private Animator mAnimator;

    //------------------------------------------------

    void Awake()
    {
        //Asignamos Instancia
        Instance = this;
        //Obtenemos Referencia al Animator
        mAnimator = GetComponent<Animator>();
    }

    //-------------------------------------------------

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
