using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class StartGameButton : MonoBehaviour
{
    [SerializeField] string tutorialName;
    [SerializeField] string selectionName;
    [SerializeField] GameObject popup;

    // Method to start the game
    public void StartGame()
    {
        if (PlayerPrefs.GetInt("firstTime", 1) == 1)
        {
            ShowPopup();
            PlayerPrefs.SetInt("firstTime", 0);
            return;
        }
        SceneManager.LoadScene(selectionName);
    }

    // Method to start the tutorial
    public void StartTutorial()
    {
        SceneManager.LoadScene(tutorialName);
    }

    // Method to show the popup
    public void ShowPopup()
    {
        popup.SetActive(true);
    }

    // Reset firstTime flag via the Inspector
    [ContextMenu("Reset First Time")]
    public void ResetFirstTime()
    {
        PlayerPrefs.SetInt("firstTime", 1);
        Debug.Log("firstTime has been reset.");
    }
}
