using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEngine.SceneManagement;

public class PlayerHealth : MonoBehaviour
{
    [Header("Health Parameters")]
    [SerializeField] private float maxHealth = 100f;
    [SerializeField] private float currentHealth;

    [Header("UI Elements")]
    [SerializeField] private Slider healthBar;
    [SerializeField] private Image fillImage;
    [SerializeField] private TextMeshProUGUI gameOverText;
    [SerializeField] private Button replayButton;
    [SerializeField] private Button quitButton;

    private bool isDead = false;

    private void RestartGame()
    {
        Time.timeScale = 1f;
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }

    private void QuitGame()
    {
        #if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
        #else
        Application.Quit();
        #endif
    }

    private void Start()
    {
        currentHealth = maxHealth;

        // Vérifie si les références UI sont assignées
        if (healthBar == null)
        {
            Debug.LogError("Health Bar n'est pas assignée au PlayerHealth!");
        }
        if (fillImage == null)
        {
            Debug.LogError("Fill Image n'est pas assignée au PlayerHealth!");
        }

        // Cache le texte Game Over et les boutons au démarrage
        if (gameOverText != null)
        {
            gameOverText.gameObject.SetActive(false);
        }
        if (replayButton != null)
        {
            replayButton.gameObject.SetActive(false);
            replayButton.onClick.AddListener(RestartGame);
        }
        if (quitButton != null)
        {
            quitButton.gameObject.SetActive(false);
            quitButton.onClick.AddListener(QuitGame);
        }

        UpdateHealthBar();
    }

    public void TakeDamage(float damage)
    {
        // Vérifie si le joueur est mort
        if (isDead) return;

        currentHealth = Mathf.Max(0f, currentHealth - damage);
        UpdateHealthBar();

        if (currentHealth <= 0f && !isDead)
        {
            Die();
        }
    }

    private void UpdateHealthBar()
    {
        if (healthBar != null)
        {
            healthBar.value = currentHealth / maxHealth;

            // Update la couleur de la barre de vie
            if (fillImage != null)
            {
                Color healthColor = Color.Lerp(Color.red, Color.green, currentHealth / maxHealth);
                fillImage.color = healthColor;
            }
        }
    }

    private void Die()
    {
        isDead = true;
        Time.timeScale = 0f;

        // Affiche le texte Game Over et les boutons
        if (gameOverText != null)
        {
            gameOverText.gameObject.SetActive(true);
            ScoreManager.Instance?.ShowFinalScore();
        }
        if (replayButton != null)
        {
            replayButton.gameObject.SetActive(true);
        }
        if (quitButton != null)
        {
            quitButton.gameObject.SetActive(true);
        }

        // Désactive le PlayerController
        PlayerController playerController = GetComponent<PlayerController>();
        if (playerController != null)
        {
            playerController.enabled = false;
        }

        // Désactive le Rigidbody2D
        Rigidbody2D rb = GetComponent<Rigidbody2D>();
        if (rb != null)
        {
            rb.simulated = false;
        }
    }
}