using UnityEngine;

public class CameraFollow : MonoBehaviour
{
    public Transform player1;  // Reference to Player 1
    public Transform player2;  // Reference to Player 2
    public float baseDistance = 5f; // Base distance behind the players
    public float baseHeight = 3f; // Base height above the players
    public float maxDistance = 10f; // Maximum distance to zoom out when players are far apart
    public float smoothSpeed = 0.125f; // Base smoothing factor
    public float maxSmoothSpeed = 0.25f; // Max smoothing speed for quick adjustments

    private void OnEnable()
    {
        // Subscribe to the OnPlayersSpawned event
        PlayerSpawner.OnPlayersSpawned += InitializePlayers;
    }

    private void OnDisable()
    {
        // Unsubscribe from the event when this script is disabled
        PlayerSpawner.OnPlayersSpawned -= InitializePlayers;
    }

    private void InitializePlayers()
    {
        // Get players by tag now that they are guaranteed to be instantiated
        player1 = GameObject.FindGameObjectWithTag("WASDPlayer").transform;
        player2 = GameObject.FindGameObjectWithTag("ArrowKeysPlayer").transform;

        if (player1 == null || player2 == null)
        {
            Debug.LogError("Players not found by tag.");
            return;
        }
    }

    private void LateUpdate()
    {
        if (player1 == null || player2 == null) return;

        // Calculate the midpoint between the two players
        Vector3 midpoint = (player1.position + player2.position) / 2;

        // Calculate distance between players
        float playersDistance = Vector3.Distance(player1.position, player2.position);

        // Adjust distance and height based on players' separation
        float dynamicDistance = Mathf.Lerp(baseDistance, maxDistance, playersDistance / maxDistance);

        // Set the desired camera position
        Vector3 desiredPosition = midpoint + Vector3.back * dynamicDistance + Vector3.up * baseHeight;

        // Adjust smoothing speed based on players' separation
        float currentSmoothSpeed = Mathf.Lerp(smoothSpeed, maxSmoothSpeed, playersDistance / maxDistance);

        // Smoothly interpolate the camera position
        Vector3 smoothedPosition = Vector3.Lerp(transform.position, desiredPosition, currentSmoothSpeed);
        transform.position = smoothedPosition;

        // Make the camera look at the midpoint
        transform.LookAt(midpoint);
    }
}
