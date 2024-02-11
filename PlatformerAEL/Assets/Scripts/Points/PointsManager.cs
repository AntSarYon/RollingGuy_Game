using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PointsManager : MonoBehaviour
{
    public static PointsManager Instance;

    public int actualPoints;
    public int recordPoints;

    //-----------------------------------------------

    void Awake()
    {
        Instance = this;

        //Inicializmaos los puntos actuales y el Record
        actualPoints = 0;
        recordPoints = 500;
    }
}
