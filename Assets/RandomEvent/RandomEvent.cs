using System;
using System.Collections;
using System.Collections.Generic;
using Unity.Mathematics;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;

public class RandomEvent : ScriptableObject 
{
    public String eventName;
    public Sprite eventIcon;
    public GameObject eventPrefab;
    protected EventAction eventAction;
    public void Start(){
        eventAction = eventPrefab.GetComponent<EventAction>();
    }
    public void Trigger()
    {
        if (eventPrefab != null)
        {
            // Instantiate the prefab in the scene
            GameObject spawnedObject = Instantiate(eventPrefab, FindLocation(), FindRotation());
            // Try to get the EventAction component from the spawned object
            eventAction = spawnedObject.GetComponent<EventAction>();

            if (eventAction != null)
            {
                // Call the event trigger method
                eventAction.EventTrigger();
            }
        }
    }
    protected virtual Vector3 FindLocation(){
        return Vector3.zero;
    }
    protected virtual Quaternion FindRotation(){
        return Quaternion.identity;
    }
}
