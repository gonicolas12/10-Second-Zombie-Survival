using UnityEngine;
using TMPro;

public class ScoreManager : MonoBehaviour
{
    [Header("UI References")]
    [SerializeField] private TextMeshProUGUI scoreText;
    [SerializeField] private TextMeshProUGUI finalScoreText;
    [SerializeField] private TextMeshProUGUI highScoreText;

    private int currentScore = 0;
    private int highScore = 0;
    private static ScoreManager instance;
    private const string HIGH_SCORE_KEY = "HighScore"; // Clé pour sauvegarder le meilleur score

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
        // Charge le meilleur score au démarrage
        highScore = PlayerPrefs.GetInt(HIGH_SCORE_KEY, 0);
    }

    private void Start()
    {
        UpdateScoreUI();
        // Cache les textes de fin de partie au démarrage
        if (finalScoreText != null)
        {
            finalScoreText.gameObject.SetActive(false);
        }
        if (highScoreText != null)
        {
            highScoreText.gameObject.SetActive(false);
        }
    }

    public void AddScore(int points)
    {
        currentScore += points;
        UpdateScoreUI();
    }

    private void UpdateScoreUI()
    {
        // Met à jour le score affiché
        if (scoreText != null)
        {
            scoreText.text = currentScore.ToString();
            scoreText.color = Color.red;
        }
    }

    public void ShowFinalScore()
    {
        // Affiche le score final
        if (finalScoreText != null)
        {
            finalScoreText.gameObject.SetActive(true);
            finalScoreText.text = $"Votre score : {currentScore}";
            finalScoreText.color = Color.red;
        }

        // Vérifie si c'est un nouveau meilleur score
        if (currentScore > highScore)
        {
            highScore = currentScore;
            PlayerPrefs.SetInt(HIGH_SCORE_KEY, highScore);
            PlayerPrefs.Save();
        }

        // Affiche le meilleur score
        if (highScoreText != null)
        {
            highScoreText.gameObject.SetActive(true);
            highScoreText.text = $"Meilleur score : {highScore}";
            highScoreText.color = Color.red;
        }
    }
}