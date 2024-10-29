using System.Collections;
using System.Collections.Generic;
using System.Runtime.InteropServices.WindowsRuntime;
using UnityEditor.Search;
using UnityEngine;
using UnityEngine.UIElements;

public class Rocket : EventAction
{
    [SerializeField] Transform Lanes;
    [SerializeField] GameObject rocket;
    public float Velocity;
    Vector3 Spawn;
    protected override IEnumerator CreateEvent()
    {
        Lanes = GameObject.FindGameObjectWithTag("Lanes").transform;
        GameObject SpawnPoints = Lanes.GetChild(UnityEngine.Random.Range(0, Lanes.childCount)).gameObject;
        Velocity = rocket.GetComponent<MoveVelocity>().velocity.x;
        if (SpawnPoints.tag == "Right Rocket Spawn")
        {
            Velocity = 1f;
            transform.position = SpawnPoints.transform.position;
            transform.rotation = SpawnPoints.transform.rotation;
        }
        if(SpawnPoints.tag == "Left Rocket Spawn")
        {
            Velocity = -1f;
            transform.position = SpawnPoints.transform.position;
            transform.rotation = SpawnPoints.transform.rotation;
        } 
        yield break;
    }
}
