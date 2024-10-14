using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class InstantKnockback : MonoBehaviour
{
    public WeaponManager targetManager;
    public WeaponManager thisManager;
    public AttackData attackData;
    private void OnTriggerEnter(Collider collider){
        if(collider.gameObject.transform == targetManager._playerMovement.characterColliderObj){
            targetManager.ApplyKnockback(thisManager.transform.position, attackData.knockback, attackData.knockback * 0.2f, attackData.damage);
            Destroy(gameObject);
        }
    }
}
