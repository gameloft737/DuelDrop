using System.Collections;
using UnityEngine;
using UnityEngine.InputSystem;

public class Message : MonoBehaviour
{
    public bool isComplete;
    [SerializeField] protected GameObject messageObj;
    [SerializeField] protected GameObject proceedObj;
    public virtual void DisableMessage(){
        messageObj.SetActive(false);
    }
    public virtual void EnableMessage(){
        messageObj.SetActive(true);
    }
    public virtual void Warn(){
        return;
    }
    public void LateUpdate(){
        if(messageObj.activeSelf){proceedObj.SetActive(isComplete);}
    }
}
