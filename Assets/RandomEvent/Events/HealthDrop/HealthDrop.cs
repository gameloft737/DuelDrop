using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HealthDrop : EventAction
{
    public override void EventTrigger()
    {
        Destroy(gameObject, 10f);
    }
    private void OnTriggerEnter(Collider other){
        Debug.Log(other);
        if(other.transform.parent == null){
            return;
        }
        if(other.transform.parent.tag.Equals("WASDPlayer")){
            wasdManager.healthSystem.Heal(80,2);
            Destroy(gameObject);
        }
        else if(other.transform.parent.tag.Equals("ArrowKeysPlayer")){
            arrowKeysManager.healthSystem.Heal(80,2);
            Destroy(gameObject);
        }
    }
   
}