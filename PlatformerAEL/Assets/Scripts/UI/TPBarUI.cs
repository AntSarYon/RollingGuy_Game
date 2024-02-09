using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

//Se definirá como un Observador de los eventos cuando el Perosnaje pierda vitalidad.

public class TPBarUI : MonoBehaviour
{
    //Referencia al Slider
    private Slider mSlider;

    //-----------------------------------------------

    void Start()
    {
        mSlider = GetComponent<Slider>();
        
        //Añadimos al TPBarUI como Observador del Evento OnEnemyDamage
        GameManager.Instance.OnEnemyDamage += OnEnemyDamageDelegate;
        
    }

    //Esto permitirá que todos los enemigos afectados por el Evento puedan llamar a esta ejecución
    private void OnEnemyDamageDelegate()
    {
        //Incremento el Valor del Slider en 1
        mSlider.value += 5.00f;
    }
}
