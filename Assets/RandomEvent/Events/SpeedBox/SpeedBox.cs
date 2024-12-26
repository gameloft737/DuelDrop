using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpeedBox: EventAction
{
    public GameObject particle;
    public override void EventTrigger()
    {
        Destroy(gameObject, 10f);
    }
    private void OnTriggerEnter(Collider other){
        
        AudioManager.instance.Play("SpeedBox");
        if(other.transform.parent == null){
            return;
        }
        if(other.transform.parent.tag.Equals("WASDPlayer")){
            wasdManager._playerMovement.SetSpeedBoost(2f,6,particle);
            Destroy(gameObject);
        }
        else if(other.transform.parent.tag.Equals("ArrowKeysPlayer")){
            arrowKeysManager._playerMovement.SetSpeedBoost(2f,6,particle);
            Destroy(gameObject);
        }
    }
   
}