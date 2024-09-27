using UnityEngine;

public class CameraFollow : MonoBehaviour
{
    public Transform player1;  // Reference to Player 1
    public Transform player2;  // Reference to Player 2
    public float distance = 5f; // Distance behind the players
    public float height = 3f; // Height above the players
    public float smoothSpeed = 0.125f; // Smoothing factor for camera movement

    private void LateUpdate()
    {
        if (player1 == null || player2 == null) return;

        // Calculate the midpoint between the two players
        Vector3 midpoint = (player1.position + player2.position) / 2;

        // Set the desired camera position
        Vector3 desiredPosition = midpoint + Vector3.back * distance + Vector3.up * height;

        // Smoothly interpolate the camera position
        Vector3 smoothedPosition = Vector3.Lerp(transform.position, desiredPosition, smoothSpeed);
        transform.position = smoothedPosition;

        // Optionally make the camera look at the midpoint
        transform.LookAt(midpoint);
    }
}
