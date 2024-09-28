using System;
using System.Collections;
using System.Collections.Generic;
using TreeEditor;
using UnityEngine;
using UnityEngine.InputSystem;

public class WeaponManager : MonoBehaviour
{
    [SerializeField] AttackData regAttack;
    [SerializeField] AttackData specialAttack;
    [SerializeField] AttackData ultimateAttack;

    [SerializeField]private PlayerMovement _playerMovement; // Reference to the PlayerMovement script
    [SerializeField]private Transform target; // Reference to the target (opponent)
    

    public void OnAttack(InputAction.CallbackContext context)
    {
        if (context.performed)
        {
            PerformAttack(regAttack);
        }
    }

   protected void PerformAttack(AttackData attack)
    {
        if (target != null)
        {
            // Calculate the direction from the player to the target
            Vector3 directionToTarget = (target.position - transform.position).normalized;

            // Check if the target is within the knockback range
            float knockbackRange = attack.range;
            float distanceToTarget = Vector3.Distance(transform.position, target.position);

            // Calculate the player's facing direction based on the character's local scale
            Vector3 facingDirection = _playerMovement.characterColliderObj.localScale.x > 0 ? transform.right : -transform.right;

            // Only apply knockback if the target is within range and the player is facing the target
            if (distanceToTarget <= knockbackRange && Vector3.Dot(facingDirection, directionToTarget) > 0)
            {
                float knockbackStrength = attack.knockBack;
                PlayerMovement opponentMovement = target.GetComponent<PlayerMovement>();
                if (opponentMovement != null)
                {
                    opponentMovement.Knockback(transform.position, knockbackStrength, knockbackRange);
                }
            }
        }
    }





}
