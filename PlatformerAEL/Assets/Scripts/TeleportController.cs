using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using static UnityEngine.RuleTile.TilingRuleOutput;

public class TeleportController : MonoBehaviour
{
    //Referencia al Script de PlayerMovement
    private PlayerMovement personaje;

    //Definimos la distancia máxima permitida entre los puntos de teletransporte.
    private float distanciaMaxima = 8f;

    //Referencia al Slider de Ataque
    [SerializeField] private Slider hcSlider;

    //------------------------------------------------------

    private void Awake()
    {
        personaje = GetComponent<PlayerMovement>();
    }

    // Start is called before the first frame update
    void Start()
    {
        
    }

    //---------------------------------------------------

    // Update is called once per frame
    void Update()
    {
        //Si el valor del Slide llega a 3, y se oprime la tecla T
        if (hcSlider.value == 3 )
        {
            if (Input.GetKeyDown(KeyCode.T) && personaje.IsTeleporting == false)
            {
            //Activamos el Flag de Teletransporte
            personaje.IsTeleporting = true;

            //Almacenamos la posicionInicial del Personaje
            personaje.PosInicioTP = transform.position;

            //Activamos la Animación de Teletransporte
            //Desactivamos todas sus animaciones, y activamos la de HIT
            personaje.MAnimator.SetBool("IsDoubleJumping", false);
            personaje.MAnimator.SetBool("IsJumping", false);
            personaje.MAnimator.SetBool("IsRunning", false);
            personaje.MAnimator.SetBool("IsFalling", false);
            personaje.MAnimator.SetBool("IsAttacking", false);
            personaje.MAnimator.SetBool("WallNear", false);
            personaje.MAnimator.SetBool("IsBeingHit", false);
            personaje.MAnimator.SetBool("IsTeleporting", true);
            
                hcSlider.value = 0;
            }
        }
        else if (hcSlider.value ==0)
        {
            if(Input.GetKeyDown(KeyCode.T))

            //Sino, lo desactivamos
            personaje.IsTeleporting = false;
        }
    }
}
