using UnityEngine;

public class PlayerController : MonoBehaviour
{
    [Header("Movement Parameters")]
    [SerializeField] private float moveSpeed = 5f;
    [SerializeField] private float jumpForce = 8f;
    [SerializeField] private float fallMultiplier = 2.5f;

    [Header("Shooting Parameters")]
    [SerializeField] private GameObject bulletPrefab;
    [SerializeField] private Transform firePoint;
    [SerializeField] private float bulletSpeed = 30f;

    [Header("Ground Check")]
    [SerializeField] private Transform groundCheck;
    [SerializeField] private float groundCheckRadius = 0.2f;
    [SerializeField] private LayerMask groundLayer;

    [Header("Animation Parameters")]
    [SerializeField] private float runAnimationSpeed = 1f;

    // Constantes pour les paramètres d'animation
    private static readonly string IS_RUNNING = "IsRunning";
    private static readonly string IS_SHOOTING = "IsShooting";

    private Rigidbody2D rb;
    private bool isGrounded;
    private float horizontalInput;
    private bool facingRight = true;
    private Camera mainCamera;
    private Animator animator;

    private void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        mainCamera = Camera.main;
        animator = GetComponent<Animator>();
    }

    private void Update()
    {
        // Vérification du sol
        isGrounded = Physics2D.OverlapCircle(groundCheck.position, groundCheckRadius, groundLayer);

        // Déplacements
        horizontalInput = Input.GetAxisRaw("Horizontal");

        // Animation de course/idle
        animator.SetBool(IS_RUNNING, Mathf.Abs(horizontalInput) > 0.1f);
        animator.SetFloat("AnimationSpeed", runAnimationSpeed);

        // Saut
        if (Input.GetButtonDown("Jump") && isGrounded)
        {
            rb.AddForce(Vector2.up * jumpForce, ForceMode2D.Impulse);
        }

        // Vérification de la position de la souris pour la direction du personnage
        Vector2 mousePosition = mainCamera.ScreenToWorldPoint(Input.mousePosition);
        if (mousePosition.x > transform.position.x && !facingRight)
        {
            Flip();
        }
        else if (mousePosition.x < transform.position.x && facingRight)
        {
            Flip();
        }

        // Tir
        if (Input.GetButtonDown("Fire1"))
        {
            Shoot(mousePosition);
        }
    }

    private void FixedUpdate()
    {
        // Mouvement horizontal
        rb.linearVelocity = new Vector2(horizontalInput * moveSpeed, rb.linearVelocity.y);

        // Gravité augmentée pendant la chute
        if (rb.linearVelocity.y < 0)
        {
            rb.linearVelocity += Vector2.up * Physics2D.gravity.y * (fallMultiplier - 1) * Time.fixedDeltaTime;
        }
    }

    private void Shoot(Vector2 targetPosition)
    {
        // Déclenche l'animation de tir
        animator.SetTrigger(IS_SHOOTING);

        // Crée la balle au point de tir
        GameObject bullet = Instantiate(bulletPrefab, firePoint.position, Quaternion.identity);

        // Calcule la direction vers la position de la souris
        Vector2 direction = (targetPosition - (Vector2)firePoint.position).normalized;
        Rigidbody2D bulletRb = bullet.GetComponent<Rigidbody2D>();

        // Applique la vélocité à la balle
        bulletRb.linearVelocity = direction * bulletSpeed;

        // Calcule l'angle pour la rotation de la balle
        float angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;
        bullet.transform.rotation = Quaternion.Euler(0, 0, angle);
    }

    private void Flip()
    {
        facingRight = !facingRight; // Inverse la direction du personnage
        Vector3 scale = transform.localScale; // Récupère l'échelle actuelle
        scale.x *= -1;
        transform.localScale = scale;
    }
}