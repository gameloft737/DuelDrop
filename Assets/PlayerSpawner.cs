using UnityEngine;
using System;

public class PlayerSpawner : MonoBehaviour
{
    public static GameObject selectedPlayer1Prefab; // Prefab for Player 1 (selected from SceneLoader)
    public static GameObject selectedPlayer2Prefab; // Prefab for Player 2 (selected from SceneLoader)
    public Vector3 player1SpawnPoint = new Vector3(-2, 0, 0); // Spawn point for Player 1
    public Vector3 player2SpawnPoint = new Vector3(2, 0, 0);  // Spawn point for Player 2

    // Event to notify when players are spawned
    public static event Action OnPlayersSpawned;

    private void Start()
    {
        // Check if the prefabs are assigned
        if (selectedPlayer1Prefab == null || selectedPlayer2Prefab == null)
        {
            Debug.LogError("Selected player prefabs are not assigned.");
            return;
        }

        // Instantiate the selected players at designated spawn points
        Instantiate(selectedPlayer1Prefab, player1SpawnPoint, Quaternion.identity);
        Instantiate(selectedPlayer2Prefab, player2SpawnPoint, Quaternion.identity);

        Debug.Log("Players spawned successfully in the new scene.");

        // Trigger the event to notify other scripts
        OnPlayersSpawned?.Invoke();
    }
}
