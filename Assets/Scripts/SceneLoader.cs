using System;
using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;


public class SceneLoader : MonoBehaviour
{
    private int sceneIndex = 0;
    [SerializeField]private int modeIndex = 1;


    // Selected player prefabs
    public CharacterCycler WASDCharacters;
    public CharacterCycler arrowKeysCharacters;
    public string[] sceneNames;
    public Texture2D[] sceneIcons;
    public MeshRenderer sceneImage;


    private void Start()
    {
        if (sceneIcons.Length > 0 && sceneImage != null)
        {
            sceneIndex = 0;
            sceneImage.material.mainTexture = sceneIcons[sceneIndex];
        }
        else
        {
            Debug.LogError("Scene icons or scene image not set!");
        }
    }


    public void LoadScene()
    {
        StartCoroutine(RunLoad(0.5f));
    }
    private IEnumerator RunLoad(float duration){
        yield return new WaitForSeconds(duration);
        if (sceneIndex < sceneNames.Length)
        {
            PlayerPrefs.SetInt("selectedWASD", WASDCharacters.currentIndex);
            PlayerPrefs.SetInt("selectedArrowKeys", arrowKeysCharacters.currentIndex);
            PlayerPrefs.SetInt("modeIndex", modeIndex);


            SceneManager.LoadScene(sceneNames[sceneIndex]);
        }
        else
        {
            Debug.LogError("Scene index out of range!");
        }
    }

    public void SetScene(int newPosition)
    {
        if (newPosition >= 0 && newPosition < sceneIcons.Length)
        {
            sceneImage.material.mainTexture = sceneIcons[newPosition];
            sceneIndex = newPosition;
        }
        else
        {
            Debug.LogError("Invalid scene index!");
        }
    }


    public void SetMode(int newMode)
    {
        modeIndex = newMode;
    }
}
