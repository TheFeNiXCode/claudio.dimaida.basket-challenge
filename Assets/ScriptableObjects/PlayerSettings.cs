using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu]
public class PlayerSettings : SavableObject
{

    [SerializeField] private int typeOfBall = 0;
    [SerializeField] private float durationBackboardText = 10f;

    [SerializeField] private List<GameObject> listOfBall;

    [SerializeField, Range(0, 2)] private int gameDifficult = 1;   // 0 = easy, 1 = normal, 2 = hard

    public override string nameFile { get { return "PlayerSettings"; } }

    public override void ResetFile()
    {
        typeOfBall = 0;
        durationBackboardText = 10f;
        gameDifficult = 1;
    }

    public void newBallSelected(int indexBall)
    {
        typeOfBall = indexBall;
        SaveFile();
    }

    public GameObject ballSelected()
    {
        return listOfBall[typeOfBall];
    }

    public void setDurationBackboard(float duration)
    {
        durationBackboardText = duration;
        SaveFile();
    }

    public float getDurationBackboard()
    {
        return durationBackboardText;
    }

    public int getGameDifficult()
    {
        return gameDifficult;
    }

    public void setGameDifficult(int difficult)
    {
        gameDifficult = difficult;
        SaveFile();
    }

    public List<(int, float)> BonusProbability()
    {
        List<(int, float)> list;

        switch (gameDifficult)
        {
            case 0:
                list = new List<(int, float)>
                {
                    (3, 0.2f),  // 20%
                    (0, 0.3f),  // 40%
                    (1, 0.15f), // 25%
                    (2, 0.2f)  //  15%
                };
                break;
            case 1:
                list = new List<(int, float)>
                {
                    (3, 0.4f),  // 40%
                    (0, 0.3f),  // 30%
                    (1, 0.2f), // 20%
                    (2, 0.1f)  // 10%
                };
                break;
            case 2:
                list = new List<(int, float)>
                {
                    (3, 0.5f),  // 50%
                    (0, 0.3f),  // 30%
                    (1, 0.15f), // 15%
                    (2, 0.05f)  // 5%
                };
                break;
            default:
                list = new List<(int, float)>
                {
                    (3, 0.4f),  // 40%
                    (0, 0.3f),  // 30%
                    (1, 0.2f), // 20%
                    (2, 0.1f)  // 10%
                };
                break;
        }
        return list;
    }
}
