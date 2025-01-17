using UnityEngine;

public class Zombie : MonoBehaviour
{
    [Header("Param�tres du Zombie")]
    [SerializeField] private float vitesse = 3f;
    [SerializeField] private float pointsDeVie = 100f;
    [SerializeField] private float degats = 20f;

    private Transform joueur;
    private Rigidbody2D rb;
    private SpriteRenderer spriteRenderer;
    private bool estVivant = true;

    void Start()
    {
        // Obtient les composants n�cessaires
        rb = GetComponent<Rigidbody2D>();
        spriteRenderer = GetComponent<SpriteRenderer>();
        // Trouve le joueur au d�marrage
        joueur = GameObject.FindGameObjectWithTag("Player")?.transform;
    }

    void Update()
    {
        if (!estVivant || joueur == null) return;

        // Calcule la direction vers le joueur
        Vector2 direction = ((Vector2)joueur.position - rb.position).normalized;

        // D�place le zombie
        rb.linearVelocity = direction * vitesse;

        // Retourne le sprite selon la direction
        if (direction.x < 0)
            spriteRenderer.flipX = true;
        else if (direction.x > 0)
            spriteRenderer.flipX = false;
    }

    public void PrendreDegats(float degats)
    {
        pointsDeVie -= degats;

        if (pointsDeVie <= 0 && estVivant)
        {
            Mourir();
        }
    }

    private void Mourir()
    {
        estVivant = true;
        // D�sactive les composants
        rb.linearVelocity = Vector2.zero;
        Destroy(gameObject, 1f);
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("Player"))
        {
            // Vous pouvez ajouter ici le code pour infliger des d�g�ts au joueur
            Debug.Log("Le zombie a touch� le joueur !");
        }
    }
}
