using UnityEngine;
using TMPro;

public class ScoreManager : MonoBehaviour
{
    public static ScoreManager Instance;

    public int score = 0;
    public TextMeshProUGUI scoreText;

    private static int persistedScore = 0;

    public static void ResetScore() => persistedScore = 0;

    void Awake()
    {
        Instance = this;
        score = persistedScore;
    }

    void Start()
    {
        scoreText.text = "x " + score;
    }

    public void AddScore(int amount)
    {
        score += amount;
        persistedScore = score;
        scoreText.text = "x " + score;
    }
}