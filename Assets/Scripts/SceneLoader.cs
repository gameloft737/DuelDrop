using System;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class SceneLoader : MonoBehaviour
{
    private int sceneIndex = 0;
    private int modeIndex = 1;
    // Selected player prefabs
    public CharacterCycler WASDChararacters;
    public CharacterCycler arrowKeysCharacters;
    public int selectedWASD;
    public int selectedArrowKeys;
    public string[] sceneNames;
    public Sprite[] sceneIcons;
    public Image sceneImage;
    private void Start(){
        sceneIndex = 0;
        sceneImage.sprite = sceneIcons[sceneIndex];
    }
    public void LoadScene()
    {
        PlayerPrefs.SetInt("selectedWASD",  WASDChararacters.currentIndex);
        PlayerPrefs.SetInt("selectedArrowKeys",  arrowKeysCharacters.currentIndex);
        
        PlayerPrefs.SetInt("modeIndex",  modeIndex);
        SceneManager.LoadScene(sceneNames[sceneIndex]);
    }
    public void SetScene(int newPosition){
        sceneImage.enabled = true;
        sceneIndex = newPosition;
        sceneImage.sprite = sceneIcons[sceneIndex];
    }
    public void SetMode(int newMode){
        modeIndex = newMode;
    }
}
