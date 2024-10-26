using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FaceTowards : MonoBehaviour
{
    public Transform target; // The target the object should face towards.

    // Update is called once per frame
    void Update()
    {
        if (target != null)
        {
            // Calculate the direction vector from the object to the target.
            Vector3 direction = target.position - transform.position;

            // If the direction vector is not zero, adjust the scale.
            if (direction != Vector3.zero)
            {
                // Create a target rotation that only affects the Y-axis.
                Quaternion targetRotation = Quaternion.LookRotation(direction);

                // Smoothly rotate towards the target rotation.
                transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, Time.deltaTime * 5f);
            }
        }
    }
}
