using UnityEngine;
using TMPro;

public class ScoreManager : MonoBehaviour
{
    [Header("UI References")]
    [SerializeField] private TextMeshProUGUI scoreText;
    [SerializeField] private TextMeshProUGUI finalScoreText;

    private int currentScore = 0;
    private static ScoreManager instance;

    public static ScoreManager Instance
    {
        get { return instance; }
    }

    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
        }
    }

    private void Start()
    {
        UpdateScoreUI();

        // Cache le texte de score final au démarrage
        if (finalScoreText != null)
        {
            finalScoreText.gameObject.SetActive(false);
        }
    }

    public void AddScore(int points)
    {
        currentScore += points;
        UpdateScoreUI();
    }

    private void UpdateScoreUI()
    {
        if (scoreText != null)
        {
            scoreText.text = currentScore.ToString();
            scoreText.color = Color.red;
        }
    }

    public void ShowFinalScore()
    {
        if (finalScoreText != null)
        {
            finalScoreText.gameObject.SetActive(true);
            finalScoreText.text = $"Votre score : {currentScore}";
            finalScoreText.color = Color.red;
        }
    }
}