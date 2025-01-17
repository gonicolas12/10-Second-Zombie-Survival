using UnityEngine;

public class Zombie : MonoBehaviour
{
    [Header("Paramètres du Zombie")]
    [SerializeField] private float vitesse = 5f;
    [SerializeField] private float pointsDeVie = 100f;
    [SerializeField] private float degats = 20f;
    [SerializeField] private Vector2 zoneAttaqueDimensions = new Vector2(1f, 1f); // Taille de la zone d'attaque
    [SerializeField] private float tempsEntreAttaques = 1f; // Temps entre chaque attaque

    private Transform joueur;
    private Rigidbody2D rb;
    private SpriteRenderer spriteRenderer;
    private bool estVivant = true;
    private bool enTrainDattaquer = false;

    void Start()
    {
        // Obtient les composants nécessaires
        rb = GetComponent<Rigidbody2D>();
        spriteRenderer = GetComponent<SpriteRenderer>();
        // Trouve le joueur au démarrage
        joueur = GameObject.FindGameObjectWithTag("Player")?.transform;
    }

    void Update()
    {
        if (!estVivant || joueur == null) return;

        // Vérifie si le joueur est dans la zone d'attaque
        if (EstDansZoneAttaque())
        {
            if (!enTrainDattaquer)
            {
                // Lance une attaque
                StartCoroutine(Attaquer());
            }
            // Arrête le mouvement
            rb.linearVelocity = Vector2.zero;
        }
        else
        {
            // Calcule la direction vers le joueur
            Vector2 direction = ((Vector2)joueur.position - rb.position).normalized;

            // Déplace le zombie
            rb.linearVelocity = direction * vitesse;

            // Retourne le sprite selon la direction
            if (direction.x < 0)
                spriteRenderer.flipX = true;
            else if (direction.x > 0)
                spriteRenderer.flipX = false;
        }
    }

    private bool EstDansZoneAttaque()
    {
        // Position de la zone d'attaque
        Vector2 centre = (Vector2)transform.position;
        // Vérifie si le joueur est dans la zone définie
        Collider2D joueurCollider = Physics2D.OverlapBox(centre, zoneAttaqueDimensions, 0f, LayerMask.GetMask("Player"));
        return joueurCollider != null;
    }

    private System.Collections.IEnumerator Attaquer()
    {
        enTrainDattaquer = true;

        // Inflict damage to the player (to be implemented in the player's script)
        Debug.Log("Zombie attaque le joueur !");

        // TODO: Ajouter le code pour infliger des dégâts au joueur

        // Attendre avant de pouvoir attaquer à nouveau
        yield return new WaitForSeconds(tempsEntreAttaques);
        enTrainDattaquer = false;
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
        estVivant = false;
        rb.linearVelocity = Vector2.zero; // Arrête le zombie
        Destroy(gameObject, 1f); // Détruit le zombie après 1 seconde
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("Player"))
        {
            Debug.Log("Le zombie a touché le joueur !");
        }
    }

    void OnDrawGizmosSelected()
    {
        // Dessine la zone d'attaque pour la visualiser dans l'éditeur
        Gizmos.color = Color.red;
        Gizmos.DrawWireCube(transform.position, zoneAttaqueDimensions);
    }
}
