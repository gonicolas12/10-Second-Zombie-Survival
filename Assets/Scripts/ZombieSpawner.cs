using UnityEngine;
using System.Collections;

public class ZombieSpawner : MonoBehaviour
{
    [Header("Références")]
    [SerializeField] private GameObject zombiePrefab;

    [Header("Paramètres de Spawn")]
    [SerializeField] private float tempsEntreSpawns = 1f;
    [SerializeField] private int nombreMaxZombies = 100;
    [SerializeField] private float margeHorsEcran = 2f; // Distance supplémentaire hors de l'écran

    [Header("Configuration du Sol")]
    [SerializeField] private float hauteurMaxRechercheSol = 10f; // Distance de recherche vers le bas pour trouver le sol
    [SerializeField] private LayerMask layerSol; // Layer du sol

    private Camera mainCamera;
    private int zombiesActuels = 0;

    void Start()
    {
        // Récupérer la caméra principale
        mainCamera = Camera.main;
        // Démarrer le spawn automatique
        StartCoroutine(SpawnAutomatique());
    }

    IEnumerator SpawnAutomatique()
    {
        while (true)
        {
            if (zombiesActuels < nombreMaxZombies)
            {
                SpawnZombie();
            }
            yield return new WaitForSeconds(tempsEntreSpawns);
        }
    }

    Vector2 ObtenirPositionSpawnSol()
    {
        // Calculer les limites horizontales de l'écran en unités monde
        float largeurCamera = 2f * mainCamera.orthographicSize * mainCamera.aspect;

        // Ajouter une marge pour spawn hors écran
        float largeurTotale = largeurCamera / 2 + margeHorsEcran;

        // Générer une position aléatoire dans la plage horizontale
        float posX = Random.Range(-largeurTotale, largeurTotale);

        // Effectuer un raycast pour détecter le sol
        Vector2 origineRaycast = new Vector2(posX, mainCamera.transform.position.y + hauteurMaxRechercheSol);
        RaycastHit2D hit = Physics2D.Raycast(origineRaycast, Vector2.down, hauteurMaxRechercheSol, layerSol);

        if (hit.collider != null)
        {
            // Retourner la position exacte sur le sol
            return new Vector2(posX, hit.point.y);
        }
        else
        {
            // Si aucun sol n'est détecté, on retourne une position par défaut (évite les erreurs)
            Debug.LogWarning("Aucune surface détectée pour spawn un zombie !");
            return new Vector2(posX, mainCamera.transform.position.y);
        }
    }

    void SpawnZombie()
    {
        Vector2 positionSpawn = ObtenirPositionSpawnSol();

        // Créer le zombie
        GameObject zombie = Instantiate(zombiePrefab, positionSpawn, Quaternion.identity);
        zombiesActuels++;

        // Log pour vérifier la position de spawn
        Debug.Log($"Zombie spawned at position: {positionSpawn}");

        // Détruire le zombie après 60 secondes
        Destroy(zombie, 60f);

        // Suivre la destruction du zombie
        StartCoroutine(SuiviDestruction(zombie));
    }

    IEnumerator SuiviDestruction(GameObject zombie)
    {
        yield return new WaitUntil(() => zombie == null);
        zombiesActuels--;
    }

    void OnDrawGizmosSelected()
    {
        // Dessiner une ligne pour visualiser la portée du raycast de recherche de sol
        Gizmos.color = Color.green;
        float largeurCamera = 2f * Camera.main.orthographicSize * Camera.main.aspect;
        float largeurTotale = largeurCamera / 2 + margeHorsEcran;

        Vector2 origineRaycast = new Vector2(-largeurTotale, Camera.main.transform.position.y + hauteurMaxRechercheSol);
        Gizmos.DrawLine(origineRaycast, origineRaycast + Vector2.down * hauteurMaxRechercheSol);

        origineRaycast.x = largeurTotale;
        Gizmos.DrawLine(origineRaycast, origineRaycast + Vector2.down * hauteurMaxRechercheSol);
    }
}
