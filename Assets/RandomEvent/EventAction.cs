using System.Collections;
using System.Collections.Generic;
using Unity.IO.LowLevel.Unsafe;
using UnityEngine;

public abstract class EventAction : MonoBehaviour
{
   protected WeaponManager arrowKeysManager;
   protected WeaponManager wasdManager;
   public virtual void StopEvent(){}
   public void Start(){
      arrowKeysManager = GameObject.FindGameObjectsWithTag("ArrowKeysManager")[0].GetComponent<WeaponManager>();
      wasdManager = GameObject.FindGameObjectsWithTag("WASDManager")[0].GetComponent<WeaponManager>();
   }
   public abstract void EventTrigger();
    
}
