using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;


public class CoinManager : MonoBehaviour
{
    [SerializeField] private TMP_Text coinText;
    [SerializeField] private PointsEvent points;


    void Start()
    {
        UpdateValueOnScreen();
    }

    public void UpdateValueOnScreen()
    {
        
    }
}
