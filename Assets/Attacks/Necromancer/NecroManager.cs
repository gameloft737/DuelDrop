using System.Collections;
using System.Collections.Generic;
using Unity.PlasticSCM.Editor.WebApi;
using UnityEngine;

public class NecroManager : WeaponManager
{
    [SerializeField] private float lifeDrainTime = 4f;
    [SerializeField] private GameObject skeleton;
    protected override void PerformSpecialAttack(AttackData attack)
    {
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
    private IEnumerator CreateSkeletons(float delay, AttackData attack){
        GameObject skeletonObj;
        InstantKnockback skeletonKnockback;
        AgentTracker skeletonAgent;
        yield return new WaitForSeconds(delay);
        for(int i = 0; i < 3; i++){
            skeletonObj = Instantiate(skeleton, transform.position, Quaternion.identity);
            skeletonKnockback = skeletonObj.GetComponent<InstantKnockback>();
            skeletonAgent = skeletonObj.GetComponent<AgentTracker>();
            skeletonAgent.target = targetManager.transform;
            skeletonKnockback.attackData=attack;
            skeletonKnockback.targetManager = targetManager;
            skeletonKnockback.thisManager = this;
            yield return new WaitForSeconds(delay);
        }
    }
}
