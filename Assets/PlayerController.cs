using UnityEngine;

public class PlayerController : MonoBehaviour
{
    [Header("Movement Parameters")]
    [SerializeField] private float moveSpeed = 5f;
    [SerializeField] private float jumpForce = 8f;

    [Header("Ground Check")]
    [SerializeField] private Transform groundCheck;
    [SerializeField] private float groundCheckRadius = 0.2f;
    [SerializeField] private LayerMask groundLayer;

    private Rigidbody2D rb;
    private bool isGrounded;
    private float horizontalInput;
    private bool facingRight = true;

    private void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
    }

    private void Update()
    {
        // Vérification du sol
        isGrounded = Physics2D.OverlapCircle(groundCheck.position, groundCheckRadius, groundLayer);

        // Utilisation des flèches directionnelles au lieu de Q et D
        horizontalInput = Input.GetAxisRaw("Horizontal");

        // Debug de la détection du sol
        Debug.Log("Is Grounded: " + isGrounded);

        // Saut (barre d'espace) avec debug
        if (Input.GetButtonDown("Jump") && isGrounded)
        {
            Debug.Log("Jump attempted!");
            rb.AddForce(Vector2.up * jumpForce, ForceMode2D.Impulse);
        }

        // Gestion du flip du sprite selon la direction
        if (horizontalInput > 0 && !facingRight)
        {
            Flip();
        }
        else if (horizontalInput < 0 && facingRight)
        {
            Flip();
        }
    }

    private void FixedUpdate()
    {
        // Application du mouvement
        rb.linearVelocity = new Vector2(horizontalInput * moveSpeed, rb.linearVelocity.y);
    }

    private void Flip()
    {
        facingRight = !facingRight;
        Vector3 scale = transform.localScale;
        scale.x *= -1;
        transform.localScale = scale;
    }

    private void OnDrawGizmos()
    {
        if (groundCheck != null)
        {
            // Visualisation du groundCheck dans l'éditeur
            Gizmos.color = Color.yellow;
            Gizmos.DrawWireSphere(groundCheck.position, groundCheckRadius);
        }
    }
}