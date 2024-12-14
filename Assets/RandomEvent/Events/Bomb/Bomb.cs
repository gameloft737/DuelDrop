using System.Collections;
using System.Collections.Generic;
using Cinemachine;
using Unity.Mathematics;
using UnityEngine;

public class Bomb : EventAction
{
    [SerializeField]private float radius;
    [SerializeField]private float damage;
    [SerializeField]private float maxKnockback;
    [SerializeField] private GameObject explosion;
    [SerializeField] private float bombRange;
    public override void EventTrigger()
    {
        AudioManager.instance.Play("BombTicking");
        StartCoroutine(ApplyKnockbackAfterDelay());
    }

    private IEnumerator ApplyKnockbackAfterDelay()
    {
        // Wait for 5 seconds
        yield return new WaitForSeconds(5f);
        
        AudioManager.instance.Play("BombExplosion");
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
