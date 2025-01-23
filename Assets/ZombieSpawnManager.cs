using UnityEngine;

public class ZombieSpawnManager : MonoBehaviour
{
    public GameObject zombiePrefab;
    public float spawnInterval = 1f;
    public float spawnOffsetX = 5f; // Distance horizontale depuis le bord de la cam�ra
    public float spawnHeightOffset = 2f; // Distance au-dessus du sol

    private Camera mainCamera;
    private LayerMask groundLayer;

    private void Start()
    {
        mainCamera = Camera.main;
        groundLayer = LayerMask.GetMask("Ground");
        InvokeRepeating(nameof(SpawnZombie), 0f, spawnInterval);
    }

    private void SpawnZombie()
    {
        if (mainCamera == null) return;

        // Calculer les limites de la cam�ra en world space
        float cameraHeight = 2f * mainCamera.orthographicSize;
        float cameraWidth = cameraHeight * mainCamera.aspect;
        Vector3 cameraPosition = mainCamera.transform.position;

        // Choisir al�atoirement le c�t� de spawn (gauche ou droite)
        float spawnPosX = cameraPosition.x + (Random.value < 0.5f ? -cameraWidth / 2 - spawnOffsetX : cameraWidth / 2 + spawnOffsetX);

        // Trouver le sol � cette position X
        Vector2 rayStart = new Vector2(spawnPosX, cameraPosition.y + cameraHeight / 2);
        RaycastHit2D hit = Physics2D.Raycast(rayStart, Vector2.down, cameraHeight * 2, groundLayer);

        if (hit.collider != null)
        {
            // Spawn le zombie l�g�rement au-dessus du point de contact
            Vector3 spawnPosition = new Vector3(spawnPosX, hit.point.y + spawnHeightOffset, 0);
            Instantiate(zombiePrefab, spawnPosition, Quaternion.identity);
        }
        else
        {
            // Si aucun sol n'est trouv�, spawn � la hauteur de la cam�ra
            Vector3 spawnPosition = new Vector3(spawnPosX, cameraPosition.y, 0);
            Instantiate(zombiePrefab, spawnPosition, Quaternion.identity);
        }
    }
}