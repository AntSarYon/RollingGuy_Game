using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class VictoryCheck : MonoBehaviour
{
    public static VictoryCheck Instance;

    //Flag de Victoria
    private bool victory;

    public bool Victory { get => victory; set => victory = value; }

    //---------------------------------------------------------------------

    void Awake()
    {
        //Asignamos Instancia de Clase
        Instance = this;

        //Iniciamos con el Flag de Victoria desactivado
        victory = false;
    }

    //---------------------------------------------------------------------

    private void OnTriggerEnter2D(Collider2D collision)
    {
        //Si el Objeto entro al Trigger
        if (collision.CompareTag("Player"))
        {
            //Activamos el Flag de Victoria
            victory=true;

            //Invocamos al Evento de Victoria Alcanzada
            GameManager.Instance.VictoryAchieved();
        }
    }

}
