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

    [Header("Limites de Spawn au sol")]
    [SerializeField] private float minY = -4f; // Hauteur minimale pour spawn
    [SerializeField] private float maxY = -3f; // Hauteur maximale pour spawn

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

        // Générer une position Y dans les limites du sol définies
        float posY = Random.Range(minY, maxY);

        // Retourner la position en coordonnées monde
        return (Vector2)mainCamera.transform.position + new Vector2(posX, posY);
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
}
