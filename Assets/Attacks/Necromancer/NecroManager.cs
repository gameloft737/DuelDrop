using System.Collections;
using System.Collections.Generic;
using Unity.PlasticSCM.Editor.WebApi;
using UnityEngine;

public class NecroManager : WeaponManager
{
    [SerializeField] private float lifeDrainTime = 4f;
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
}
