using UnityEngine;

public class CameraFollow : MonoBehaviour
{
    [SerializeField] private Transform target;
    [SerializeField] private float smoothSpeed = 0.125f;
    [SerializeField] private Vector3 offset = new Vector3(0, 1, -10);

    private void FixedUpdate()
    {
        if (target == null) return;

        // Position désirée de la caméra
        Vector3 desiredPosition = target.position + offset;

        // Lissage du mouvement
        Vector3 smoothedPosition = Vector3.Lerp(transform.position, desiredPosition, smoothSpeed);

        // Application de la position
        transform.position = smoothedPosition;
    }
}