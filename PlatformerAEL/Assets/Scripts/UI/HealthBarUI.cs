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

    //Valor inicial del Slider (antes de moverse)
    private float sliderInitialValue;
    private float sliderFinalValue;

    private float interpolation;

    //--------------------------------------------------------

    void Awake()
    {
        //Obtenemos referencia al Slider
        mSlider = GetComponent<Slider>();

        //Los valores iniciales y finales del Slider empiezan en 100 (el maximo)
        sliderInitialValue = 100;
        sliderFinalValue = 100;

        //El factor de interpolacion inicia en 0
        interpolation = 0;
    }

    //--------------------------------------------------------

    void Start()
    {
        //Añadimos al HealthBar como Observador de Eventos
        GameManager.Instance.OnPlayerDamage += OnPlayerDamageDelegate; 
        GameManager.Instance.OnPlayerDamage += OnPlayerBeingResurrected;

        //Actualizamos el valor inicial del Slider
        sliderInitialValue = mSlider.value;
    }

    

    void Update()
    {
        //Interpolamos el valor de la Barra constantemente
        mSlider.value = Mathf.Lerp(sliderInitialValue, sliderFinalValue, interpolation);

        //Incrementamos el factor de Interpolacion cada frame
        interpolation += 0.8f * Time.deltaTime;

        //Comprobamos si la interpolacion ha llegado a 1
        if (interpolation >= 1.0f)
        {
            //Hacemos que el inicial sea igual al final
            sliderInitialValue = sliderFinalValue;

            //Devolvemos el factor de Interpolacion a 0
            interpolation = 0.0f;
        }
    }

    //--------------------------------------------------------------------

    private void OnPlayerDamageDelegate()
    {
        //Asiganmos un nuevo Valor Final para la Barra de vida
        sliderFinalValue = mSlider.value - GameManager.Instance.DamageReceivedInProgress;

        //Limitamos su valor maximo y final para que no exceda los limites del Slide
        Mathf.Clamp(sliderFinalValue, 0.0f, 100f);

        //Si el valor Final del Slider será de 0
        if (sliderFinalValue <= 0)
        {
            //Nos teletransortamos al nuevo punto de Checkpoint.
            GameManager.Instance.Player.position = GameManager.Instance.UltimoCheckpoint;

            //Reseteamos los valores de la Barra de Vida
            sliderInitialValue = 100;
            sliderFinalValue = 100;

            //El factor de interpolacion inicia en 0
            interpolation = 1;

            //Devolvemos el Slider al Valor máximo
            mSlider.value = mSlider.maxValue;
        }
    }

    //------------------------------------------------------------------------

    private void OnPlayerBeingResurrected()
    {
        
    }
}
