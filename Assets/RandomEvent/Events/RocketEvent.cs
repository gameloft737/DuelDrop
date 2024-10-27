using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Mathematics;


public class RocketEvent : EventAction
{
    [SerializeField] Transform Lanes;
    public override void EventTrigger()
    {
        Lanes = GameObject.FindGameObjectWithTag("Lanes").transform;
        GameObject SpawnPoint = Lanes.GetChild(UnityEngine.Random.Range(0, Lanes.childCount)).gameObject;
        GameObject Rocket = GameObject.FindGameObjectWithTag("Rocket");
        Rocket.transform = SpawnPoint.transform;
    }
}
