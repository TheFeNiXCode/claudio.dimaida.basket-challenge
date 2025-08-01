using System.Collections;
using UnityEngine;
using TMPro;
using UnityEngine.UI;
using UnityEngine.Events;

public class TimerManager : MonoBehaviour
{
    [Header("Countdown iniziale")]
    [SerializeField] private TextMeshProUGUI countdownText;
    [SerializeField] private float countdownDuration = 3f; // 3, 2, 1
    [SerializeField] private Vector3 countdownStartScale = new Vector3(2f, 2f, 2f);
    [SerializeField] private Vector3 countdownEndScale = Vector3.one;
    [SerializeField] private float scaleAnimationSpeed = 0.2f;
    [SerializeField] private CanvasGroup countdownCanvasGroup;

    [Header("Timer partita")]
    [SerializeField] private TextMeshProUGUI gameTimerText;
    [SerializeField] private float matchDuration = 120f; // default 2 min
    [SerializeField] private bool autoStart = true;

    [Header("Player")]
    [SerializeField] private PlayerManager playerManager;

    [Header("Eventi di Gioco")]
    public UnityEvent OnGameStart;
    public UnityEvent OnGameEnd;
    public UnityEvent OnGamePaused;
    public UnityEvent OnGameResumed;

    public float currentMatchTime;
    private bool isGameRunning = false;
    private bool isPaused = false;

    private void Start()
    {

        AudioManager.Instance.PlayMusic(1, true, 1f);

        if (countdownText != null) countdownText.gameObject.SetActive(false);
        if (countdownCanvasGroup != null) countdownCanvasGroup.alpha = 0f;

        if (autoStart)
        {
            StartCoroutine(GameStartSequence());
        }
    }

    public void StartGameManually()
    {
        StartCoroutine(GameStartSequence());
    }

    private IEnumerator GameStartSequence()
    {
        if (playerManager != null)
            playerManager.DisableInput();

        countdownText.gameObject.SetActive(true);
        countdownCanvasGroup.gameObject.SetActive(true);
        countdownCanvasGroup.alpha = 1f;

        float countdown = countdownDuration;

        while (countdown > 0)
        {
            AudioManager.Instance.PlaySFX(7, 1f);

            countdownText.text = Mathf.Ceil(countdown).ToString();

            // Effetto scala con coroutine
            yield return StartCoroutine(AnimateScale(countdownText.transform, countdownStartScale, countdownEndScale, scaleAnimationSpeed));

            yield return new WaitForSeconds(1f);
            countdown -= 1f;
        }

        AudioManager.Instance.PlaySFX(2, 1f);

        // Mostra "GO!"
        countdownText.text = "GO!";
        yield return StartCoroutine(AnimateScale(countdownText.transform, countdownStartScale, countdownEndScale, scaleAnimationSpeed));

        yield return new WaitForSeconds(0.5f);
        yield return StartCoroutine(FadeOut(countdownCanvasGroup, 0.5f));
        countdownText.gameObject.SetActive(false);
        countdownCanvasGroup.gameObject.SetActive(false);

        // Avvia timer partita
        currentMatchTime = matchDuration;
        isGameRunning = true;

        if (playerManager != null)
            playerManager.EnableInput();

        OnGameStart?.Invoke();
    }

    private void Update()
    {
        if (isGameRunning && !isPaused)
        {
            currentMatchTime -= Time.deltaTime;

            if (currentMatchTime <= 0f)
            {
                currentMatchTime = 0f;
                AudioManager.Instance.PlaySFX(3, 0.5f);
                AudioManager.Instance.StopMusic();
                AudioManager.Instance.StopMusicSFXBack();

                StartCoroutine(WaitForShotAndEndGame());
            }

            UpdateTimerUI(currentMatchTime);
        }
    }

    private IEnumerator WaitForShotAndEndGame()
    {
        isGameRunning = false; // Ferma il timer

        //Se il giocatore sta ancora tirando, aspetta
        while (playerManager != null && playerManager.InShotInProgress())
        {
            yield return null;
        }
        EndGame();
    }


    private void UpdateTimerUI(float time)
    {
        int minutes = Mathf.FloorToInt(time / 60f);
        int seconds = Mathf.FloorToInt(time % 60f);
        gameTimerText.text = $"{minutes:00}:{seconds:00}";
    }

    private void EndGame()
    {
        isGameRunning = false;

        if (playerManager != null) {

            playerManager.DisableInput();
            
        }

        OnGameEnd?.Invoke();
    }

    public void PauseGame()
    {
        if (!isGameRunning || isPaused) return;
        isPaused = true;
        OnGamePaused?.Invoke();
    }

    public void ResumeGame()
    {
        if (!isPaused) return;
        isPaused = false;
        OnGameResumed?.Invoke();
    }

    private IEnumerator AnimateScale(Transform target, Vector3 from, Vector3 to, float duration)
    {
        float elapsed = 0f;
        target.localScale = from;
        while (elapsed < duration)
        {
            elapsed += Time.deltaTime;
            float t = elapsed / duration;
            target.localScale = Vector3.Lerp(from, to, t);
            yield return null;
        }
        target.localScale = to;
    }

    private IEnumerator FadeOut(CanvasGroup canvasGroup, float duration)
    {
        float elapsed = 0f;
        float startAlpha = canvasGroup.alpha;
        while (elapsed < duration)
        {
            elapsed += Time.deltaTime;
            canvasGroup.alpha = Mathf.Lerp(startAlpha, 0f, elapsed / duration);
            yield return null;
        }
        canvasGroup.alpha = 0f;
    }
}
