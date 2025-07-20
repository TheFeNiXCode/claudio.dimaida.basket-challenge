using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu]
public class PlayerSettings : SavableObject
{

    [SerializeField] private int typeOfBall = 0;
    [SerializeField] private List<GameObject> listOfBall;

    public override string nameFile { get { return "PlayerSettings"; } }

    public override void ResetFile()
    {
        typeOfBall = 0;
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
}
