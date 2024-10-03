using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NecroManager : WeaponManager
{
    protected override void PerformSpecialAttack(AttackData attack)
    {
        
        targetManager.healthSystem.Damage(attack.damage);
    }
}
