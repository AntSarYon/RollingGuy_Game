using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class PauseMenuController : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void Retry()
    {
        //Desactivamos todo el CANVAS de Victoria
        transform.parent.gameObject.SetActive(false);

        //Desactivamos la pausa
        ClosePause();

        //Desactivamos los Flags de RecordActualizados
        RecordManager.Instance.timeRecordUpdated = false;
        RecordManager.Instance.pointsRecordUpdated = false;

        SceneManager.LoadScene("Level1");
    }

    public void GoToMenu()
    {
        //Desactivamos todo el CANVAS de Victoria
        transform.parent.gameObject.SetActive(false);

        //Desactivamos la pausa
        ClosePause();

        //Desactivamos los Flags de RecordActualizados
        RecordManager.Instance.timeRecordUpdated = false;
        RecordManager.Instance.pointsRecordUpdated = false;

        SceneManager.LoadScene("Menu");
    }

    public void ClosePause()
    {
        GameManager.Instance.InPause = false;
    }

    public void UpdateMusicVolume(float newVolume)
    {
        AudioManager.instance.updateMusicVolume(newVolume);
    }

    public void UpdateSFXVolume(float newVolume)
    {
        AudioManager.instance.updateSfxVolume(newVolume);
    }

}
