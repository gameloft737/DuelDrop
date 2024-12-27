using System.Collections;
using UnityEngine;
using UnityEngine.AI;

public class BartManager : WeaponManager
{
    [SerializeField] private GameObject attackObj;
    [SerializeField] private GameObject ultimateAttackObj; 
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
    [SerializeField] Transform originPoint;

protected override void PerformAttack(AttackData attack)
{
    if (target != null){

        // Instantiate the object at the origin point with the origin's rotation
        GameObject attacker = Instantiate(attackObj, originPoint.position, originPoint.rotation);

        // Apply force to make it shoot out
        Rigidbody rb = attacker.GetComponent<Rigidbody>();
         if (rb != null)
        {
            Vector3 forceDirection = _playerMovement.isRight ? originPoint.right : -originPoint.right;
            rb.AddForce(forceDirection * attack.range, ForceMode.Impulse);
        }

        // Configure InstantKnockback component
        InstantKnockback kb = attacker.GetComponent<InstantKnockback>();
        if (kb != null)
        {
            kb.targetManager = targetManager;
            kb.thisManager = this;
            kb.attackData = attack;
        }
    }
}

    protected override void PerformSpecialAttack(AttackData attack)
    {
        if (target != null)
        {
            _playerMovement.animator.SetTrigger("special");
            isFrozen = true;
            _playerMovement.animator.SetBool("isSpecial", true);
            _playerMovement.isFrozen = true;
            _playerMovement.Floor();    
            StartCoroutine(Healer(2.2f, attack));
            
            healthSystem.Heal(attack.damage,1f);
            
            healthSystem.isFrozen = true;
            AudioManager.instance.Play("BartSpecialAttack");
            GameObject particleEffect = Instantiate(attack.getParticle(0), transform.position, Quaternion.identity, transform);
            
            StartCoroutine(DestroyParticleEffect(particleEffect, 2));
            
        }   
    }
    protected IEnumerator Healer(float delay, AttackData attack)
    {
        yield return new WaitForSeconds(delay);
        _playerMovement.isFrozen = false;
        
            isFrozen = false;
            _playerMovement.animator.SetBool("isSpecial", false);
            healthSystem.isFrozen = false;
        
    }
    protected override void PerformUltimateAttack(AttackData attack)
    {
        if (target != null){
            
        // Instantiate the object at the origin point with the origin's rotation
        GameObject attacker = Instantiate(ultimateAttackObj, transform.position, Quaternion.identity);

        // Apply force to make it move towards the target
        Rigidbody rb = attacker.GetComponent<Rigidbody>();
        if (rb != null)
        {
            // Calculate the direction towards the target
            Vector3 directionToTarget = (target.position - transform.position).normalized;

            // Apply force in the direction of the target
            rb.AddForce(directionToTarget * attack.range, ForceMode.Impulse);
        }


        // Configure InstantKnockback component
        Potion potion = attacker.GetComponent<Potion>();
        if (potion != null)
        {
            potion.playerMovement = target.GetComponent<PlayerMovement>();
            potion.particleEffect = attack.getParticle(0);
            potion.attackData = attack;
            potion.weaponManager = targetManager;
        }
    }
    }
    
    protected override IEnumerator TryPerformUltimateAttack(AttackData attack)
    {
        if (isUltimateAttacking)
        {
            Debug.Log("Already performing an ultimate attack, wait for it to finish.");
            yield break;
        }

        if (attackCooldowns[attack] <= 0f)
        {
            isTutorial = 0;
            isUltimateAttacking = true; // Mark as performing ultimate attack
            attackCooldowns[attack] = attack.reloadSpeed;
            
        AudioManager.instance.Play("BartUltimateAttack");
        
            _playerMovement.animator.SetTrigger("ultimate");
            yield return new WaitForSeconds(attack.delay);

            PerformUltimateAttack(attack);
            isUltimateAttacking = false; // Reset flag after attack
        }
        else
        {
            AnimateSlider(attack);
            Debug.Log("UltimateAttack on cooldown!");
        }

        yield return null;
    }
    
}
