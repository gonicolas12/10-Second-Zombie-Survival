using UnityEngine;
using System.Collections;

public class ZombieSpawner : MonoBehaviour
{
    [Header("R�f�rences")]
    [SerializeField] private GameObject zombiePrefab;

    [Header("Param�tres de Spawn")]
    [SerializeField] private float tempsEntreSpawns = 1f;
    [SerializeField] private int nombreMaxZombies = 100;
    [SerializeField] private float margeHorsEcran = 2f; // Distance suppl�mentaire hors de l'�cran

    [Header("Limites de Spawn au sol")]
    [SerializeField] private float minY = -4f; // Hauteur minimale pour spawn
    [SerializeField] private float maxY = -3f; // Hauteur maximale pour spawn

    private Camera mainCamera;
    private int zombiesActuels = 0;

    void Start()
    {
        // R�cup�rer la cam�ra principale
        mainCamera = Camera.main;
        // D�marrer le spawn automatique
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
        // Calculer les limites horizontales de l'�cran en unit�s monde
        float largeurCamera = 2f * mainCamera.orthographicSize * mainCamera.aspect;

        // Ajouter une marge pour spawn hors �cran
        float largeurTotale = largeurCamera / 2 + margeHorsEcran;

        // G�n�rer une position al�atoire dans la plage horizontale
        float posX = Random.Range(-largeurTotale, largeurTotale);

        // G�n�rer une position Y dans les limites du sol d�finies
        float posY = Random.Range(minY, maxY);

        // Retourner la position en coordonn�es monde
        return (Vector2)mainCamera.transform.position + new Vector2(posX, posY);
    }

    void SpawnZombie()
    {
        Vector2 positionSpawn = ObtenirPositionSpawnSol();

        // Cr�er le zombie
        GameObject zombie = Instantiate(zombiePrefab, positionSpawn, Quaternion.identity);
        zombiesActuels++;

        // Log pour v�rifier la position de spawn
        Debug.Log($"Zombie spawned at position: {positionSpawn}");

        // D�truire le zombie apr�s 60 secondes
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
