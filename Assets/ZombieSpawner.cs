using UnityEngine;
using System.Collections;

public class ZombieSpawner : MonoBehaviour
{
    [Header("Références")]
    [SerializeField] private GameObject zombiePrefab;

    [Header("Paramètres de Spawn")]
    [SerializeField] private float tempsEntreSpawns = 10f;
    [SerializeField] private int nombreMaxZombies = 10;
    [SerializeField] private float margeHorsEcran = 2f; // Distance supplémentaire hors de l'écran

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

    Vector2 ObtenirPositionSpawnHorsEcran()
    {
        // Calculer les limites de l'écran en unités monde
        float hauteurCamera = 2f * mainCamera.orthographicSize;
        float largeurCamera = hauteurCamera * mainCamera.aspect;

        // Ajouter une marge pour spawn hors écran
        float hauteurTotale = hauteurCamera / 2 + margeHorsEcran;
        float largeurTotale = largeurCamera / 2 + margeHorsEcran;

        // Choisir un côté aléatoirement
        int cote = Random.Range(0, 4);
        Vector2 position = Vector2.zero;

        switch (cote)
        {
            case 0: // Haut
                position = new Vector2(
                    Random.Range(-largeurCamera, largeurCamera),
                    hauteurTotale
                );
                break;
            case 1: // Droite
                position = new Vector2(
                    largeurTotale,
                    Random.Range(-hauteurCamera, hauteurCamera)
                );
                break;
            case 2: // Bas
                position = new Vector2(
                    Random.Range(-largeurCamera, largeurCamera),
                    -hauteurTotale
                );
                break;
            case 3: // Gauche
                position = new Vector2(
                    -largeurTotale,
                    Random.Range(-hauteurCamera, hauteurCamera)
                );
                break;
        }

        // Retourner la position en coordonnées monde
        return (Vector2)mainCamera.transform.position + position;
    }

    void SpawnZombie()
    {
        Vector2 positionSpawn = ObtenirPositionSpawnHorsEcran();

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
