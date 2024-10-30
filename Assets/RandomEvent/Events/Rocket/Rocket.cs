using System.Collections;
using System.Collections.Generic;
using System.Runtime.InteropServices.WindowsRuntime;
using UnityEditor.Search;
using UnityEngine;
using UnityEngine.UIElements;

public class Rocket : EventAction
{
    [SerializeField] GameObject rocket;
    [SerializeField] float speed = 20f;
    public override void EventTrigger()
    {
        MoveVelocity velocity = rocket.GetComponent<MoveVelocity>();
        if (transform.position.x < 0)
        {
            velocity.velocity = new Vector3(speed,0,0);
        }
        else
        {
            velocity.velocity = new Vector3(-speed,0,0);
        } 
    }
}
