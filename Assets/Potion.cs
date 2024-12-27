using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Potion : MonoBehaviour
{
    public string soundEffect;
    
    public PlayerMovement playerMovement;
    
    public WeaponManager weaponManager;
    public AttackData attackData;
    public GameObject particleEffect;
    public float duration = 3;
    private void OnTriggerEnter(Collider collider){
        
        AudioManager.instance.Play(soundEffect);
        if(collider.gameObject.transform == playerMovement.characterColliderObj){
            playerMovement.SetSpeedBoost(attackData.damage, duration);
            Instantiate(particleEffect,transform.position, Quaternion.identity);
            weaponManager.FreezeAll();
            weaponManager.UnfreezeAll();
            Destroy(gameObject);
        }
    }
}
