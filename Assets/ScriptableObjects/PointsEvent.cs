using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu]
public class PointsEvent : SavableObject
{

    [SerializeField] private int bestScore;
    [SerializeField] private int totalCoins;
    [SerializeField] private List<Sprite> spriteReward;
    [SerializeField] private int pointToSilverScore = 50;
    [SerializeField] private int pointToGoldenScore = 100;

    private int indexSprite;
    public override string nameFile { get { return "Points"; } }

    public override void ResetFile()
    {
        bestScore = 0;
        totalCoins = 0;
        indexSprite = 0;
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

    public bool checkIfUpdateSpriteScore(int actualScore)
    {
        bestScore = Mathf.Max(bestScore, actualScore);
        SaveFile();

        if (actualScore >= pointToSilverScore && indexSprite == 0) return true;
        if (actualScore >= pointToGoldenScore && indexSprite == 1) return true;
        return false;
    }

    public Sprite UpdateGameScoreSprite(int actualScore)
    {
        if (actualScore < pointToSilverScore)
        {
            indexSprite = 0;
            return spriteReward[0]; // bronzo
        }
        else if (actualScore < pointToGoldenScore)
        {
            indexSprite = 1;
            return spriteReward[1]; //argento
        }
        else
        {
            indexSprite = 2;
            return spriteReward[2]; // oro
        }
    }


    public int BestScore()
    {
        return bestScore;
    }
}
