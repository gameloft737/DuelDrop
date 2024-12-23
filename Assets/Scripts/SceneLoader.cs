using System;
using UnityEngine;
using UnityEngine.SceneManagement;

public class SceneLoader : MonoBehaviour
{
    private int sceneIndex = 0; // The name of the scene to load
    
    // Selected player prefabs
    public CharacterCycler WASDChararacters;
    public CharacterCycler arrowKeysCharacters;
    public int selectedWASD;
    public int selectedArrowKeys;
    public string[] sceneNames;
    private void Start(){
        sceneIndex = 0;
    }
    public void LoadScene()
    {
        PlayerPrefs.SetInt("selectedWASD",  WASDChararacters.currentIndex);
        PlayerPrefs.SetInt("selectedArrowKeys",  arrowKeysCharacters.currentIndex);
        SceneManager.LoadScene(sceneNames[sceneIndex]);
    }
    public void SetScene(int newPosition){
        sceneIndex = newPosition;
    }
}
