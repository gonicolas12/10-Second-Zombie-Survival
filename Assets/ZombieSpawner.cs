using UnityEngine;
using System.Collections;

public class ZombieSpawner : MonoBehaviour
{
    [Header("R�f�rences")]
    [SerializeField] private GameObject zombiePrefab;

    [Header("Param�tres de Spawn")]
    [SerializeField] private float tempsEntreSpawns = 10f;
    [SerializeField] private int nombreMaxZombies = 10;
    [SerializeField] private float margeHorsEcran = 2f; // Distance suppl�mentaire hors de l'�cran

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

    Vector2 ObtenirPositionSpawnHorsEcran()
    {
        // Calculer les limites de l'�cran en unit�s monde
        float hauteurCamera = 2f * mainCamera.orthographicSize;
        float largeurCamera = hauteurCamera * mainCamera.aspect;

        // Ajouter une marge pour spawn hors �cran
        float hauteurTotale = hauteurCamera / 2 + margeHorsEcran;
        float largeurTotale = largeurCamera / 2 + margeHorsEcran;

        // Choisir un c�t� al�atoirement
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

        // Retourner la position en coordonn�es monde
        return (Vector2)mainCamera.transform.position + position;
    }

    void SpawnZombie()
    {
        Vector2 positionSpawn = ObtenirPositionSpawnHorsEcran();

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
