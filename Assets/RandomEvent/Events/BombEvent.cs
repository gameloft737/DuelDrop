using System.Collections;
using System.Collections.Generic;
using Cinemachine;
using Unity.Mathematics;
using UnityEngine;

public class BombEvent : EventAction
{
    [SerializeField]private float radius;
    [SerializeField]private float damage;
    [SerializeField]private float maxKnockback;
    [SerializeField] Transform environment;
    [SerializeField] private Collider objectCollider;
    [SerializeField] private GameObject explosion;
    [SerializeField] private Collider specificGroundCollider;
    [SerializeField] private float bombRange;
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
            
        // Teleport the bombObject to the new position
        StartCoroutine(ApplyKnockbackAfterDelay());
        
    }
    public void FixedUpdate(){
        float yDistance = Mathf.Abs(transform.position.y - specificGroundCollider.transform.position.y);
        if (yDistance < 2f)
        {
            objectCollider.enabled = true;
        }

    }

    private IEnumerator ApplyKnockbackAfterDelay()
    {
        // Wait for 5 seconds
        yield return new WaitForSeconds(5f);
        Instantiate(explosion, transform.position, quaternion.identity);

        // Check the distance between the bomb and manager
        if (arrowKeysManager != null)
        {
            float distanceToManager1 = Vector3.Distance(transform.position, arrowKeysManager.transform.position);
            if (distanceToManager1 < bombRange)
            {
                ApplyKnockbackBasedOnDistance(arrowKeysManager, distanceToManager1);
                
                CameraShakeManager.instance.CameraShake(GetComponent<CinemachineImpulseSource>());
            }
        }

        // Check the distance between the bomb and manager
        if (wasdManager != null)
        {
            float distanceToManager2 = Vector3.Distance(transform.position, wasdManager.transform.position);
            if (distanceToManager2 < bombRange)
            {
                ApplyKnockbackBasedOnDistance(wasdManager, distanceToManager2);
                
                CameraShakeManager.instance.CameraShake(GetComponent<CinemachineImpulseSource>());

            }
            Destroy(gameObject);
        }
    }
    private void ApplyKnockbackBasedOnDistance(WeaponManager targetManager, float distance)
    {
        // If the distance is within the radius, calculate the knockback percentage
        if (distance <= radius)
        {
            float knockbackPercentage = Mathf.Clamp01(1 - (distance / radius));
            float knockbackForce = maxKnockback * knockbackPercentage;

            // Apply knockback to the target manager
            targetManager.ApplyKnockback(transform.position, knockbackForce + 5f, knockbackForce* 0.3f, damage);
        }
    }

    private void OnDrawGizmos()
    {
        // Draw the radius in the editor for visualization
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, radius);
    }

}
