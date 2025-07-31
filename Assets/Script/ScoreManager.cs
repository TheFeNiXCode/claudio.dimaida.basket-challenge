using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;

public class ScoreManager : MonoBehaviour
{

    [Header("Points")]
    [SerializeField] private PointsEvent points;
    [SerializeField] private BackboardManager backboard;
    [SerializeField] private TMP_Text scoreText;
    [SerializeField] private Image spriteScore;
    [SerializeField] private int defaultScoreToAdd = 10;
    [SerializeField] private int defaultBackspinScoreToAdd = 10;
    [SerializeField] private int defaultMultiplierBackspin = 5;
    [SerializeField] private int defaultMutliplierBackboard = 10;



    [Header("ResultsUI")]
    [SerializeField] private TMP_Text rScoreText;
    [SerializeField] private TMP_Text rBackboardText;
    [SerializeField] private TMP_Text rBackspinText;
    [SerializeField] private TMP_Text rCoinText;
    [SerializeField] private StarsManager starsManager;


    public int score { get; private set; }
    public int backboardHit { get; private set; }
    public int backspinHit { get; private set; }

    void Start()
    {
        setIntialValue();
    }

    public void UpdateScore(PlayerManager player)
    {
        
        score += defaultScoreToAdd;
        if (player.isThisShotABackspin) {

            score += defaultBackspinScoreToAdd;
            backspinHit += 1;
        } 
        score += backboard.AddBonusPoints();

        if(score > points.BestScore()) scoreText.color = new Color(243, 155, 0, 255);
        scoreText.SetText(score.ToString());

        if (points.checkIfUpdateSpriteScore(score))
        {
            spriteScore.sprite = points.UpdateGameScoreSprite(score);
            StartCoroutine(PopAnimation(spriteScore.rectTransform));
        }
    }

    public int getScore()
    {
        return score;
    }

    public int getBackboard()
    {
        return backboardHit;
    }

    public int getBackspin()
    {
        return backspinHit;
    }

    private void setIntialValue()
    {
        score = 0;
        backboardHit = 0;
        backspinHit = 0;
    }

    public void AddBackboardHit()
    {
        backboardHit += 1;
    }

    public void UpdateResultsTable()
    {
        rScoreText.SetText("+ " + score.ToString());
        rBackboardText.SetText("+ " + backboardHit.ToString() + " x " + defaultMutliplierBackboard.ToString());
        rBackspinText.SetText("+ " + backspinHit.ToString() + " x " + defaultMultiplierBackspin.ToString());

        points.UpdateBackboard(backboardHit);
        points.UpdateBackspin(backspinHit);

        int coinValue = score + backboardHit * defaultMutliplierBackboard + backspinHit * defaultMultiplierBackspin;

        rCoinText.SetText(coinValue.ToString());
        points.UpdateCoins(coinValue);

        starsManager.ShowStars(coinValue);

    }

    public void ResetResultsTable()
    {
        rScoreText.SetText("+ 0");
        rBackboardText.SetText("+ 0" + " x 0");
        rBackspinText.SetText("+ 0" + " x 0");
        rCoinText.SetText("0");
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
