using UnityEngine;
using System.Collections;
using TMPro;

public class ZombieSpawnManager : MonoBehaviour
{
    [Header("Spawn Settings")]
    public GameObject zombiePrefab;
    public float spawnInterval = 1f;
    public float spawnOffsetX = 5f;
    public float spawnHeightOffset = 2f;

    [Header("Round Settings")]
    public float roundDuration = 10f;
    public int baseZombiesPerRound = 5;
    public float zombiesIncreasePerRound = 1.5f;
    public TextMeshProUGUI roundText;
    public TextMeshProUGUI timerText;

    private Camera mainCamera;
    private LayerMask groundLayer;
    private int currentRound = 0;
    private bool isSpawning = false;
    private int zombiesAlive = 0;
    private int zombiesToSpawn = 0;
    private Coroutine spawnCoroutine;
    private Coroutine timerCoroutine;
    public string killZombiesMessage = "Tuez les zombies restants !";

    private void Start()
    {
        mainCamera = Camera.main;
        groundLayer = LayerMask.GetMask("Ground");
        StartNextRound();
    }

    private void StartNextRound()
    {
        if (timerText != null)
        {
            timerText.text = "";
        }

        currentRound++;
        UpdateRoundText();
        zombiesToSpawn = Mathf.RoundToInt(baseZombiesPerRound * Mathf.Pow(zombiesIncreasePerRound, currentRound - 1));
        isSpawning = true;

        if (spawnCoroutine != null)
            StopCoroutine(spawnCoroutine);

        if (timerCoroutine != null)
            StopCoroutine(timerCoroutine);

        spawnCoroutine = StartCoroutine(SpawnRoutine());
        timerCoroutine = StartCoroutine(TimerRoutine());
    }

    // M�thode pour g�rer le timer
    private IEnumerator TimerRoutine()
    {
        float timeRemaining = roundDuration;

        while (timeRemaining > 0)
        {
            UpdateTimerText(timeRemaining);
            timeRemaining -= Time.deltaTime;
            yield return null;
        }

        UpdateTimerText(0);

        // Attendre 0.5 secondes puis afficher le message
        yield return new WaitForSeconds(0.5f);
        if (timerText != null)
        {
            timerText.text = killZombiesMessage;
        }
    }

    private void UpdateTimerText(float timeRemaining)
    {
        if (timerText != null)
        {
            int seconds = Mathf.CeilToInt(timeRemaining);
            timerText.text = $"{seconds}";
        }
    }

    private void UpdateRoundText()
    {
        if (roundText != null)
        {
            roundText.text = $"Round {currentRound}";
        }
    }

    private IEnumerator SpawnRoutine()
    {
        float endTime = Time.time + roundDuration;

        while (Time.time < endTime && zombiesToSpawn > 0)
        {
            SpawnZombie();
            zombiesToSpawn--;
            zombiesAlive++;
            yield return new WaitForSeconds(spawnInterval);
        }

        isSpawning = false;
        StartCoroutine(CheckForRoundEnd());
    }

    private IEnumerator CheckForRoundEnd()
    {
        while (zombiesAlive > 0)
        {
            yield return new WaitForSeconds(0.5f);
        }

        StartNextRound();
    }

    public void OnZombieKilled()
    {
        zombiesAlive--;
    }

    private void SpawnZombie()
    {
        if (mainCamera == null) return;

        float cameraHeight = 2f * mainCamera.orthographicSize;
        float cameraWidth = cameraHeight * mainCamera.aspect;
        Vector3 cameraPosition = mainCamera.transform.position;

        float spawnPosX = cameraPosition.x + (Random.value < 0.5f ? -cameraWidth / 2 - spawnOffsetX : cameraWidth / 2 + spawnOffsetX);

        Vector2 rayStart = new Vector2(spawnPosX, cameraPosition.y + cameraHeight / 2);
        RaycastHit2D hit = Physics2D.Raycast(rayStart, Vector2.down, cameraHeight * 2, groundLayer);

        Vector3 spawnPosition;
        if (hit.collider != null)
        {
            spawnPosition = new Vector3(spawnPosX, hit.point.y + spawnHeightOffset, 0);
        }
        else
        {
            spawnPosition = new Vector3(spawnPosX, cameraPosition.y, 0);
        }

        GameObject zombie = Instantiate(zombiePrefab, spawnPosition, Quaternion.identity);
        ZombieController zombieController = zombie.GetComponent<ZombieController>();
        if (zombieController != null)
        {
            zombieController.SetSpawnManager(this);
        }
    }
}