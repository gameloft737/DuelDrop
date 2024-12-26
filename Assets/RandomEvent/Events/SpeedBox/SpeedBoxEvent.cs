using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "RandomEvent", menuName = "RandomEvent/SpeedBoostEvent")]
public class SpeedBoostEvent : RandomEvent
{
    [SerializeField] private float spawnRadius;
    protected override Vector3 FindLocation(){
        spawnRadius = EnvironmentManager.instance.GetBaseLength(2f)/2;
        Vector3 newPosition = new Vector3(UnityEngine.Random.Range(-spawnRadius, spawnRadius), 20f, 0);
        return newPosition;
    }
}
