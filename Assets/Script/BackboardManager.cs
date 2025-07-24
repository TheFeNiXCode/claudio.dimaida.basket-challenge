using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class WeightedValue
{
    public int index;       // Il valore da ritornare
    [Range(0f, 1f)]
    public float weight;    // La sua probabilità relativa
}

public class BackboardManager : MonoBehaviour
{
    [SerializeField] private GameObject canvasText;
    public bool hitBackboard = false;
    [SerializeField] private TextGlowAnimator glow;

    [SerializeField] PlayerSettings settings;

    [SerializeField] private List<WeightedValue> bonusBackboard = new List<WeightedValue>();

    [SerializeField] private int indexBonus = 3;

    private void Start()
    {
        SetBonusBackboardValue(settings.BonusProbability());
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Ball") && other.transform.root.CompareTag("Player"))
        {
            hitBackboard = true;
        }
    }

    public void SetBonusBackboardValue(List<(int index, float weight)> newValues)
    {
        bonusBackboard.Clear();
        foreach (var item in newValues)
        {
            WeightedValue entry = new WeightedValue
            {
                index = item.index,
                weight = item.weight
            };
            bonusBackboard.Add(entry);
        }
    }


    public int GetRandomProbabilityValue()
    {
        if (bonusBackboard == null || bonusBackboard.Count == 0)
        {
            Debug.LogError("Lista di valori pesati vuota!");
            return 0;
        }

        // Calcola la somma totale dei pesi
        float total = 0f;
        foreach (var entry in bonusBackboard)
        {
            total += entry.weight;
        }

        if (Mathf.Approximately(total, 0f))
        {
            Debug.LogError("Somma pesi = 0!");
            return bonusBackboard[0].index;
        }

        // Numero casuale in [0, total]
        float randomPoint = Random.value * total;
        float cumulative = 0f;

        foreach (var entry in bonusBackboard)
        {
            cumulative += entry.weight;
            if (randomPoint <= cumulative)
            {
                return entry.index;
            }
        }

        return bonusBackboard[bonusBackboard.Count - 1].index; // fallback
    }


    public void BonusBackboard()
    {
        if (hitBackboard && !glow.isAnimationActive)
        {
            Debug.Log("Entro!");

            indexBonus = GetRandomProbabilityValue();
            Debug.Log("Bonus value: "+ indexBonus);

            if (indexBonus != 3)
            {
                StartCanvasAnimation(indexBonus);
            }
        }
    }

    public int AddBonusPoints()
    {
        if (glow.isAnimationActive)
        {
            if (indexBonus == 0) return 4;
            if (indexBonus == 1) return 6;
            if (indexBonus == 2) return 8;
        }
        return 0;
    }


    public void StartCanvasAnimation(int index)
    {
        if (hitBackboard && !glow.isAnimationActive)
        {
            canvasText.SetActive(true);
            glow.StartEffect(index);
            StartCoroutine(TimerToDisable(settings.getDurationBackboard()));
        }
    }


    IEnumerator TimerToDisable(float duration)
    {
        yield return new WaitForSeconds(duration);

        this.gameObject.GetComponent<TextGlowAnimator>().StopEffect();
        canvasText.SetActive(false);

    }

}
