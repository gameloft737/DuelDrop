using System.Collections;
using UnityEngine;
using UnityEngine.AI;

public class NecroManager : WeaponManager
{
    [SerializeField] private float lifeDrainTime = 4f;
    [SerializeField] private GameObject skeleton;
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
            AudioManager.instance.Play("NecroAttack");
            attackCooldowns[attack] = attack.reloadSpeed; // Set the cooldown based on reloadSpeed

            yield return new WaitForSeconds(attack.delay);
            GameObject particleEffect = Instantiate(attack.getParticle(1), transform.position, Quaternion.identity, transform);
            particleEffect.transform.localScale = Vector3.one;
            StartCoroutine(DestroyParticleEffect(particleEffect, attack.reloadSpeed));

            PerformAttack(attack);

            yield return new WaitForSeconds(0.3f);
            particleEffect = Instantiate(attack.getParticle(2), transform.position, Quaternion.identity, transform);
            particleEffect.transform.localScale = Vector3.one;
            StartCoroutine(DestroyParticleEffect(particleEffect, attack.reloadSpeed));

            PerformAttack(attack);
            
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
        
        _playerMovement.animator.SetTrigger("special");
        
        AudioManager.instance.Play("NecroSpecialAttack");
        targetManager.healthSystem.Damage(attack.damage,lifeDrainTime);
        healthSystem.Heal(attack.damage, lifeDrainTime);
        
        GameObject particleEffect = Instantiate(attack.getParticle(0), transform.position, Quaternion.identity, transform);
        particleEffect.transform.localScale = Vector3.one;
        particleEffect.GetComponentInChildren<ParticleLine>().startPoint = target;
        particleEffect.GetComponentInChildren<ParticleLine>().endPoint = transform;
        StartCoroutine(DestroyParticleEffect(particleEffect,lifeDrainTime));
    }
    protected override void PerformUltimateAttack(AttackData attack)
    {
        StartCoroutine(CreateSkeletons(2f, attack));
    }
    private IEnumerator CreateSkeletons(float delay, AttackData attack)
    {
        GameObject skeletonObj;
        InstantKnockback skeletonKnockback;
        AgentTracker skeletonAgent;

        for (int i = 0; i < 3; i++)
        {
            _playerMovement.animator.SetTrigger("ultimate");
            AudioManager.instance.Play("NecroUltimateAttack");

            yield return new WaitForSeconds(attack.delay);

            // Define the desired spawn position
            Vector3 spawnPosition = transform.position;

            // Find the closest point on the NavMesh
            NavMeshHit hit;
            if (NavMesh.SamplePosition(spawnPosition, out hit, 25f, NavMesh.AllAreas))
            {
                // Instantiate the skeleton at the nearest valid position on the NavMesh
                skeletonObj = Instantiate(skeleton, hit.position, Quaternion.identity);
                
                skeletonKnockback = skeletonObj.GetComponent<InstantKnockback>();
                skeletonAgent = skeletonObj.GetComponent<AgentTracker>();
                skeletonAgent.target = targetManager.transform;
                skeletonKnockback.attackData = attack;
                skeletonKnockback.targetManager = targetManager;
                skeletonKnockback.thisManager = this;
            }
            else
            {
                Debug.LogWarning("No valid NavMesh point found for the spawn position.");
            }

            yield return new WaitForSeconds(delay);
        }
    }
}
