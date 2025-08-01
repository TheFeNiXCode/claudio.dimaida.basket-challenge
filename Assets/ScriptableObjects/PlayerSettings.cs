using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu]
public class PlayerSettings : SavableObject
{

    [SerializeField] private int typeOfBall = 0;
    [SerializeField] private float durationBackboardText = 10f;
    [SerializeField] private bool battleVSCPU = false;

    [SerializeField] private List<GameObject> listOfBall;

    [SerializeField, Range(0, 2)] private int gameDifficult = 1;   // 0 = easy, 1 = normal, 2 = hard

    [SerializeField] private int sensivity = 1600;

    [SerializeField] private bool audio = true;


    public override string nameFile { get { return "PlayerSettings"; } }

    public override void ResetFile()
    {
        battleVSCPU = false;
        typeOfBall = 0;
        durationBackboardText = 10f;
        gameDifficult = 1;
        sensivity = 1600;
        audio = true;
    }

    public bool IsVSCPU()
    {
        return battleVSCPU;
    }

    public void battleVSCPUSelected(bool battle)
    {
        battleVSCPU = battle;
        SaveFile();
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

    public void setGameSensivity(int s)
    {
        switch (s)
        {
            case 0:
                sensivity = 2400;
                break;
            case 1:
                sensivity = 2000;
                break;
            case 2:
                sensivity = 1600;
                break;
            case 3:
                sensivity = 1200;
                break;
            case 4:
                sensivity = 800;
                break;
        }
        SaveFile();
    }

    public int getSensivity()
    {
        switch (sensivity)
        {
            case 2400:
                return 0;
            case 2000:
                return 1;
            case 1600:
                return 2;
            case 1200:
                return 3;
            case 800:
                return 4;
            default:
                return 0;
        }
    }

    public int getRealSenivity()
    {
        return sensivity;
    }

    public void setAudio(bool a)
    {
        audio = a;
    }
    public bool getAudio()
    {
        return audio;
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
