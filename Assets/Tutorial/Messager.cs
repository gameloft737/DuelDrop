using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class Messager : MonoBehaviour
{
    [SerializeField] private InputAction next;
    [SerializeField] private Message[] messages;
    [SerializeField]private int currentIndex = 0;
    private Message currentMessage;
    public void Awake(){
        next.Enable();
        next.performed += Next;
        currentMessage = messages[currentIndex];
    }
    public void Next(InputAction.CallbackContext context)
    {
        if(context.performed){
            if(currentMessage.isComplete){
                NextMessage();
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
