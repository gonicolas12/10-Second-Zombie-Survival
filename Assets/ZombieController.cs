using UnityEngine;

public class ZombieController : MonoBehaviour
{
    public float moveSpeed = 2f;
    public int maxHealth = 3;
    public float avoidanceForce = 5f;

    private int currentHealth;
    private Transform playerTransform;
    private Rigidbody2D rb;
    private CapsuleCollider2D col;
    private SpriteRenderer spriteRenderer;
    private LayerMask groundLayer;
    private bool isDead = false;
    private bool isFlashing = false;

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

        if (rb != null)
        {
            rb.gravityScale = 1f;
            rb.constraints = RigidbodyConstraints2D.FreezeRotation;
        }
    }

    private void FixedUpdate()
    {
        if (isDead || rb == null || playerTransform == null) return;

        if (IsOnGround())
        {
            MoveTowardsPlayer();
        }
    }

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

        // Désactiver les composants immédiatement
        if (rb != null) rb.simulated = false;
        if (col != null) col.enabled = false;

        // Désactiver tous les scripts
        enabled = false;

        // Détruire l'objet immédiatement
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
}