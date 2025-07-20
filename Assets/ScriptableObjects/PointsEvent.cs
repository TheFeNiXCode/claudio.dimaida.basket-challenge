using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu]
public class PointsEvent : SavableObject
{

    [SerializeField]
    private int bestScore;
    [SerializeField]
    private int totalCoins;

    public override string nameFile { get { return "Points"; } }

    public override void ResetFile()
    {
        bestScore = 0;
        totalCoins = 0;
    }
    public void UpdateCoins(int newCoins)
    {
        totalCoins += newCoins;
        SaveFile();
    }
    public void UpdatePoints(int lastScore, int newCoins) {

        bestScore = Mathf.Max(bestScore, lastScore);
        totalCoins += newCoins;
        SaveFile();
    }

    
}
