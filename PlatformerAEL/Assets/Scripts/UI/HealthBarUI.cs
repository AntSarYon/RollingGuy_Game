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
        GameManager.Instance.OnEnemyDamage += OnPlayerDamageDelegate;
        
    }

    //-------------------------------------------------------------------------------------------

    //Esto permitir� que todos los enemigos afectados por el Evento puedan llamar a esta ejecuci�n
    private void OnPlayerDamageDelegate(object sender, EventArgs e)
    {
        print("aqui llego");
        //Reduzco el Valor del Slider de Vida en 5.
        mSlider.value -= 1f;
    }
}