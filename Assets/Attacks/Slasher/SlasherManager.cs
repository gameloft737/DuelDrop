using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SlasherManager : WeaponManager
{
    protected override void PerformSpecialAttack(AttackData attack)
    {
        if (target != null)
        {
            _playerMovement.canMove = false;
            _playerMovement.Floor();
            StartCoroutine(Smash(1.5f, attack));
            
        }
    }
    protected IEnumerator Smash(float delay, AttackData attack)
    {
        yield return new WaitForSeconds(delay);
        _playerMovement.canMove = true;
        if (target != null)
        {
            // Instantiate the claw effect at the player's position
            GameObject particleEffect = Instantiate(attack.getParticle(0), transform.position, Quaternion.identity, transform);

            // Schedule destruction of the claw effect just before the attack reloads
            StartCoroutine(DestroyParticleEffect(particleEffect, attack.reloadSpeed));


            // Check if the target is within the knockback range
            float knockbackRange = attack.range;
            float distanceToTarget = Vector3.Distance(transform.position, target.position);

            // Only apply knockback if the target is within range and the player is facing the target
            if (distanceToTarget <= knockbackRange)
            {
                float knockbackStrength = attack.knockback;
                if (targetManager != null)
                {
                    targetManager.ApplyKnockback(transform.position, knockbackStrength * knockbackModifier ,knockbackStrength*0.2f, attack.damage);
                }
            }
        }
    }
}
