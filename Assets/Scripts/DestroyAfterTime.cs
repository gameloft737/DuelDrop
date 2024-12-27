using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class DestroyAfterTime : MonoBehaviour
{
    public float time = 5f;
    public bool isGroundCheck = false;
    public LayerMask groundLayer;
    void Start()
    {
        Destroy(gameObject, time);
    }
    private void OnTriggerEnter(Collider collider){
        if (isGroundCheck)
        {
            // Check if the collided object's layer matches the groundLayer
            if (((1 << collider.gameObject.layer) & groundLayer) != 0)
            {
                Destroy(gameObject);
            }
        }
    }

}

