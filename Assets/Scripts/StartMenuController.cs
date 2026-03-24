using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using System.Collections;

public class StartMenuController : MonoBehaviour
{
    public RectTransform startImage;    // parent RectTransform that scales
    public Image baseLetters;           // normal version of the letters
    public Image highlightLetters;      // highlight version of the letters

    public Image shineOverlay;          // optional white flash overlay

    public float popScale = 1.4f;
    public float popDuration = 0.12f;
    public float shineDuration = 0.25f;

    private bool clicked = false;

    void Start()
    {
        SetLetterAlphas(1f, 0f);
    }

    public void PlayGame()
    {
        if (clicked) return;
        clicked = true;
        StartCoroutine(PopAndLoad());
    }

    void SetLetterAlphas(float baseAlpha, float highlightAlpha)
    {
        if (baseLetters != null)
        {
            Color c = baseLetters.color;
            c.a = baseAlpha;
            baseLetters.color = c;
        }
        if (highlightLetters != null)
        {
            Color c = highlightLetters.color;
            c.a = highlightAlpha;
            highlightLetters.color = c;
        }
    }

    IEnumerator PopAndLoad()
    {
        Vector3 originalScale = startImage.localScale;

        // Pop up — base fades out, highlight fades in
        float t = 0f;
        while (t < popDuration)
        {
            t += Time.unscaledDeltaTime;
            float p = t / popDuration;
            startImage.localScale = originalScale * Mathf.Lerp(1f, popScale, p);
            SetLetterAlphas(1f - p, p);
            yield return null;
        }
        startImage.localScale = originalScale * popScale;
        SetLetterAlphas(0f, 1f);

        // Pop back down — highlight fades out, base fades back in
        t = 0f;
        while (t < popDuration)
        {
            t += Time.unscaledDeltaTime;
            float p = t / popDuration;
            startImage.localScale = originalScale * Mathf.Lerp(popScale, 1f, p);
            SetLetterAlphas(p, 1f - p);
            yield return null;
        }
        startImage.localScale = originalScale;
        SetLetterAlphas(1f, 0f);

        // Shine flash
        if (shineOverlay != null)
        {
            shineOverlay.gameObject.SetActive(true);
            t = 0f;
            while (t < shineDuration)
            {
                t += Time.unscaledDeltaTime;
                float alpha = Mathf.Lerp(0.8f, 0f, t / shineDuration);
                shineOverlay.color = new Color(1f, 1f, 1f, alpha);
                yield return null;
            }
            shineOverlay.gameObject.SetActive(false);
        }

        SceneManager.LoadScene("SeaScroll");
    }
}
