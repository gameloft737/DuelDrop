using UnityEngine;
using System;

public class PlayerSpawner : MonoBehaviour
{
    public static PlayerSpawner instance;
    public GameObject[] WASDCharacters;
    public GameObject[] arrowKeysCharacters;
    public GameObject WASDPlayer; // Prefab for Player 1 (selected from SceneLoader)
    public GameObject arrowKeysPlayer; // Prefab for Player 2 (selected from SceneLoader)
    public Vector3 WASDPlayerSpawnPoint = new Vector3(-2, 0, 0); // Spawn point for Player 1
    public Vector3 arrowKeysPlayerSpawnPoint = new Vector3(2, 0, 0);  // Spawn point for Player 2

    // Event to notify when players are spawned
    public static event Action OnPlayersSpawned;
    private void Awake()
    {
        if(instance == null){
            instance = this;
        }
        else{
            Destroy(gameObject);
            return;
        }   
    }
    private void Start()
    {
        
        WASDPlayer = WASDCharacters[PlayerPrefs.GetInt("selectedWASD")];
        arrowKeysPlayer = arrowKeysCharacters[PlayerPrefs.GetInt("selectedArrowKeys")];
        // Instantiate the selected players at designated spawn points
        Instantiate(WASDPlayer, WASDPlayerSpawnPoint, Quaternion.identity);
        Instantiate(arrowKeysPlayer, arrowKeysPlayerSpawnPoint, Quaternion.identity);
        // Trigger the event to notify other scripts
        OnPlayersSpawned?.Invoke();
    }
    public void TeleportPlayer(Transform WASDPlayer, Transform arrowKeyPlayer){
        arrowKeyPlayer.position = arrowKeysPlayerSpawnPoint;
        WASDPlayer.position = WASDPlayerSpawnPoint;
    }

}
