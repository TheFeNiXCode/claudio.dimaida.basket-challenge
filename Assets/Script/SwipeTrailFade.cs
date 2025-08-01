using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SwipeTrailFade : MonoBehaviour
{
    private TrailRenderer trail;
    private Coroutine fadeRoutine;
    private float originalTime;

    void Awake()
    {
        trail = GetComponent<TrailRenderer>();
        originalTime = trail.time; // tempo originale
    }

    public void StartFade()
    {
        if (fadeRoutine != null)
            StopCoroutine(fadeRoutine);

        fadeRoutine = StartCoroutine(FadeOut());
    }

    public IEnumerator FadeOut()
    {
        // Riduce gradualmente il tempo del trail (simula dissolvenza)
        float t = trail.time;
        while (t > 0f)
        {
            t -= Time.deltaTime;
            trail.time = Mathf.Max(0f, t);
            yield return null;
        }

        gameObject.SetActive(false);
        trail.time = originalTime; // reset per il prossimo swipe
    }
}
