using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HealthDropEvent : EventAction
{
    [SerializeField] Transform environment;
    [SerializeField] private Collider objectCollider;
    [SerializeField] private Collider specificGroundCollider;
    protected override IEnumerator CreateEvent()
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
            yield return null;
             eventObject.SetActive(true);
            // Set the position of the object
            GetComponent<Rigidbody>().position = newPosition;
            transform.position = newPosition;
        }
        specificGroundCollider = selectedPlatform.GetComponent<Collider>();
        Destroy(gameObject, 10f);
        yield break;
    }
    private void OnTriggerEnter(Collider other){
        Debug.Log(other);
        if(other.transform.parent == null){
            return;
        }
        if(other.transform.parent.tag.Equals("WASDPlayer")){
            wasdManager.healthSystem.Heal(80,2);
            Destroy(gameObject);
        }
        else if(other.transform.parent.tag.Equals("ArrowKeysPlayer")){
            arrowKeysManager.healthSystem.Heal(80,2);
            Destroy(gameObject);
        }
    }
   
}