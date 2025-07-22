using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;

public class MenuManager : MonoBehaviour
{

    [SerializeField] private PointsEvent points;
    [SerializeField] private TMP_Text bestScoreText;


    void Start()
    {
        updateBestScore();
    }

    void updateBestScore()
    {
        bestScoreText.SetText(points.BestScore().ToString());
    }
}
