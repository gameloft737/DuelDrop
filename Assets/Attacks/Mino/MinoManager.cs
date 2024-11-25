using System;
using System.Collections;
using UnityEngine;
using UnityEngine.UIElements;

public class MinoManager : WeaponManager
{ 
    protected override IEnumerator TryPerformAttack(AttackData attack){
        if (isAttacking)
        {
            Debug.Log("Already attacking, wait for the attack to finish.");
            yield break;
        }
        if (attackCooldowns[attack] <= 0f)
        {    
            isAttacking = true; // Mark as attacking
            _playerMovement.animator.SetTrigger("attack");
            AudioManager.instance.Play("MinoAttack");
            yield return new WaitForSeconds(attack.delay);

            PerformAttack(attack);

            // Start cooldown after the attack
            attackCooldowns[attack] = attack.reloadSpeed;
            isAttacking = false; // Mark attack as finished
        }
        else
        {
            Debug.Log("Attack on cooldown!");
        }

        yield return null; // Wait for the next frame
    }
    protected override void PerformSpecialAttack(AttackData attack)
    {
        GameObject particleS = Instantiate(attack.getParticle(1), transform.position, transform.rotation, transform);
        StartCoroutine(DestroyParticleEffect(particleS, 1f));
        StartCoroutine(SpawnProjectile(attack, 1f));

    }
    private IEnumerator SpawnProjectile(AttackData attack, float delay){
        yield return new WaitForSeconds(delay);
        GameObject shot = Instantiate(attack.getParticle(0), transform.position, transform.rotation);
        Vector3 directionToTarget = (target.position - transform.position).normalized;
        shot.transform.forward = directionToTarget;
        MoveVelocity moveVelocity = shot.GetComponentInChildren<MoveVelocity>();
        if (moveVelocity != null)
        {
            moveVelocity.velocity = directionToTarget * attack.range;
            moveVelocity.StartMovement();
        }
        else{Debug.Log("NULL");}

        InstantKnockback knockback = shot.GetComponentInChildren<InstantKnockback>();
        if (knockback != null)
        {
            knockback.targetManager = targetManager;
            knockback.attackData = attack;
        }
        else{Debug.Log("NULL Knockback");}
        Destroy(shot, 10f );
    }
}

