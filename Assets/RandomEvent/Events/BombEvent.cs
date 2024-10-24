using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class BombEvent : EventAction
{
    [SerializeField]private float radius;
    [SerializeField]private float damage;
    [SerializeField]private float maxKnockback;
    [SerializeField]private LayerMask groundLayer; 
    [SerializeField] private Collider objectCollider;
    public override void EventTrigger()
    {
        Vector3 randomPoint = new Vector3(Random.Range(-20, 20), 7.5f, 0);

        // Step 2: Ensure the point is on the NavMesh
        NavMeshHit hit;
        if (NavMesh.SamplePosition(randomPoint, out hit, 20f, NavMesh.AllAreas))
        {
            // Get the exact position on the NavMesh
            Vector3 navMeshPoint = hit.position;
            Vector3 newPosition = new Vector3(navMeshPoint.x, 20f, 0);
            
            RaycastHit groundHit;
            if (Physics.Raycast(navMeshPoint + Vector3.up * 1f, Vector3.down, out groundHit, 20f, groundLayer))
            {
                // Store the specific ground collider directly below the NavMesh point
                Collider specificGroundCollider = groundHit.collider;
            }
            transform.position= newPosition;
            // Teleport the bombObject to the new position
            StartCoroutine(ApplyKnockbackAfterDelay());
        }
    }

    private IEnumerator ApplyKnockbackAfterDelay()
    {
        // Wait for 3 seconds
        yield return new WaitForSeconds(3f);

        // Check the distance between the bomb and manager1
        if (arrowKeysManager != null)
        {
            float distanceToManager1 = Vector3.Distance(transform.position, arrowKeysManager.transform.position)+ 10f;
            ApplyKnockbackBasedOnDistance(arrowKeysManager, distanceToManager1);
        }

        // Check the distance between the bomb and manager2
        if (wasdManager != null)
        {
            float distanceToManager2 = Vector3.Distance(transform.position, wasdManager.transform.position) + 10f;
            ApplyKnockbackBasedOnDistance(wasdManager, distanceToManager2);
        }
        Destroy(gameObject);
    }

    private void ApplyKnockbackBasedOnDistance(WeaponManager targetManager, float distance)
    {
        // If the distance is within the radius, calculate the knockback percentage
        if (distance <= radius)
        {
            float knockbackPercentage = Mathf.Clamp01(1 - (distance / radius));
            float knockbackForce = maxKnockback * knockbackPercentage;

            // Apply knockback to the target manager
            targetManager.ApplyKnockback(transform.position, knockbackForce, 0.1f, damage);
        }
    }

    private void OnDrawGizmos()
    {
        // Draw the radius in the editor for visualization
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, radius);
    }

}
