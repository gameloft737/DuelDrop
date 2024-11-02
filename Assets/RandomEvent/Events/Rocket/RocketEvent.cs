using System.Collections;
using System.Collections.Generic;
using UnityEngine;


[CreateAssetMenu(fileName = "RandomEvent", menuName = "RandomEvent/RocketEvent")]
public class RocketEvent : RandomEvent
{
    private Transform spawnPoint;
    protected override Vector3 FindLocation(){
        Transform lanes = GameObject.FindGameObjectWithTag("Lanes").transform;
        spawnPoint = lanes.GetChild(UnityEngine.Random.Range(0, lanes.childCount));
        return spawnPoint.position;
    }
    protected override Quaternion FindRotation(){
        return spawnPoint.rotation;
    }
    protected override Transform FindParent(){
        return spawnPoint;
    }
}
