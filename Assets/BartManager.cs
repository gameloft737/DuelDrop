using System.Collections;
using UnityEngine;
using UnityEngine.AI;

public class BartManager : WeaponManager
{
    [SerializeField] private float lifeDrainTime = 4f;
    [SerializeField] private GameObject attackObj;
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
            AudioManager.instance.Play("BartAttack");
            attackCooldowns[attack] = attack.reloadSpeed; // Set the cooldown based on reloadSpeed

            yield return new WaitForSeconds(attack.delay);
            GameObject particleEffect = Instantiate(attack.getParticle(1), transform.position, Quaternion.identity, transform);
            particleEffect.transform.localScale = Vector3.one;
            StartCoroutine(DestroyParticleEffect(particleEffect, attack.reloadSpeed));

            PerformAttack(attack);
            
            isAttacking = false; // Mark attack as finished
        }
        else
        {
            AnimateSlider(attack);
            Debug.Log("Attack on cooldown!");
        }

        yield return null; // Wait for the next frame
    }
    protected override void PerformAttack(AttackData attack)
    {
        if (target != null)
        {
            Vector3 directionToTarget = (target.position - transform.position).normalized;
            GameObject attacker = Instantiate(attackObj, transform.position, Quaternion.identity);
            attacker.GetComponent<Rigidbody>().AddForce(directionToTarget * attack.range, ForceMode.Impulse);
            InstantKnockback kb = attacker.GetComponent<InstantKnockback>();
            kb.targetManager = targetManager;
            kb.thisManager = this;
            kb.attackData = attack;

        }
    }
    protected override void PerformSpecialAttack(AttackData attack)
    {
        
    }
    protected override void PerformUltimateAttack(AttackData attack)
    {
    }
}
