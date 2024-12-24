using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class StartGameButton : MonoBehaviour
{
    [SerializeField] string tutorialName;
    [SerializeField] string selectionName;

    // Method to start the game
    public void StartGame()
    {
        if (PlayerPrefs.GetInt("firstTime", 1) == 1)
        {
            PlayerPrefs.SetInt("firstTime", 0);
            StartTutorial();
            return;
        }
        SceneManager.LoadScene(selectionName);
    }

    // Method to start the tutorial
    public void StartTutorial()
    {
        SceneManager.LoadScene(tutorialName);
    }

    // Reset firstTime flag via the Inspector
    [ContextMenu("Reset First Time")]
    public void ResetFirstTime()
    {
        PlayerPrefs.SetInt("firstTime", 1);
        Debug.Log("firstTime has been reset.");
    }
}
    