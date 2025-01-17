using UnityEngine;

public class Bullet : MonoBehaviour
{
    private bool isDestroyed = false;
    private string playerTag = "Player";

    private void OnTriggerEnter2D(Collider2D collision)
    {
        // V�rifie si la balle n'est pas d�j� d�truite et si ce n'est pas le joueur
        if (!isDestroyed && collision.gameObject.tag != playerTag)
        {
            isDestroyed = true;
            Destroy(gameObject);
        }
    }

    // Ajoute une auto-destruction apr�s un certain temps pour �viter les balles perdues
    private void Start()
    {
        Destroy(gameObject, 3f);
    }
}
