using System.Collections;
using System.Collections.Generic;
using TreeEditor;
using UnityEngine;
using UnityEngine.InputSystem;

public class WeaponManager : MonoBehaviour
{
    [SerializeField] AttackData attack;
    [SerializeField] AttackData specialAttack;
    [SerializeField] AttackData ultimateAttack;

    private PlayerMovement _playerMovement; // Reference to the PlayerMovement script
    [SerializeField]private Transform target; // Reference to the target (opponent)

    private void Awake()
    {
        _playerMovement = GetComponent<PlayerMovement>(); // Get the PlayerMovement component
        // Assume target is assigned somehow (could be a public field or set in another method)
    }

    public void OnAttack(InputAction.CallbackContext context)
    {
        if (context.performed)
        {
            PerformAttack();
        }
    }

    protected void PerformAttack()
    {
        // Assuming you have a reference to the target PlayerMovement component
        if (target != null)
        {
            float knockbackStrength = attack.knockBack; // Adjust this value as needed
            float knockbackRange = attack.range;
            // Apply knockback to the opponent
            PlayerMovement opponentMovement = target.GetComponent<PlayerMovement>();
            if (opponentMovement != null)
            {
                opponentMovement.Knockback(transform.position, knockbackStrength, knockbackRange);
            }
        }
    }


}
