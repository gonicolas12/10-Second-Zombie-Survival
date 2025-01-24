using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class PlayerHealth : MonoBehaviour
{
    [Header("Health Parameters")]
    [SerializeField] private float maxHealth = 100f;
    [SerializeField] private float currentHealth;

    [Header("UI Elements")]
    [SerializeField] private Slider healthBar;
    [SerializeField] private Image fillImage;
    [SerializeField] private TextMeshProUGUI gameOverText;

    private bool isDead = false;

    private void Start()
    {
        currentHealth = maxHealth;

        // V�rifie si les r�f�rences UI sont assign�es
        if (healthBar == null)
        {
            Debug.LogError("Health Bar n'est pas assign�e au PlayerHealth!");
        }

        if (fillImage == null)
        {
            Debug.LogError("Fill Image n'est pas assign�e au PlayerHealth!");
        }

        // Cache le texte Game Over au d�marrage
        if (gameOverText != null)
        {
            gameOverText.gameObject.SetActive(false);
        }

        UpdateHealthBar();
    }

    public void TakeDamage(float damage)
    {
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
        // Arr�te le jeu
        Time.timeScale = 0f;

        // Affiche le texte Game Over
        if (gameOverText != null)
        {
            gameOverText.gameObject.SetActive(true);
        }

        // D�sactive le PlayerController
        PlayerController playerController = GetComponent<PlayerController>();
        if (playerController != null)
        {
            playerController.enabled = false;
        }

        // D�sactive le Rigidbody2D
        Rigidbody2D rb = GetComponent<Rigidbody2D>();
        if (rb != null)
        {
            rb.simulated = false;
        }
    }
}