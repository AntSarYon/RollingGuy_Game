using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UIElements;

public class EnemyIA : MonoBehaviour
{
    //Velocidad
    [SerializeField]
    private float speed = 1f;

    //Distancia del Rayo
    [SerializeField]
    private float rayDistance = 3f;
    
    //Referencias al RigidBody y al Collider
    private Rigidbody2D mRb;
    private BoxCollider2D mCollider;

    //------------------------------------------------------------------------------

    private void Start()
    {
        //Obtención de componentes referencia
        mRb = GetComponent<Rigidbody2D>();
        mCollider = GetComponent<BoxCollider2D>();
    }

    //---------------------------------------------------------------------------------
    private void Update()
    {
        //Actualiza la velocidad
        mRb.velocity = new Vector2(speed, mRb.velocity.y);

        //Si detecta que hay una caida en esa dirección...
        if (VerificarCaida())
        {
            // Cambiar direccion
            speed *= -1;
        }
       
    }
    //------------------------------------------------------------------------------------
    private bool VerificarCaida()
    {
        //Lanzar raycast hacia el suelo delante
        var hit = Physics2D.Raycast(
            transform.position,
            new Vector2(
                mRb.velocity.x < 0f ? -1 : 1,
                -1f
            ).normalized,
            rayDistance,
            LayerMask.GetMask("Ground") //Buscamos un choque con la capa Ground
        );

        //Retornamos el resultado (Impacto o caida)
        return hit;
    }

    //---------------------------------------------------------------------

    private void OnDrawGizmos()
    {
        mRb = GetComponent<Rigidbody2D>();
        Gizmos.DrawRay(
            transform.position,
            new Vector2(
                mRb.velocity.x < 0f ? -1 : 1,
                -1f
            ).normalized*rayDistance
       );
    }
}
