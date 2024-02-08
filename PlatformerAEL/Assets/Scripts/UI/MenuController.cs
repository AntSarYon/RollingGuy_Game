using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class MenuController : MonoBehaviour
{
    public void StartFadeIn()
    {
        GetComponent<Animator>().Play("FadeIn");
    }

    public void StartGame()
    {
        SceneManager.LoadScene("Level1");
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
