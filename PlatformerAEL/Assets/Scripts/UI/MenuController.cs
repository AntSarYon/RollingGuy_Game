using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class MenuController : MonoBehaviour
{
    [SerializeField] private GameObject OptionsPanel;

    //----------------------------------------------

    void Start()
    {
        //Iniciamos desactivando el Panel de Opciones
        OptionsPanel.SetActive(false);
    }

    //----------------------------------------------

    public void OpenOptionsPanel()
    {
        //Activamos el Panel de Options
        OptionsPanel.SetActive(true);
    }

    public void CloseOptionsPanel()
    {
        //Activamos el Panel de Options
        OptionsPanel.SetActive(false);
    }

    //----------------------------------------------

    public void StartFadeIn()
    {
        GetComponent<Animator>().Play("FadeIn");
    }


    public void StartGame()
    {
        SceneManager.LoadScene("Level1");
    }

    //----------------------------------------------

    public void PlayBackgroundMusic()
    {
        AudioManager.instance.updateMusic("MenuMusic");
    }

    public void PlayBlankMusic()
    {
        //Actualizamos la musica al Blank
        AudioManager.instance.updateMusic("Blank");
    }

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

    public void PlayOption()
    {
        AudioManager.instance.PlaySfx("Option");
    }
}
