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
    [SerializeField] bool generalKnockback = false;
    [SerializeField] bool cameraShake = false;
    public AttackData attackData;
    [SerializeField] string soundEffect = "";
    private void Start(){
        if(generalKnockback){
            targetManager = GameObject.FindGameObjectsWithTag("ArrowKeysManager")[0].GetComponent<WeaponManager>();
            thisManager = GameObject.FindGameObjectsWithTag("WASDManager")[0].GetComponent<WeaponManager>();
        }
    }
    private void OnTriggerEnter(Collider collider){
        
        AudioManager.instance.Play(soundEffect);
        if(generalKnockback){
            if(collider.gameObject.transform == thisManager._playerMovement.characterColliderObj){
                if(baseOnParent){   
                    thisManager.ApplyKnockback(transform.parent.position, attackData.knockback, attackData.knockback * 0.1f, attackData.damage);
                }
                else{
                    thisManager.ApplyKnockback(targetManager.transform.position, attackData.knockback, attackData.knockback, attackData.damage);
                }
                if(cameraShake){
                    CameraShakeManager.instance.CameraShake(GetComponent<CinemachineImpulseSource>());
                }
                Destroy(gameObject);
            }

            if(collider.gameObject.transform == targetManager._playerMovement.characterColliderObj){
                if(baseOnParent){   
                    targetManager.ApplyKnockback(transform.parent.position, attackData.knockback, attackData.knockback * 0.1f, attackData.damage);
                }
                else{
                    targetManager.ApplyKnockback(thisManager.transform.position, attackData.knockback, attackData.knockback, attackData.damage);
                }
                if(cameraShake){
                    CameraShakeManager.instance.CameraShake(GetComponent<CinemachineImpulseSource>());
                }
                Destroy(gameObject);
            }
        }
        else if(collider.gameObject.transform == targetManager._playerMovement.characterColliderObj){
            if(baseOnParent){   
                targetManager.ApplyKnockback(transform.parent.position, attackData.knockback, attackData.knockback * 0.1f, attackData.damage);
            }
            else{
                targetManager.ApplyKnockback(thisManager.transform.position, attackData.knockback, attackData.knockback * 0.1f, attackData.damage);
            }
            if(cameraShake){
                CameraShakeManager.instance.CameraShake(GetComponent<CinemachineImpulseSource>());
            }
            Destroy(gameObject);
        }
    }
}
