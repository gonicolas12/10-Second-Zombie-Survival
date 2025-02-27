using UnityEngine;
using UnityEngine.SceneManagement;

public class ZombieController : MonoBehaviour
{
    [Header("Zombie Parameters")]
    public float moveSpeed = 3f;
    public int maxHealth = 3;
    public float avoidanceForce = 5f;

    [Header("Damage Parameters")]
    public float damagePerSecond = 20f;
    public float damageTickRate = 1f;

    private int currentHealth;
    private Transform playerTransform;
    private PlayerHealth playerHealth;
    private Rigidbody2D rb;
    private CapsuleCollider2D col;
    private SpriteRenderer spriteRenderer;
    private LayerMask groundLayer;
    private bool isDead = false;
    private bool isFlashing = false;
    private float nextDamageTime;
    private ZombieSpawnManager spawnManager;

    private void Awake()
    {
        // Initialisation des composants dans Awake plutôt que Start
        rb = GetComponent<Rigidbody2D>();
        col = GetComponent<CapsuleCollider2D>();
        spriteRenderer = GetComponent<SpriteRenderer>();
        groundLayer = LayerMask.GetMask("Ground");
    }

    private void Start()
    {
        currentHealth = maxHealth;
        playerTransform = GameObject.FindGameObjectWithTag("Player")?.transform;

        // Récupérer le PlayerHealth du joueur
        if (playerTransform != null)
        {
            playerHealth = playerTransform.GetComponent<PlayerHealth>();
        }
        // Démarrer la coroutine de dégâts
        if (rb != null)
        {
            rb.gravityScale = 1f;
            rb.constraints = RigidbodyConstraints2D.FreezeRotation;
        }
    }

    private void OnCollisionStay2D(Collision2D collision)
    {
        if (isDead || playerHealth == null) return;

        // Appliquer des dégâts au joueur
        if (collision.gameObject.CompareTag("Player") && Time.time >= nextDamageTime)
        {
            playerHealth.TakeDamage(damagePerSecond * Time.fixedDeltaTime);
            nextDamageTime = Time.time + Time.fixedDeltaTime;
        }
    }

    private void FixedUpdate()
    {
        if (isDead || rb == null || playerTransform == null) return; // Vérifie si le joueur est mort

        if (IsOnGround())
        {
            MoveTowardsPlayer();
        }
    }

    // Déplacement du Zombie vers le joueur
    private void MoveTowardsPlayer()
    {
        if (isDead) return;

        Vector2 direction = (playerTransform.position - transform.position).normalized;
        if (rb != null)
        {
            rb.linearVelocity = new Vector2(direction.x * moveSpeed, rb.linearVelocity.y);
            transform.localScale = new Vector3(direction.x > 0 ? 1 : -1, 1, 1);
        }
    }

    // Vérifie si le Zombie est au sol
    private bool IsOnGround()
    {
        if (col == null) return false;

        float extraHeight = 0.1f;
        RaycastHit2D hit = Physics2D.Raycast(
            col.bounds.center,
            Vector2.down,
            col.bounds.extents.y + extraHeight,
            groundLayer
        );
        return hit.collider != null;
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (isDead || other == null) return;

        if (other.CompareTag("Bullet"))
        {
            TakeDamage(1);

            // Détruire la balle si elle existe encore
            if (other != null && other.gameObject != null)
            {
                Destroy(other.gameObject);
            }
        }
    }

    public void TakeDamage(int damage)
    {
        if (isDead || gameObject == null) return;

        currentHealth -= damage;

        if (currentHealth <= 0 && !isDead)
        {
            Die();
        }
        else if (!isFlashing && spriteRenderer != null)
        {
            Flash();
        }
    }

    // Flashe le Zombie en rouge
    private void Flash()
    {
        if (isDead || spriteRenderer == null || !gameObject.activeInHierarchy) return;

        isFlashing = true;
        spriteRenderer.color = Color.red;

        // Utiliser un délai plus court
        Invoke(nameof(ResetColor), 0.05f);
    }

    private void ResetColor()
    {
        if (isDead || spriteRenderer == null || !gameObject.activeInHierarchy) return;

        spriteRenderer.color = Color.white;
        isFlashing = false;
    }

    private void Die()
    {
        if (isDead) return;
        isDead = true;

        // Notifie le SpawnManager
        if (spawnManager != null)
        {
            spawnManager.OnZombieKilled();
        }

        // Ajoute des points au ScoreManager
        if (ScoreManager.Instance != null)
        {
            ScoreManager.Instance.AddScore(100);
        }

        // Désactive les composants
        if (rb != null) rb.simulated = false;
        if (col != null) col.enabled = false;
        enabled = false;
        Destroy(gameObject);
    }

    private void OnDisable()
    {
        CancelInvoke();
    }

    private void OnDestroy()
    {
        CancelInvoke();
    }

    public void SetSpawnManager(ZombieSpawnManager manager)
    {
        spawnManager = manager;
    }
}