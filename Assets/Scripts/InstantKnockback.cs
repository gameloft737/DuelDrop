using System;
using System.Collections;
using System.Collections.Generic;
using Cinemachine;
using Unity.VisualScripting;
using UnityEngine;

public class InstantKnockback : MonoBehaviour
{
    public WeaponManager targetManager;
    public WeaponManager thisManager;
    [SerializeField] bool baseOnParent = false;
    [SerializeField] bool cameraShake = false;
    public AttackData attackData;
    private void OnTriggerEnter(Collider collider){
        if(collider.gameObject.transform == targetManager._playerMovement.characterColliderObj){
            if(baseOnParent){   
                targetManager.ApplyKnockback(transform.parent.position, attackData.knockback, attackData.knockback * 0.2f, attackData.damage);
            }
            else{
                targetManager.ApplyKnockback(thisManager.transform.position, attackData.knockback, attackData.knockback * 0.2f, attackData.damage);
            }
            if(cameraShake){
                CameraShakeManager.instance.CameraShake(GetComponent<CinemachineImpulseSource>());
            }
            Destroy(gameObject);
        }
    }
}
