using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;


[CreateAssetMenu(fileName = "RandomEvent", menuName = "Custom/RandomEvent")]
public class RandomEvent : ScriptableObject 
{
    public String eventName;
    public Sprite eventIcon;
    public GameObject eventPrefab;
    private EventAction eventAction;
    public void Start(){
        eventAction = eventPrefab.GetComponent<EventAction>();
    }
    public void Trigger()
    {
        if (eventPrefab != null)
        {
            // Instantiate the prefab in the scene
            GameObject spawnedObject = Instantiate(eventPrefab);
            // Try to get the EventAction component from the spawned object
            eventAction = spawnedObject.GetComponent<EventAction>();

            if (eventAction != null)
            {
                // Call the event trigger method
                eventAction.EventTrigger();
            }
        }
    
    }
}
