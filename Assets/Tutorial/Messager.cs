using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;

public class Messager : MonoBehaviour
{
    [SerializeField] private InputAction next;
    [SerializeField] private Message[] messages;
    [SerializeField]private int currentIndex = 0;
    [SerializeField] private string sceneToLoad;
    private Message currentMessage;
    public void Start(){
        next.Enable();
        next.performed += Next;
        currentMessage = messages[currentIndex];
    if (SceneManager.GetActiveScene().name != "Tutorial") {
        Destroy(gameObject);
    }
    }

    public void Next(InputAction.CallbackContext context)
    {
        if(context.performed && SceneManager.GetActiveScene().name == "Tutorial"){
            if(currentMessage.isComplete){
                if(!NextMessage()){
                    Debug.Log("LOADING CHARACTER SELECT FROM: TUTORIAL");
                    SceneManager.LoadScene(sceneToLoad);
                }
            } else {
                currentMessage.Warn();
            }
        }
    }
    private bool NextMessage(){
        if(currentIndex >= messages.Length - 1){
            return false;
        }
        currentMessage.DisableMessage();
        currentIndex++;
        currentMessage = messages[currentIndex];
        currentMessage.EnableMessage();
        return true;
    }
}
