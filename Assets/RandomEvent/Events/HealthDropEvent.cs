using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HealthDropEvent : EventAction
{
    [SerializeField] Transform environment;
    [SerializeField] private Collider objectCollider;
    [SerializeField] private Collider specificGroundCollider;
    [SerializeField] private Collider groundCollider;
    private bool isActivated = false;
    public override void EventTrigger()
    {
        environment = GameObject.FindGameObjectWithTag("Environment").transform;

        // Select a random child (platform) from the environment
        GameObject selectedPlatform = environment.GetChild(UnityEngine.Random.Range(0, environment.childCount)).gameObject;

        // Get the BoxCollider of the selected platform
        BoxCollider platformCollider = selectedPlatform.GetComponent<BoxCollider>();

        if (platformCollider != null)
        {
            // Get the bounds of the BoxCollider
            Bounds platformBounds = platformCollider.bounds;

            // Generate a random X position within the platform's bounds
            float randomX = UnityEngine.Random.Range(platformBounds.min.x, platformBounds.max.x);

            // Define the new position with the random X, specified Y, and a Z value of 0
            Vector3 newPosition = new Vector3(randomX, 20f, 0);

            // Set the position of the object
            transform.position = newPosition;
        }
        specificGroundCollider = selectedPlatform.GetComponent<Collider>();
        StartCoroutine(PlayerChecking());
    }
    private IEnumerator PlayerChecking()
    {
        while(isActivated == false)
        {
            PlayerCheck();
            yield return null;
        }
        isActivated = false;
    }
    private void PlayerCheck()
    {
        float distanceToManager2 = Vector3.Distance(transform.position, wasdManager.transform.position);
        if (distanceToManager2 < 0.5) 
        {
            isActivated = true;
            wasdManager.healthSystem.Heal(80,5);
        }
        float distanceToManager1 = Vector3.Distance(transform.position, arrowKeysManager.transform.position);
        if (distanceToManager1 < 0.5)
        {
            isActivated = true;
            arrowKeysManager.healthSystem.Heal(80, 5);
        }
    }
}