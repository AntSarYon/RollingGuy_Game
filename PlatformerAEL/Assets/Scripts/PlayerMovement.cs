using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerMovement : MonoBehaviour
{
    [SerializeField]
    private float runSpeed = 4f;
    [SerializeField]
    private float jumpSpeed = 10f;

    private Vector2 mMoveInput;
    private Rigidbody2D mRb;
    private Animator mAnimator;
    private CapsuleCollider2D mCollider;
    private bool IsJumping = false;

    private void Start()
    {
        mRb = GetComponent<Rigidbody2D>();
        mAnimator = GetComponent<Animator>();
        mCollider = GetComponent<CapsuleCollider2D>();
    }

    private void Update()
    {
        mRb.velocity = new Vector2(
            mMoveInput.x * runSpeed,
            mRb.velocity.y
        );

        if (Mathf.Abs(mRb.velocity.x) > Mathf.Epsilon)
        {
            transform.localScale = new Vector3(
                Mathf.Sign(mRb.velocity.x),
                transform.localScale.y,
                transform.localScale.z
            );
            // Idle -> Running
            mAnimator.SetBool("IsRunning", true);
        }else {
            mAnimator.SetBool("IsRunning", false);
        }

        if (mRb.velocity.y < 0f)
        {
            mAnimator.SetBool("IsJumping", false);
            mAnimator.SetBool("IsFalling", true);
        }

        if (mCollider.IsTouchingLayers(LayerMask.GetMask("Ground")))
        {
            // Toco el suelo
            mAnimator.SetBool("IsFalling", false);
        }
        
    }

    private void OnMove(InputValue value)
    {
        mMoveInput = value.Get<Vector2>();

    }

    private void OnJump(InputValue value)
    {
        // Verificar si estamos en pleno salto o no
        if (mCollider.IsTouchingLayers(LayerMask.GetMask("Ground")))
        {
            if (value.isPressed)
            {
                // Saltar
                mRb.velocity = new Vector2(
                    mRb.velocity.x,
                    jumpSpeed
                );
                mAnimator.SetBool("IsJumping", true);
            }
        }
    }
}
