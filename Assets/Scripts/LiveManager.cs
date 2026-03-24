using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections;

public class LiveManager : MonoBehaviour
{
    public static LiveManager Instance;

    public int lives = 3;
    public GameObject[] lifeIcons; // assign 3 icon GameObjects in Inspector

    public GameObject gameOverText; // ✅ assign in inspector
    public float gameOverDelay = 2f;

    private bool isGameOver = false;

    private static int persistedLives = 3;
    private static readonly int defaultLives = 3;

    private Vector3[] iconInitialScales;

    public static void ResetLives() => persistedLives = defaultLives;

    void Awake()
    {
        Instance = this;
        lives = persistedLives;

        iconInitialScales = new Vector3[lifeIcons.Length];
        for (int i = 0; i < lifeIcons.Length; i++)
            iconInitialScales[i] = lifeIcons[i].transform.localScale;
    }

    void Start()
    {
        UpdateUI();
        gameOverText.SetActive(false); // hide at start
    }

    public void removeLife(int amount)
    {
        if (isGameOver) return;

        lives -= amount;
        persistedLives = lives;
        UpdateUI();

        if (lives <= 0)
        {
            GameOver();
        }
    }

    public float iconShrinkDuration = 0.3f;

    void UpdateUI()
    {
        for (int i = 0; i < lifeIcons.Length; i++)
        {
            if (i < lives)
            {
                lifeIcons[i].SetActive(true);
                lifeIcons[i].transform.localScale = iconInitialScales[i];
            }
            else if (lifeIcons[i].activeSelf)
            {
                StartCoroutine(ShrinkAndHide(lifeIcons[i]));
            }
        }
    }

    IEnumerator ShrinkAndHide(GameObject icon)
    {
        int index = System.Array.IndexOf(lifeIcons, icon);
        Vector3 initialScale = iconInitialScales[index];
        float t = 0f;
        while (t < iconShrinkDuration)
        {
            t += Time.unscaledDeltaTime;
            icon.transform.localScale = Vector3.Lerp(initialScale, Vector3.zero, t / iconShrinkDuration);
            yield return null;
        }
        icon.SetActive(false);
        icon.transform.localScale = initialScale;
    }

    void GameOver()
    {
        isGameOver = true;

        ResetLives();
        ScoreManager.ResetScore();

        Debug.Log("GAME OVER");

        gameOverText.SetActive(true);

        // stop time (optional but nice)
        Time.timeScale = 0f;

        StartCoroutine(ReturnToMenu());
        // return to menu after delay (real-time, not affected by timeScale)
        // Invoke(nameof(LoadMenu), gameOverDelay);
    }

    void LoadMenu()
    {
        Time.timeScale = 1f; // restore time
        SceneManager.LoadScene("StartMenu"); // your menu scene name
    }

    IEnumerator ReturnToMenu()
    {
        yield return new WaitForSecondsRealtime(gameOverDelay);

        Time.timeScale = 1f;
        SceneManager.LoadScene("StartMenu");
    }

}