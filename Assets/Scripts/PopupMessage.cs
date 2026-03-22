using UnityEngine;
using TMPro;
using System.Collections;

public class PopupMessage : MonoBehaviour
{
    public TextMeshProUGUI message;
    public float fadeInDuration = 0.6f;
    public float scaleUpDuration = 0.6f;
    public float holdDuration = 2f;
    public float fadeOutDuration = 0.6f;

    private Vector3 startScale;

    void Start()
    {
        startScale = message.transform.localScale;
        StartCoroutine(Animate());
    }

    IEnumerator Animate()
    {
        // init
        message.transform.localScale = startScale * 0.5f;
        SetAlpha(0f);

        // fade in + scale up
        float t = 0f;
        while (t < 1f)
        {
            t += Time.deltaTime / fadeInDuration;
            float eased = Mathf.SmoothStep(0f, 1f, t);
            SetAlpha(eased);
            message.transform.localScale = Vector3.Lerp(startScale * 0.5f, startScale, eased);
            yield return null;
        }

        SetAlpha(1f);
        message.transform.localScale = startScale;

        // hold
        yield return new WaitForSeconds(holdDuration);

        // fade out
        t = 0f;
        while (t < 1f)
        {
            t += Time.deltaTime / fadeOutDuration;
            SetAlpha(1f - Mathf.SmoothStep(0f, 1f, t));
            yield return null;
        }

        gameObject.SetActive(false);
    }

    void SetAlpha(float alpha)
    {
        Color c = message.color;
        c.a = alpha;
        message.color = c;
    }
}
