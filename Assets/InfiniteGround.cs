using UnityEngine;

public class InfiniteGround : MonoBehaviour
{
    [SerializeField] private Transform ground;
    [SerializeField] private float groundWidth = 60f; // Largeur du sol
    private Camera mainCamera;

    private void Start()
    {
        mainCamera = Camera.main;
    }

    private void Update()
    {
        // V�rifie si le sol est hors de l'�cran
        float cameraLeftEdge = mainCamera.transform.position.x - mainCamera.orthographicSize * mainCamera.aspect;
        float cameraRightEdge = mainCamera.transform.position.x + mainCamera.orthographicSize * mainCamera.aspect;

        // Repositionne le sol si n�cessaire
        if (ground.position.x + groundWidth / 2 < cameraLeftEdge)
        {
            Debug.Log("Repositioning ground to the right");
            RepositionGroundToRight();
        }
        else if (ground.position.x - groundWidth / 2 > cameraRightEdge)
        {
            Debug.Log("Repositioning ground to the left");
            RepositionGroundToLeft();
        }
    }

    private void RepositionGroundToRight()
    {
        // Repositionne le sol � droite
        ground.position = new Vector3(ground.position.x + groundWidth, ground.position.y, ground.position.z);
    }

    private void RepositionGroundToLeft()
    {
        // Repositionne le sol � gauche
        ground.position = new Vector3(ground.position.x - groundWidth, ground.position.y, ground.position.z);
    }
}
