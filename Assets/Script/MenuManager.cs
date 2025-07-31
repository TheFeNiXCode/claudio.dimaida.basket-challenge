using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;

public class MenuManager : MonoBehaviour
{

    [SerializeField] private PointsEvent points;
    [SerializeField] private TMP_Text bestScoreText;
    [SerializeField] private TMP_Text coinsText;


    void Start()
    {
        updateBestScore();
        updateCoins();
    }

    void updateBestScore()
    {
        bestScoreText.SetText(points.BestScore().ToString());
    }

    void updateCoins()
    {
        coinsText.SetText(points.TotalCoins().ToString());
    }
}
