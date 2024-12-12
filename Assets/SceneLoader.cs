using System;
using UnityEngine;
using UnityEngine.SceneManagement;

public class SceneLoader : MonoBehaviour
{
    private string sceneToLoad; // The name of the scene to load
    
    // Selected player prefabs
    public CharacterCycler WASDChararacters;
    public CharacterCycler arrowKeysCharacters;
    public int selectedWASD;
    public int selectedArrowKeys;
    public string[] sceneNames;
    public void LoadScene()
    {
        PlayerPrefs.SetInt("selectedWASD",  WASDChararacters.currentIndex);
        PlayerPrefs.SetInt("selectedArrowKeys",  arrowKeysCharacters.currentIndex);
        SceneManager.LoadScene(sceneToLoad);
    }
    public void SetScene(int newPosition){
        sceneToLoad = sceneNames[newPosition];
    }
}
