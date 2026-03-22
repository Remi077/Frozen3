using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using System.Collections;

public class TimerGauge : MonoBehaviour
{
    [Header("Timer")]
    public float duration = 60f;
    public string nextScene = "WinScene";

    [Header("UI")]
    public Slider gauge;               // Slider UI — la fill bar rétrécit automatiquement
    public GameObject reussiteText;    // TextMeshPro ou Text "Reussite"

    private float timeLeft;
    private bool finished = false;

    void Start()
    {
        timeLeft = duration;

        if (gauge != null)
        {
            gauge.minValue = 0f;
            gauge.maxValue = duration;
            gauge.value = duration;
        }

        if (reussiteText != null)
            reussiteText.SetActive(false);
    }

    void Update()
    {
        if (finished) return;

        timeLeft -= Time.deltaTime;
        timeLeft = Mathf.Max(timeLeft, 0f);

        if (gauge != null)
            gauge.value = timeLeft;

        if (timeLeft <= 0f)
            StartCoroutine(OnTimerEnd());
    }

    IEnumerator OnTimerEnd()
    {
        finished = true;

        SeaScroller.Speed = 0f;

        if (reussiteText != null)
            reussiteText.SetActive(true);

        yield return new WaitForSeconds(2f);

        SceneManager.LoadScene(nextScene);
    }
}
