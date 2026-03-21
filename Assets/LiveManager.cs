using UnityEngine;
using TMPro;
using UnityEngine.SceneManagement;
using System.Collections;

public class LiveManager : MonoBehaviour
{
    public static LiveManager Instance;

    public int lives = 3;
    public TextMeshProUGUI livesText;

    public GameObject gameOverText; // ✅ assign in inspector
    public float gameOverDelay = 2f;

    private bool isGameOver = false;

    void Awake()
    {
        Instance = this;
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
        UpdateUI();

        if (lives <= 0)
        {
            GameOver();
        }
    }

    void UpdateUI()
    {
        livesText.text = "Lives: " + lives;
    }

    void GameOver()
    {
        isGameOver = true;

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