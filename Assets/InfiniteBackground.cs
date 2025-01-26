using UnityEngine;

public class InfiniteBackground : MonoBehaviour
{
    public float parallaxEffect = 0.3f;
    public GameObject imagePrefab; // L'image à répéter
    public int numberOfCopies = 3;
    private float imageWidth;
    private Transform playerTransform;
    private GameObject[] backgrounds;
    private Vector3 lastPlayerPosition;

    void Start()
    {
        playerTransform = GameObject.FindGameObjectWithTag("Player").transform;
        lastPlayerPosition = playerTransform.position;

        // Calculer la largeur de l'image
        SpriteRenderer spriteRenderer = imagePrefab.GetComponent<SpriteRenderer>();
        imageWidth = spriteRenderer.bounds.size.x;

        // Créer les copies
        backgrounds = new GameObject[numberOfCopies * 2 + 1];
        for (int i = -numberOfCopies; i <= numberOfCopies; i++)
        {
            Vector3 position = new Vector3(imageWidth * i, transform.position.y, transform.position.z);
            backgrounds[i + numberOfCopies] = Instantiate(imagePrefab, position, Quaternion.identity, transform);
        }
    }

    void Update()
    {
        if (playerTransform == null) return;

        // Calculer le déplacement
        float deltaX = (playerTransform.position.x - lastPlayerPosition.x) * parallaxEffect;
        transform.position += new Vector3(deltaX, 0, 0);

        // Repositionner les images si nécessaire
        float playerPosX = playerTransform.position.x;
        foreach (GameObject bg in backgrounds)
        {
            float relativeX = bg.transform.position.x - playerPosX;
            if (relativeX < -imageWidth * (numberOfCopies + 0.5f))
            {
                bg.transform.position += new Vector3(imageWidth * (numberOfCopies * 2 + 1), 0, 0);
            }
            else if (relativeX > imageWidth * (numberOfCopies + 0.5f))
            {
                bg.transform.position -= new Vector3(imageWidth * (numberOfCopies * 2 + 1), 0, 0);
            }
        }

        lastPlayerPosition = playerTransform.position;
    }
}