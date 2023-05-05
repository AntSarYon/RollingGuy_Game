using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BossIA : MonoBehaviour
{

    [SerializeField]
    private float speed = 1f;
    [SerializeField]
    private float rayDistance = 5f;
    private Rigidbody2D mRb;
    private BoxCollider2D mCollider;

    private void Start()
    {
        mRb = GetComponent<Rigidbody2D>();
        mCollider = GetComponent<BoxCollider2D>();
    }

    private void Update()
    {
        mRb.velocity = new Vector2(speed, mRb.velocity.y);
        if (VerificarCaida())
        {
            // Cambiar direccion
            speed *= -1;
        }
       
    }

    private bool VerificarCaida()
    {
        //Lanzar raycast
        var hit = Physics2D.Raycast(
            transform.position,
            new Vector2(
                mRb.velocity.x < 0f ? -1 : 1,
                -1f
            ).normalized,
            rayDistance,
            LayerMask.GetMask("Ground")
        );

        if (hit) 
        {
            Debug.DrawRay(
                transform.position,
                new Vector2(
                    mRb.velocity.x < 0f ? -1 : 1,
                    -1f
                ).normalized * rayDistance,
                Color.red
            );
            return false;
        }
        else 
        {
            Debug.DrawRay(
                transform.position,
                new Vector2(
                    mRb.velocity.x < 0f ? -1 : 1,
                    -1f
                ).normalized * rayDistance,
                Color.green
            );
            return true;
        }
}}
