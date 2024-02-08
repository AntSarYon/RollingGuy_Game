using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem.Controls;
using UnityEngine.UI;
using static UnityEngine.RuleTile.TilingRuleOutput;

public class TeleportController : MonoBehaviour
{
    //Referencia al GameManager
    private GameManager gameManager;

    //Referencia al Script de PlayerMovement
    private PlayerMovement player;

    [Header("Sonido de Teletransporte")]
    [SerializeField]
    private AudioClip clipIniciaTP;

    [Header("Sonido de Fin de Teletransporte")]
    [SerializeField]
    private AudioClip clipFinTP;

    //Distancia de Teletransporte máxima permitida.
    private float distanciaTPMaxima;

    //Referencia a la Barra de Ataque
    private Slider barraAtaque;
    private float valorMaximoDeBarra;
    private float numAtaquesNecesarios;

    //Posiciones para medir la distancia Teletransporte
    private Vector3 posInicioTP;
    private Vector3 posActualTP;
    private float distanciaRecorrida;

    // GETTERS Y SETTERS
    public Vector2 PosInicioTP { get => posInicioTP; set => posInicioTP = value; }
    public Vector2 PosActualTP { get => posActualTP; set => posActualTP = value; }

    //------------------------------------------------------

    private void Awake()
    {
        player = GetComponent<PlayerMovement>();
        gameManager = GameManager.Instance;
        
        //Buscamos el objeto de UI que contiene el Slider de nuestro interés
        barraAtaque = GameObject.Find("TPBar").GetComponent<Slider>();
    }

    private void Start()
    {
        //Obtenemos el máximo valor que puede recibir la barra de vida
        //Este siempre debe ser un valor de 10
        valorMaximoDeBarra = barraAtaque.maxValue;

        //Cada ataque agregará 10 unidades a la barra de ataque
        numAtaquesNecesarios = valorMaximoDeBarra / 5;

        //La distancia maxima para el teletransporte es el valor maximo de la barra
        distanciaTPMaxima = valorMaximoDeBarra;
    }

    //---------------------------------------------------

    void Update()
    {
        //Si el personaje No se está teletransportando
        if (player.IsTeleporting == false)
        {
            //Si se oprime la tecla T
            if (Input.GetKeyDown(KeyCode.T))
            {
                //Si la Barra de Ataque está en su máximo
                if (barraAtaque.value == valorMaximoDeBarra)
                {
                    player.MAudioSource.PlayOneShot(clipIniciaTP, 0.65f);
                    //Activamos el Flag de Teletransporte del Personaje
                    player.IsTeleporting = true;

                    //Almacenamos la posicionInicial del Personaje
                    PosInicioTP = transform.position;
                }
            }
        }

        //Si el personaje si se está teletransportando...
        else
        {
            //Actualizamos la referencia a la posición Actual
            posActualTP = transform.position;

            //Calculamos la distancia entre la Pos Actual y la Pos de inicio de TP
            distanciaRecorrida = (PosInicioTP - PosActualTP).magnitude;

            //la barra de ataque se ira reduciendo mientras mas cerca este
            // el jugador de la ditancia máxima permitida
            barraAtaque.value = distanciaTPMaxima - distanciaRecorrida;

            //Si la distancia supera el maximo permitido
            if (barraAtaque.value == 0)
            {
                //Desactivamos la animacion Teletransporte
                player.MAnimator.SetBool("IsTeleporting", false);

                //Desactivamos el Flag de Teletransporte
                player.IsTeleporting = false;
            }
        }
    }
}