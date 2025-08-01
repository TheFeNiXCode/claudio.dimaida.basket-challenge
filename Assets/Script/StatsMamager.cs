using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class StatsMamager : MonoBehaviour
{
    [Header("Stats UI Text")]
    [SerializeField] private TMP_Text win;
    [SerializeField] private TMP_Text lose;
    [SerializeField] private TMP_Text draw;
    [SerializeField] private TMP_Text shot;
    [SerializeField] private TMP_Text perfect;
    [SerializeField] private TMP_Text backspin;
    [SerializeField] private TMP_Text backboard;


    [Header("Settings")]
    [SerializeField] PointsEvent pointsEvent;


    void Start()
    {
        win.SetText(pointsEvent.GetWin().ToString());
        lose.SetText(pointsEvent.GetLose().ToString());
        draw.SetText(pointsEvent.GetDraw().ToString());
        shot.SetText(pointsEvent.GetShots().ToString());
        perfect.SetText(pointsEvent.GetPerfect().ToString());
        backboard.SetText(pointsEvent.GetBackboard().ToString());
        backspin.SetText(pointsEvent.GetBackspin().ToString());
    }

}
