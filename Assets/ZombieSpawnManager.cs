using UnityEngine;
using System.Collections;
using TMPro;
using UnityEngine.UI;

public class ZombieSpawnManager : MonoBehaviour
{
    [Header("Spawn Settings")]
    public GameObject zombiePrefab;
    public float spawnInterval = 1f;
    public float spawnOffsetX = 5f;
    public float spawnHeightOffset = 2f;

    [Header("Round Settings")]
    public float roundDuration = 10f;
    public int baseZombiesPerRound = 10;
    public float zombiesIncreasePerRound = 1.5f;
    public TextMeshProUGUI roundText;
    public TextMeshProUGUI timerText;
    public TextMeshProUGUI roundTransitionText;
    public float transitionDuration = 2f; // Durée d'affichage au centre
    public float fadeSpeed = 2f;

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

    private IEnumerator RoundTransitionEffect()
    {
        // Configurer le texte de transition
        roundTransitionText.text = $"Round {currentRound + 1}";

        // Fade in
        float alpha = 0f;
        Color textColor = roundTransitionText.color;
        while (alpha < 1f)
        {
            alpha += Time.deltaTime * fadeSpeed;
            roundTransitionText.color = new Color(textColor.r, textColor.g, textColor.b, alpha);
            yield return null;
        }

        yield return new WaitForSeconds(transitionDuration);

        // Fade out
        while (alpha > 0f)
        {
            alpha -= Time.deltaTime * fadeSpeed;
            roundTransitionText.color = new Color(textColor.r, textColor.g, textColor.b, alpha);
            yield return null;
        }

        roundTransitionText.color = new Color(textColor.r, textColor.g, textColor.b, 0f);
    }

    private void StartNextRound()
    {
        // Effet de transition
        StartCoroutine(RoundTransitionEffect());

        // Mise à jour du round
        currentRound++;
        UpdateRoundText();
        zombiesToSpawn = Mathf.RoundToInt(baseZombiesPerRound * Mathf.Pow(zombiesIncreasePerRound, currentRound - 1));
        isSpawning = true;

        // Réinitialiser le timer
        if (timerText != null)
        {
            timerText.text = "";
        }

        // Arrêter les coroutines en cours
        if (spawnCoroutine != null)
            StopCoroutine(spawnCoroutine);

        if (timerCoroutine != null)
            StopCoroutine(timerCoroutine);

        spawnCoroutine = StartCoroutine(SpawnRoutine());
        timerCoroutine = StartCoroutine(TimerRoutine());
    }

    // Méthode pour gérer le timer
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
        // Attendre 1 seconde avant de commencer
        float endTime = Time.time + roundDuration;

        // Faire spawn les zombies
        while (Time.time < endTime && zombiesToSpawn > 0)
        {
            SpawnZombie();
            zombiesToSpawn--;
            zombiesAlive++;
            yield return new WaitForSeconds(spawnInterval);
        }

        // Fin du round
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

        // Position de spawn aléatoire
        float spawnPosX = cameraPosition.x + (Random.value < 0.5f ? -cameraWidth / 2 - spawnOffsetX : cameraWidth / 2 + spawnOffsetX);

        // Raycast pour trouver la position de spawn
        Vector2 rayStart = new Vector2(spawnPosX, cameraPosition.y + cameraHeight / 2);
        RaycastHit2D hit = Physics2D.Raycast(rayStart, Vector2.down, cameraHeight * 2, groundLayer);

        // Position de spawn
        Vector3 spawnPosition;
        if (hit.collider != null)
        {
            spawnPosition = new Vector3(spawnPosX, hit.point.y + spawnHeightOffset, 0);
        }
        else
        {
            spawnPosition = new Vector3(spawnPosX, cameraPosition.y, 0);
        }

        // Créer le zombie
        GameObject zombie = Instantiate(zombiePrefab, spawnPosition, Quaternion.identity);
        ZombieController zombieController = zombie.GetComponent<ZombieController>();

        if (zombieController != null)
        {
            zombieController.SetSpawnManager(this);
        }
    }
}