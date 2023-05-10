using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

//Se definirá como un Observador de los eventos cuando el Perosnaje pierda vitalidad.

public class HealthBarUI : MonoBehaviour
{
    //Referencia al Slider
    private Slider mSlider;

    //--------------------------------------------------------

    void Start()
    {
        mSlider = GetComponent<Slider>();
        
        //Añadimos al TPBarUI como Observador del Evento OnEnemyDamage
        GameManager.Instance.OnPlayerDamage += OnPlayerDamageDelegate;
        
    }

    //-------------------------------------------------------------------------------------------

    //Esto permitirá que todos los enemigos afectados por el Evento puedan llamar a esta ejecución
    private void OnPlayerDamageDelegate(object sender, EventArgs e)
    {
        //Reduzco el Valor del Slider de Vida segun el daño del enemigo
        mSlider.value -= GameManager.Instance.DamageReceivedInProgress;
        print("Recibí 15 de Daño");
    }
}
