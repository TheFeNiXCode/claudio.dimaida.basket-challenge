using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;

public class ScoreManager : MonoBehaviour
{
    [SerializeField] private PointsEvent points;
    [SerializeField] private TMP_Text scoreText;
    [SerializeField] private Image spriteScore;
    [SerializeField] private int defaultScoreToAdd = 10;
    [SerializeField] private int defaultBackspinScoreToAdd = 10;

    private int bestScore;

    public int score { get; private set; }


    public void UpdateScore(PlayerManager player)
    {
        
        score += defaultScoreToAdd;
        if (player.isThisShotABackspin) score += defaultBackspinScoreToAdd;

        if(score > points.BestScore()) scoreText.color = new Color(243, 155, 0, 255);
        scoreText.SetText(score.ToString());

        if (points.checkIfUpdateSpriteScore(score))
        {
            spriteScore.sprite = points.UpdateGameScoreSprite(score);
            StartCoroutine(PopAnimation(spriteScore.rectTransform));
        }
    }


    private IEnumerator PopAnimation(RectTransform target)
    {
        Vector3 originalScale = target.localScale;
        Vector3 enlargedScale = originalScale * 1.3f;
        float duration = 0.1f;

        // Ingrandisci
        float t = 0f;
        while (t < duration)
        {
            t += Time.deltaTime;
            target.localScale = Vector3.Lerp(originalScale, enlargedScale, t / duration);
            yield return null;
        }

        // Ritorna alla scala originale
        t = 0f;
        while (t < duration)
        {
            t += Time.deltaTime;
            target.localScale = Vector3.Lerp(enlargedScale, originalScale, t / duration);
            yield return null;
        }

        target.localScale = originalScale;
    }

}
