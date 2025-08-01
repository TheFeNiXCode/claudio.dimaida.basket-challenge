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
    [SerializeField] private Color newBestScoreColor = Color.yellow;
    [SerializeField] private int defaultScoreToAdd = 10;
    [SerializeField] private int defaultBackspinScoreToAdd = 10;
    [SerializeField] private int defaultMultiplierBackspin = 5;
    [SerializeField] private int defaultMutliplierBackboard = 10;
    [SerializeField] private int defaultMutliplierScore = 2;
    [SerializeField] PlayerSettings settings;


    [Header("ResultsUI")]
    [SerializeField] private GameObject resultsPanel;
    [SerializeField] private TMP_Text rScoreText;
    [SerializeField] private TMP_Text rBackboardText;
    [SerializeField] private TMP_Text rBackspinText;
    [SerializeField] private TMP_Text rCoinText;
    [SerializeField] private StarsManager starsManager;

    [Header("ResultsUI_CPU")]
    [SerializeField] private GameObject resultsPanelCPU;
    [SerializeField] private TMP_Text rScoreTextCPU;
    [SerializeField] private TMP_Text rBackboardTextCPU;
    [SerializeField] private TMP_Text rBackspinTextCPU;
    [SerializeField] private TMP_Text rCoinTextCPU;
    [SerializeField] private TMP_Text rWinLoseTextCPU;
    [SerializeField] private Color winColor = Color.green;
    [SerializeField] private Color loseColor = Color.red;
    [SerializeField] private CPUBattleManager cPUBattleManager;


    [Header("Score Flyers")]
    [SerializeField] private TMP_Text flyerText;
    [SerializeField] private float flyerMoveUp = 1f;
    [SerializeField] private float flyerMoveDuration = 1f;
    [SerializeField] private float popScale = 1.3f;
    [SerializeField] private float popDuration = 0.15f;

    [SerializeField] private Color lowScoreColor = Color.green;   // <= threshold1
    [SerializeField] private Color midScoreColor = new Color(1f, 0.5f, 0f); // arancione
    [SerializeField] private Color highScoreColor = Color.blue;   // >= threshold2
    [SerializeField] private Color extremeScoreColor = new Color(0.5f, 0f, 0.5f); // viola

    [SerializeField] private List<int> threshold; // <=2 verde, <=5 arancione, >5 && <8 blu, >=8 viola

    private Vector3 flyerStartPos;
    private bool isAnimating = false;

    public int score { get; private set; }
    public int shotScore { get; private set; }
    public int backboardHit { get; private set; }
    public int backspinHit { get; private set; }

    void Start()
    {
        if (resultsPanel != null) resultsPanel.SetActive(false);
        if (resultsPanelCPU != null) resultsPanelCPU.SetActive(false);

        setIntialValue();
        InitialFlyerPosition();
    }

    private void InitialFlyerPosition()
    {
        // Salviamo la posizione originale del flyer
        if (flyerText != null)
        {
            flyerStartPos = flyerText.transform.position;
            flyerText.gameObject.SetActive(false);
        }
    }

    public void UpdateScore(PlayerManager player)
    {
        shotScore += defaultScoreToAdd;
        
        if (player.isThisShotABackspin) {

            shotScore += defaultBackspinScoreToAdd;
            backspinHit += 1;
        }
        shotScore += backboard.AddBonusPoints();

        score += shotScore;
        if (score > points.BestScore()) scoreText.color = newBestScoreColor;
        scoreText.SetText(score.ToString());

        if (points.checkIfUpdateSpriteScore(score))
        {
            spriteScore.sprite = points.UpdateGameScoreSprite(score);
            StartCoroutine(PopAnimation(spriteScore.rectTransform));
        }

        // Mostra il flyer se disponibile
        if (flyerText != null)
            ShowFlyer("+ " + shotScore, shotScore);

        //reset del punteggio del singolo canestro
        shotScore = 0;
    }

    private void ShowFlyer(string text, int scoreValue)
    {
        if (isAnimating) return;

        flyerText.text = text;
        flyerText.color = GetFlyerColor(scoreValue);
        flyerText.gameObject.SetActive(true);
        flyerText.gameObject.GetComponent<LookAtCamera>().isActive = true;

        StartCoroutine(AnimateFlyer());
    }

    private Color GetFlyerColor(int scoreValue)
    {
        if (scoreValue <= threshold[0])
            return lowScoreColor;
        else if (scoreValue > threshold[0] && scoreValue <= threshold[1])
            return midScoreColor;
        else if (scoreValue > threshold[1] && scoreValue < threshold[2])
            return highScoreColor;
        else
            return extremeScoreColor;
    }

    private IEnumerator AnimateFlyer()
    {
        isAnimating = true;

        Vector3 startPos = flyerText.rectTransform.localPosition;
        Vector3 endPos = startPos + Vector3.up * flyerMoveUp;
        Color startColor = flyerText.color;
        startColor.a = 1f;
        flyerText.color = startColor;

        Vector3 originalScale = flyerText.rectTransform.localScale;
        Vector3 targetScale = originalScale * popScale;

        float t = 0f;
        while (t < popDuration)
        {
            t += Time.deltaTime;
            flyerText.rectTransform.localScale = Vector3.Lerp(originalScale, targetScale, t / popDuration);
            yield return null;
        }

        t = 0f;
        while (t < popDuration)
        {
            t += Time.deltaTime;
            flyerText.rectTransform.localScale = Vector3.Lerp(targetScale, originalScale, t / popDuration);
            yield return null;
        }

        t = 0f;
        while (t < flyerMoveDuration)
        {
            t += Time.deltaTime;
            float progress = t / flyerMoveDuration;

            flyerText.rectTransform.localPosition = Vector3.Lerp(startPos, endPos, progress);
            flyerText.color = new Color(startColor.r, startColor.g, startColor.b, 1f - progress);

            yield return null;
        }

        flyerText.rectTransform.localPosition = startPos;
        flyerText.color = new Color(startColor.r, startColor.g, startColor.b, 0f);
        flyerText.gameObject.SetActive(false);
        flyerText.gameObject.GetComponent<LookAtCamera>().isActive = false;

        isAnimating = false;
    }

    public int getScore() => score;
    public int getBackboard() => backboardHit;
    public int getBackspin() => backspinHit;

    private void setIntialValue()
    {
        shotScore = 0;
        score = 0;
        backboardHit = 0;
        backspinHit = 0;
    }

    public void AddBackboardHit()
    {
        backboardHit += 1;
    }

    public void UpdateResult()
    {
        if (settings.IsVSCPU())
        {
            if (resultsPanelCPU != null)
                resultsPanelCPU.SetActive(true);
            UpdateResultsTableCPU();
        }
        else {
            if (resultsPanel != null)
                resultsPanel.SetActive(true);
            UpdateResultsTable();
        } 
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

    public void UpdateResultsTableCPU()
    {
        points.UpdateBackboard(backboardHit);
        points.UpdateBackspin(backspinHit);

        int coinValue = score + backboardHit * defaultMutliplierBackboard + backspinHit * defaultMultiplierBackspin;

        if (score < cPUBattleManager.GetScoreCPU())
        {
            rWinLoseTextCPU.SetText("YOU LOSE");
            rWinLoseTextCPU.color = loseColor;
            points.UpdateLose();
            rScoreTextCPU.SetText("+ " + score.ToString());
            coinValue = score + backboardHit * defaultMutliplierBackboard + backspinHit * defaultMultiplierBackspin;
        }
        else if(score > cPUBattleManager.GetScoreCPU())
        {
            rWinLoseTextCPU.SetText("YOU WIN");
            rWinLoseTextCPU.color = winColor;
            points.UpdateWin();

            switch (settings.getGameDifficult())
            {
                case 0:
                    defaultMutliplierScore = 2;
                    break;
                case 1:
                    defaultMutliplierScore = 3;
                    break;
                case 2:
                    defaultMutliplierScore = 5;
                    break;
                default:
                    defaultMutliplierScore = 2;
                    break;
            }

            rScoreTextCPU.SetText("+ " + score.ToString() + " x " + defaultMutliplierScore.ToString());
            coinValue = score * defaultMutliplierScore + backboardHit * defaultMutliplierBackboard + backspinHit * defaultMultiplierBackspin;
        }
        else
        {
            rWinLoseTextCPU.SetText("DRAW");
            points.UpdateDraw();
            rScoreTextCPU.SetText("+ " + score.ToString());
            coinValue = score + backboardHit * defaultMutliplierBackboard + backspinHit * defaultMultiplierBackspin;
        }

        rBackboardTextCPU.SetText("+ " + backboardHit.ToString() + " x " + defaultMutliplierBackboard.ToString());
        rBackspinTextCPU.SetText("+ " + backspinHit.ToString() + " x " + defaultMultiplierBackspin.ToString());
        rCoinTextCPU.SetText(coinValue.ToString());
        points.UpdateCoins(coinValue);

        starsManager.ShowStarsCPU(coinValue);
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
