using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

//Se definir� como un Observador de los eventos cuando el Perosnaje pierda vitalidad.

public class HealthBarUI : MonoBehaviour
{
    //Referencia al Slider
    private Slider mSlider;

    //--------------------------------------------------------

    void Start()
    {
        mSlider = GetComponent<Slider>();
        
        //A�adimos al TPBarUI como Observador del Evento OnEnemyDamage
        GameManager.Instance.OnPlayerDamage += OnPlayerDamageDelegate;
        
    }

    //-------------------------------------------------------------------------------------------

    //Esto permitir� que todos los enemigos afectados por el Evento puedan llamar a esta ejecuci�n
    private void OnPlayerDamageDelegate(object sender, EventArgs e)
    {
        //Reduzco el Valor del Slider de Vida segun el da�o del enemigo
        mSlider.value -= GameManager.Instance.DamageReceivedInProgress;
        if (mSlider.value <= 0)
        {
            //Nos teletransortamos al nuevo punto de Checkpoint.
            GameManager.Instance.Player.position = GameManager.Instance.UltimoCheckpoint;

            //Devolvemos el Slider al Valor m�ximo
            mSlider.value = mSlider.maxValue;
        }
    }
}
