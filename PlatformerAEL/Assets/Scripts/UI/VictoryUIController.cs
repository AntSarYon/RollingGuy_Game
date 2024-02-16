using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;

public class VictoryUIController : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI txtTimeAchieved;
    [SerializeField] private TextMeshProUGUI txtTimeRecord;
    [SerializeField] private GameObject txtTimeNewRecordMessage1;
    [SerializeField] private GameObject txtTimeNewRecordMessage2;
    [Space]
    [SerializeField] private TextMeshProUGUI txtPointsAchieved;
    [SerializeField] private TextMeshProUGUI txtPointsRecord;
    [SerializeField] private GameObject txtPointsNewRecordMessage1;
    [SerializeField] private GameObject txtPointsNewRecordMessage2;

    //-----------------------------------------------------------------------------------

    private void OnEnable()
    {
        //Actualizamos valores de Tiempo
        if (Timer.Instance.timer < 10)
            txtTimeAchieved.text = $"{Timer.Instance.minutes}:0{(int)Timer.Instance.timer}";
        else
            txtTimeAchieved.text = $"{Timer.Instance.minutes}:{(int)Timer.Instance.timer}";


        if (RecordManager.Instance.secondsRecord < 10)
            txtTimeRecord.text = $"{RecordManager.Instance.minutesRecord}:0{(int)RecordManager.Instance.secondsRecord}";

        else
            txtTimeRecord.text = $"{RecordManager.Instance.minutesRecord}:{(int)RecordManager.Instance.secondsRecord}";

        //Actualizmaos valores de Puntuacion
        txtPointsAchieved.text = PointsManager.Instance.actualPoints.ToString();
        txtPointsRecord.text = RecordManager.Instance.pointsRecord.ToString();

        //Controlamos visualizacion de Textos de Nuevo record, solo si realmente se consiguió un nuevo record
        if (RecordManager.Instance.timeRecordUpdated)
        {
            txtTimeNewRecordMessage1.SetActive(true);
            txtTimeNewRecordMessage2.SetActive(true);
        }
        else
        {
            txtTimeNewRecordMessage1.SetActive(false);
            txtTimeNewRecordMessage2.SetActive(false);
        }


        if (RecordManager.Instance.pointsRecordUpdated)
        {
            txtPointsNewRecordMessage1.SetActive(true);
            txtPointsNewRecordMessage2.SetActive(true);
        }
        else
        {
            txtPointsNewRecordMessage1.SetActive(false);
            txtPointsNewRecordMessage2.SetActive(false);
        }
    }

    //-----------------------------------------------------------------------------------

    public void Retry()
    {
        //Desactivamos todo el CANVAS de Victoria
        transform.parent.gameObject.SetActive(false);

        //Reproducimos el FadeIn de Fin de jeugo
        HudController.Instance.PlayVictoryFadeIn();


        //Desactivamos los Flags de RecordActualizados
        RecordManager.Instance.timeRecordUpdated = false;
        RecordManager.Instance.pointsRecordUpdated = false;

        SceneManager.LoadScene("Level1");
    }

    public void GoToMenu()
    {
        //Desactivamos todo el CANVAS de Victoria
        transform.parent.gameObject.SetActive(false);

        //Reproducimos el FadeIn de Fin de jeugo
        HudController.Instance.PlayVictoryFadeIn();

        //Desactivamos los Flags de RecordActualizados
        RecordManager.Instance.timeRecordUpdated = false;
        RecordManager.Instance.pointsRecordUpdated = false;

        SceneManager.LoadScene("Menu");
    }
}
