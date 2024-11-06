using UnityEditor.PackageManager;
using UnityEngine;
using UnityEngine.SceneManagement;

public class CharacterSelection : MonoBehaviour
{
    public string sceneToLoad; // The name of the scene to load
    
    // Selected player prefabs
    public GameObject[] WASDChararacters;
    public GameObject[] arrowKeysCharacters;
    public int selectedWASD;
    public int selectedArrowKeys;
    public void LoadScene()
    {
        PlayerPrefs.SetInt("selectedWASD",  selectedWASD);
        PlayerPrefs.SetInt("selectedArrowKeys",  selectedArrowKeys);
        SceneManager.LoadScene(sceneToLoad);
    }
}
