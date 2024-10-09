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
    }
}
