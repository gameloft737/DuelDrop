using UnityEditor.PackageManager;
using UnityEngine;
using UnityEngine.SceneManagement;

public class SceneLoader : MonoBehaviour
{
    public string sceneToLoad; // The name of the scene to load
    
    // Selected player prefabs
    public CharacterCycler WASDChararacters;
    public CharacterCycler arrowKeysCharacters;
    public int selectedWASD;
    public int selectedArrowKeys;
    public void LoadScene()
    {
        PlayerPrefs.SetInt("selectedWASD",  WASDChararacters.currentIndex);
        PlayerPrefs.SetInt("selectedArrowKeys",  arrowKeysCharacters.currentIndex);
        SceneManager.LoadScene(sceneToLoad);
    }
}
