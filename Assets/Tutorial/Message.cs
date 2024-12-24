using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class Message : MonoBehaviour
{
    public bool isComplete;
    [SerializeField] GameObject messageObj;
    public virtual void DisableMessage(){
        messageObj.SetActive(false);
    }
    public virtual void EnableMessage(){
        messageObj.SetActive(true);
        Debug.Log("enabled");
    }
}
