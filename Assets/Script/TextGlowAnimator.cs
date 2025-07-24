using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class TextGlowAnimator : MonoBehaviour
{
    [Header("Text")]
    [SerializeField] private TMP_Text textMeshPro;

    [Header("Settings")]
    [SerializeField] private Color startGlowColor = Color.white;
    [SerializeField] private Color targetGlowColor = Color.yellow;
    [SerializeField] private float startGlowPower = 0.3f;
    [SerializeField] private float targetGlowPower = 1.0f;
    [SerializeField] private float glowDuration = 1.5f; // durata di un ciclo PingPong

    [Header("Text Options")]
    [SerializeField] private string[] texts = { "BONUS\n+4 POINTS", "BONUS\n+6 POINTS", "BONUS\n+8 POINTS" };
    [SerializeField] private Color[] textColors = { Color.green, Color.blue, Color.magenta }; // Colori associati ai testi
    [SerializeField, Range(0, 2)] private int selectedIndex = 0; // Indice del testo da mostrare

    private Coroutine glowCoroutine;

    public bool isAnimationActive = false;

    private void Reset()
    {
        textMeshPro = GetComponent<TMP_Text>();
    }

    public void StartEffect(int index)
    {
        StopEffect(); // Interrompe eventuali animazioni attive

        if (textMeshPro != null)
        {
            // Imposta il testo e il colore vertex in base all'indice
            selectedIndex = index;
            textMeshPro.text = GetSelectedText(selectedIndex);
            textMeshPro.color = GetSelectedColor(selectedIndex);
        }
        isAnimationActive = true;
        glowCoroutine = StartCoroutine(AnimateGlowCoroutine());
    }

    private IEnumerator AnimateGlowCoroutine()
    {
        if (textMeshPro == null)
        {
            Debug.LogWarning("TextMeshPro non assegnato!");
            yield break;
        }

        Material mat = textMeshPro.fontMaterial;
        float elapsed = 0f;

        while (true)
        {
            elapsed += Time.deltaTime;
            float t = Mathf.PingPong(elapsed / glowDuration, 1f);

            // Colore Glow
            Color currentGlow = Color.Lerp(startGlowColor, targetGlowColor, t);
            mat.SetColor(ShaderUtilities.ID_GlowColor, currentGlow);

            // Intensità Glow
            float currentPower = Mathf.Lerp(startGlowPower, targetGlowPower, t);
            mat.SetFloat(ShaderUtilities.ID_GlowPower, currentPower);

            yield return null;
        }
    }

    public void StopEffect()
    {
        if (glowCoroutine != null)
        {
            isAnimationActive = false;
            StopCoroutine(glowCoroutine);
            glowCoroutine = null;
        }

        if (textMeshPro != null)
        {
            Material mat = textMeshPro.fontMaterial;
            mat.SetColor(ShaderUtilities.ID_GlowColor, startGlowColor);
            mat.SetFloat(ShaderUtilities.ID_GlowPower, startGlowPower);
        }
    }

    public void SetTextIndex(int index)
    {
        if (index >= 0 && index < texts.Length)
        {
            selectedIndex = index;
            if (textMeshPro != null)
            {
                textMeshPro.text = GetSelectedText(selectedIndex);
                textMeshPro.color = GetSelectedColor(selectedIndex);
            }
        }
    }

    private string GetSelectedText(int index)
    {
        return texts[Mathf.Clamp(index, 0, texts.Length - 1)];
    }

    private Color GetSelectedColor(int index)
    {
        return textColors[Mathf.Clamp(index, 0, textColors.Length - 1)];
    }
}
