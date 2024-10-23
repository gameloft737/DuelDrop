using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EventAction : MonoBehaviour
{
   protected WeaponManager arrowKeysManager;
   protected WeaponManager wasdManager;
   public void Start(){
      arrowKeysManager = GameObject.FindGameObjectsWithTag("ArrowKeysManager")[0].GetComponent<WeaponManager>();
      wasdManager = GameObject.FindGameObjectsWithTag("WASDManager")[0].GetComponent<WeaponManager>();
   }
   public virtual void EventTrigger(){

   }
    
}
