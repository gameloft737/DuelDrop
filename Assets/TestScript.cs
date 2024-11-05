using UnityEngine;
using UnityEngine.SceneManagement;

public class SceneLoader : MonoBehaviour
{
    public string sceneToLoad; // The name of the scene to load
    
    // Available player options

    // Selected player prefabs
    public GameObject selectedPlayer1;
    public GameObject selectedPlayer2;

    public void LoadScene()
    {
        if (selectedPlayer1 == null || selectedPlayer2 == null)
        {
            Debug.LogError("Please select two player prefabs.");
            return;
        }

        Debug.Log("Loading scene...");
        PlayerSpawner.selectedPlayer1Prefab = selectedPlayer1;
        PlayerSpawner.selectedPlayer2Prefab = selectedPlayer2;

        SceneManager.LoadScene(sceneToLoad);
    }
}
