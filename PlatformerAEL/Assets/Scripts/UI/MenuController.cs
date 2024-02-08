using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MenuController : MonoBehaviour
{

    public void StartGame()
    {

    }

    //----------------------------------------------

    public void PlayJump()
    {
        AudioManager.instance.PlaySfx("Jump");
    }

    public void PlayDoubleJump()
    {
        AudioManager.instance.PlaySfx("DoubleJump");
    }

    public void PlayAttack()
    {
        AudioManager.instance.PlaySfx("Attacking");
    }

    public void PlayHit()
    {
        AudioManager.instance.PlaySfx("Hit");
    }
}
