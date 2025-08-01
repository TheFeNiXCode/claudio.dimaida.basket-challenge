using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HitAreaShoot : MonoBehaviour
{

    [SerializeField] private List<GameObject> perfectArea;
    [SerializeField] private List<GameObject> backboardArea;
    private int currentIndexArea = 0;

    [SerializeField] private List<float> perfectAreaValue;
    [SerializeField] private List<float> backboardAreaValue;

    [SerializeField] private float perfectAreaIncluded = 12f;
    [SerializeField] private float backboardAreaIncluded = 6f;


    void Start()
    {
        for (int i = 0; i < perfectArea.Count; i++)
            perfectArea[i].SetActive(i == 0);

        for (int i = 0; i < backboardArea.Count; i++)
            backboardArea[i].SetActive(i == 0);
    }

    public void choiceArea()
    {
        if (perfectArea.Count == 0 || backboardArea.Count == 0)
        {
            Debug.LogWarning("Una delle liste è vuota!");
        }

        if (currentIndexArea < perfectArea.Count)
            perfectArea[currentIndexArea].SetActive(false);

        if (currentIndexArea < backboardArea.Count)
            backboardArea[currentIndexArea].SetActive(false);

        int randomIndex;
        do
        {
            randomIndex = Random.Range(0, perfectArea.Count);
        } while (randomIndex == currentIndexArea);

        if (randomIndex < perfectArea.Count)
            perfectArea[randomIndex].SetActive(true);

        if (randomIndex < backboardArea.Count)
            backboardArea[randomIndex].SetActive(true);

        currentIndexArea = randomIndex;

    }

    public bool PerfectShoot(float value)
    {
        if ((perfectAreaValue[currentIndexArea] - 2f) <= value)
        {
            if (value <= (perfectAreaValue[currentIndexArea] + perfectAreaIncluded)) return true;
        }

        return false;
    }

    public bool BackboardShoot(float value)
    {
        if ((backboardAreaValue[currentIndexArea] - 1f) <= value)
        {
            if (value <= (backboardAreaValue[currentIndexArea] + backboardAreaIncluded)) return true;
        }
        return false;
    }

    public int getIndex() => currentIndexArea;

}
