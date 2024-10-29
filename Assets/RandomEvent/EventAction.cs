using System.Collections;
using System.Collections.Generic;
using Unity.IO.LowLevel.Unsafe;
using UnityEngine;

public class EventAction : MonoBehaviour
{
   protected WeaponManager arrowKeysManager;
   protected WeaponManager wasdManager;
   [SerializeField]protected GameObject eventObject; 
   public void Start(){
      if(eventObject != null){
         eventObject.SetActive(false);
      }
      arrowKeysManager = GameObject.FindGameObjectsWithTag("ArrowKeysManager")[0].GetComponent<WeaponManager>();
      wasdManager = GameObject.FindGameObjectsWithTag("WASDManager")[0].GetComponent<WeaponManager>();
   }
   public void EventTrigger(){
      if(eventObject != null){
         eventObject.SetActive(true);
      }
      StartCoroutine(CreateEvent());
   }
   protected  virtual IEnumerator CreateEvent(){
      yield break;
   }
    
}
