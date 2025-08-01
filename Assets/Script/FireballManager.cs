using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

public class FireballManager : MonoBehaviour
{
    [Header("UI")]
    public Slider fireballSlider;

    [Header("Settings")]
    public float maxValue = 100f;
    public float pointsPerScore = 15f;
    public float fireballDuration = 10f;

    [Header("Status")]
    public bool isFireballActive = false;
    private float currentValue = 0f;

    [Header("Fire Events")]
    public UnityEvent FireballModeEnabled;
    public UnityEvent FireballModeDisabled;

    private Coroutine fireballCoroutine;

    void Start()
    {
        fireballSlider.maxValue = maxValue;
        fireballSlider.value = currentValue;
    }

    public void AddScore()
    {
        if (isFireballActive) return;

        currentValue += pointsPerScore;
        currentValue = Mathf.Clamp(currentValue, 0f, maxValue);
        fireballSlider.value = currentValue;

        if (currentValue >= maxValue)
        {
            ActivateFireball();
        }
    }

    public void OnMissedShot()
    {
        if (!isFireballActive) return;

        // Stop il countdown se attivo
        if (fireballCoroutine != null)
            StopCoroutine(fireballCoroutine);

        DeactivateFireball();
    }

    private void ActivateFireball()
    {
        if (isFireballActive) return;

        AudioManager.Instance.PlayMusicSFXBack(6, true, 1f);

        isFireballActive = true;
        FireballModeEnabled?.Invoke();
        fireballCoroutine = StartCoroutine(FireballCountdown());
    }

    private void DeactivateFireball()
    {
        AudioManager.Instance.StopMusicSFXBack();
        currentValue = 0f;
        fireballSlider.value = currentValue;
        isFireballActive = false;
        FireballModeDisabled?.Invoke();
    }


    private IEnumerator FireballCountdown()
    {
        float elapsed = 0f;

        while (elapsed < fireballDuration)
        {
            elapsed += Time.deltaTime;
            float t = elapsed / fireballDuration;
            fireballSlider.value = Mathf.Lerp(maxValue, 0f, t);
            yield return null;
        }

        DeactivateFireball();
    }
}
