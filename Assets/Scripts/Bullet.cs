using UnityEngine;

public class Bullet : MonoBehaviour
{
    private bool isDestroyed = false;
    private string playerTag = "Player";

    private void OnTriggerEnter2D(Collider2D collision)
    {
        // Vérifie si la balle n'est pas déjà détruite et si ce n'est pas le joueur
        if (!isDestroyed && collision.gameObject.tag != playerTag)
        {
            isDestroyed = true;
            Destroy(gameObject);
        }
    }

    // Ajoute une auto-destruction après un certain temps pour éviter les balles perdues
    private void Start()
    {
        Destroy(gameObject, 3f);
    }
}
