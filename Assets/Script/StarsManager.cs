using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class StarsManager : MonoBehaviour
{
    [Header("Stars Settings")]
    [SerializeField] private Image[] stars;
    [SerializeField] private float initialDelay = 0.2f;

    [SerializeField] private float delayBetweenStars = 0.5f;
    [SerializeField] private float popDuration = 0.3f;
    [SerializeField] private Vector3 popScale = new Vector3(1.5f, 1.5f, 1f);
    [SerializeField] private Vector3 originalScale = Vector3.one;

    [Header("Coin Thresholds")]
    [SerializeField] private int[] thresholds = { 10, 20, 30 };

    /*
    [Header("Audio")]
    [SerializeField] private AudioSource audioSource;
    [SerializeField] private AudioClip starSound;
    */

    public void ShowStars(int coinValue)
    {
        // Disattiva tutte le stelle e resetta la scala
        foreach (var star in stars)
        {
            star.gameObject.SetActive(false);
            star.rectTransform.localScale = Vector3.zero;
        }

        StartCoroutine(AnimateStars(coinValue));
    }

    private IEnumerator AnimateStars(int coinValue)
    {
        int starsToShow = 0;

        // Calcola quante stelle mostrare
        for (int i = 0; i < thresholds.Length; i++)
        {
            if (coinValue >= thresholds[i])
                starsToShow++;
        }

        if (starsToShow == 0)
            yield break;

        if (initialDelay > 0f)
            yield return new WaitForSeconds(initialDelay);

        // Mostra e anima le stelle
        for (int i = 0; i < starsToShow; i++)
        {
            stars[i].gameObject.SetActive(true);

            /* Play sound
            if (audioSource != null && starSound != null)
            {
                audioSource.PlayOneShot(starSound);
            }*/

            yield return StartCoroutine(PopIn(stars[i].rectTransform));
            yield return new WaitForSeconds(delayBetweenStars);
        }
    }

    private IEnumerator PopIn(RectTransform target)
    {
        // da 0 a popScale
        float elapsed = 0f;
        target.localScale = Vector3.zero;

        while (elapsed < popDuration)
        {
            elapsed += Time.deltaTime;
            float t = elapsed / popDuration;
            target.localScale = Vector3.Lerp(Vector3.zero, popScale, t);
            yield return null;
        }

        // da popScale a originalScale
        elapsed = 0f;
        Vector3 startScale = target.localScale;
        float bounceDuration = popDuration * 0.5f;

        while (elapsed < bounceDuration)
        {
            elapsed += Time.deltaTime;
            float t = elapsed / bounceDuration;
            target.localScale = Vector3.Lerp(startScale, originalScale, t);
            yield return null;
        }

        target.localScale = originalScale;
    }
}
