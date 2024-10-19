using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SlasherManager : WeaponManager
{
    int randomNum = 1;
    [SerializeField] GameObject swordParticles;
    protected override IEnumerator TryPerformAttack(AttackData attack){
        if (attackCooldowns[attack] <= 0f)
        {    
            randomNum = UnityEngine.Random.Range(1, 3);
            _playerMovement.animator.SetTrigger("attack" + randomNum);
            attackCooldowns[attack] = attack.reloadSpeed; // Set the cooldown based on reloadSpeed
            yield return new WaitForSeconds(attack.delay);
            PerformAttack(attack);
        }
        else
        {
            Debug.Log("Attack on cooldown!");
        }

        yield return null; // Wait for the next frame
    }
    protected override void PerformAttack(AttackData attack)
    {
        if (target != null)
        {
            // Instantiate the claw effect at the player's position
            GameObject particleEffect = Instantiate(attack.getParticle(randomNum), transform.position, Quaternion.identity, transform);
            particleEffect.transform.localScale = Vector3.one;
            // Schedule destruction of the claw effect just before the attack reloads
            StartCoroutine(DestroyParticleEffect(particleEffect, attack.reloadSpeed *2f));

            // Calculate the direction from the player to the target
            Vector3 directionToTarget = (target.position - transform.position).normalized;

            // Check if the target is within the knockback range
            float knockbackRange = attack.range;
            float distanceToTarget = Vector3.Distance(transform.position, target.position);

            // Calculate the player's facing direction based on the character's local scale
            Vector3 facingDirection = _playerMovement.characterColliderObj.localScale.x > 0 ? transform.right : -transform.right;

            // Only apply knockback if the target is within range and the player is facing the target
            if (distanceToTarget <= knockbackRange && Vector3.Dot(facingDirection, directionToTarget) > 0)
            {
                float knockbackStrength = attack.knockback;
                if (targetManager != null)
                {
                    targetManager.ApplyKnockback(transform.position, knockbackStrength * knockbackModifier,0.1f, attack.damage);
                }
            }
        }
    }
    protected override void PerformSpecialAttack(AttackData attack)
    {
        if (target != null)
        {
            _playerMovement.animator.SetBool("isSpecial", true);
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
            _playerMovement.animator.SetBool("isSpecial", false);
        }
    }
    protected override void PerformUltimateAttack(AttackData attack)
    {
         _playerMovement.animator.SetTrigger("ultimate");
         swordParticles.SetActive(true);
        if (target != null)
        {
            StartCoroutine(SetSwordParticles(2.5f, false));
        }
    }
    protected IEnumerator SetSwordParticles(float delay, bool value){
        yield return new WaitForSeconds(delay);
        swordParticles.SetActive(value);
    }
}
