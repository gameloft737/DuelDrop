using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PauseMenu : MonoBehaviour
{
    public static PauseMenu instance;
    public GameObject pauseMenu;
    public bool gameIsPaused;
    public AudioSource bgm;
    private void Awake()
    {
        // Singleton setup
        if (instance == null)
        {
            instance = this;
        }
        else
        {
            Destroy(gameObject);
            return;
        }
    }
    public void PauseGame(){
        bgm.volume = 0.2f;
        bgm.pitch = 0.5f;
        pauseMenu.SetActive(true);
        gameIsPaused = true;
        Time.timeScale = 0f;
    }
    public void UnpauseGame(){
        
        bgm.volume = 0.5f;
        bgm.pitch = 1f;
        pauseMenu.SetActive(false);
        gameIsPaused = false;
        Time.timeScale = 1f;
    }
}
