using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Timer : MonoBehaviour
{
    public static Timer Instance { get; private set; }

    [HideInInspector] public float timer;
    [HideInInspector] public int minutes;

    [HideInInspector] public float recordSeconds;
    [HideInInspector] public int recordMinutes;

    //------------------------------------------------------------------

    void Awake()
    {
        //Asignamos esta como unica Instancia
        Instance = this;

        //Iniciamos el Timer
        timer = 0.0f;
        minutes = 0;

        //Iniciamos el Tiempo record
        recordSeconds = 0;
        recordMinutes = 2;
        
    }

    //------------------------------------------------------------------

    void Update()
    {
        //Mientras el Flag de Victoria este desactivado...
        if (!GameManager.Instance.Victory)
        {
            //Incrementamos el Timer
            timer += Time.deltaTime;

            //Si el timer suma 60 segundos
            if (timer >= 60)
            {
                //Incrementamos el contador de Minutos en 1
                minutes++;

                //Reiniciamos el Timer en 0
                timer = 0.0f;
            }
        }
    }
}
