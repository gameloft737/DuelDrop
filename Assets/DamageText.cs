using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DamageText : MonoBehaviour
{
    [SerializeField] float duration;
    [SerializeField] Vector3 offset;
    [SerializeField] Vector3 randomizeIntensity;
    [SerializeField] float randomRotationRange = 15f; // Range for random rotation

    // Start is called before the first frame update
    void Start()
    {
        // Destroy the text object after a certain duration
        Destroy(gameObject, duration);

        // Apply a random position offset
        transform.position += offset;
        transform.position += new Vector3(
            Random.Range(-randomizeIntensity.x, randomizeIntensity.x),
            Random.Range(-randomizeIntensity.y, randomizeIntensity.y),
            Random.Range(-randomizeIntensity.z, randomizeIntensity.z)
        );

        // Apply a random rotation around the Z-axis (XY plane)
        float randomZRotation = Random.Range(-randomRotationRange, randomRotationRange);
        transform.Rotate(0, 0, randomZRotation);
    }
}
