using System.Collections;
using System.Collections.Generic;
using UnityEditor.Experimental.GraphView;
using UnityEngine;

public class FynManager : WeaponManager
{
    private bool isShieldActive = false; // Tracks if the shield is active
    [SerializeField]private float shieldDuration = 3f; // Example shield duration (you can adjust this)
    private float knockbackReduction = 0.5f; // 50% reduction in knockback when the shield is active
    private float moveSpeed = 1f;

    private Coroutine shieldCoroutine;

    // Override the special attack method to activate the shield
    protected override void PerformSpecialAttack(AttackData attack)
    {
        if (shieldCoroutine == null)
        {
            Debug.Log("Shield activated!");
            isShieldActive = true;
            GameObject particleEffect = Instantiate(attack.particle, transform.position, Quaternion.identity, transform);

            // Schedule destruction of the claw effect just before the attack reloads
            StartCoroutine(DestroyParticleEffect(particleEffect, shieldDuration));
            // Start the shield duration countdown
            shieldCoroutine = StartCoroutine(DeactivateShieldAfterDuration(shieldDuration));

            // Call base PerformSpecialAttack() if needed for additional effects, or omit this line if special attack is only shield
            base.PerformSpecialAttack(attack);


        }
    }

    // Coroutine to deactivate the shield after a set duration
    private IEnumerator DeactivateShieldAfterDuration(float duration)
    {
        yield return new WaitForSeconds(duration);
        isShieldActive = false;
        shieldCoroutine = null;
        Debug.Log("Shield deactivated!");
    }

    // Method to apply knockback when the player is hit
    public override void ApplyKnockback(Vector3 attackPosition, float knockbackStrength, float knockupStrength,WeaponManager attacker)
    {
        if (isShieldActive)
        {
            // Reduce the knockback received by 50% and reflect the rest
            float reducedKnockback = knockbackStrength * knockbackReduction;
            _playerMovement.Knockback(attackPosition, reducedKnockback);
            if(knockupStrength > 0f){ 
                _playerMovement.Knockup(knockupStrength  * knockbackReduction);
            }
            // Reflect the remaining knockback to the attacker
            if (attacker != null)
            {
                float reflectedKnockback = knockbackStrength * (1 - knockbackReduction);
                attacker._playerMovement.Knockback(transform.position, reflectedKnockback);
                Debug.Log(attacker._playerMovement);
            }
        }
        else
        {
            // No shield active, apply full knockback to the player
            _playerMovement.Knockback(attackPosition, knockbackStrength);
            if(knockupStrength > 0f){ 
                _playerMovement.Knockup(knockupStrength);
            }
        }
    }
    protected override void PerformUltimateAttack(AttackData attack)
    {
        if(target != null)
        {
            Debug.Log("Ultimate Attack Performed");
            Vector3 currentPosition = transform.position;
            Vector3 targetPosition = target.position;
            transform.position = Vector3.MoveTowards(transform.position, currentPosition, moveSpeed * Time.deltaTime);
        }
    }
}
