using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;

public class WeaponManager : MonoBehaviour
{
    [SerializeField] AttackData regAttack;
    [SerializeField] AttackData specialAttack;
    [SerializeField] AttackData ultimateAttack;


    // References to the sliders in the UI
    [SerializeField] private Slider regAttackSlider;
    [SerializeField] private Slider specialAttackSlider;
    [SerializeField] private Slider ultimateAttackSlider;

    [SerializeField] private PlayerMovement _playerMovement; // Reference to the PlayerMovement script
    [SerializeField] private Transform target; // Reference to the target (opponent)

    private Dictionary<AttackData, float> attackCooldowns = new Dictionary<AttackData, float>();

    private void Start()
    {
        // Initialize cooldowns for each attack
        attackCooldowns[regAttack] = 0f;
        attackCooldowns[specialAttack] = 0f;
        attackCooldowns[ultimateAttack] = 0f;

        regAttackSlider.value = regAttack.reloadSpeed;
        specialAttackSlider.value = specialAttack.reloadSpeed;
        ultimateAttackSlider.value = ultimateAttack.reloadSpeed;
    
    }

    private void Update()
    {
        // Update cooldown timers
        List<AttackData> keys = new List<AttackData>(attackCooldowns.Keys);
        foreach (AttackData attack in keys)
        {
            if (attackCooldowns[attack] > 0)
            {
                attackCooldowns[attack] -= Time.deltaTime;
                UpdateSlider(attack);
            }
        }
    }

    public void OnAttack(InputAction.CallbackContext context)
    {
        if (context.performed)
        {
            TryPerformAttack(regAttack);
        }
    }

    private void TryPerformAttack(AttackData attack)
    {
        if (attackCooldowns[attack] <= 0f)
        {
            PerformAttack(attack);
            attackCooldowns[attack] = attack.reloadSpeed; // Set the cooldown based on reloadSpeed
        }
        else
        {
            Debug.Log("Attack on cooldown!");
        }
    }

    protected void PerformAttack(AttackData attack)
    {
        if (target != null)
        {
            // Instantiate the claw effect at the player's position
            GameObject clawEffect = Instantiate(attack.particle, transform.position, Quaternion.identity, transform);

            // Schedule destruction of the claw effect just before the attack reloads
            StartCoroutine(DestroyClawEffect(clawEffect, attack.reloadSpeed));

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

    private IEnumerator DestroyClawEffect(GameObject clawEffect, float delay)
    {
        // Wait for the delay (time until the next attack can be performed)
        yield return new WaitForSeconds(delay);
        Destroy(clawEffect);
    }




    private void UpdateSlider(AttackData attack)
    {
        if (attack == regAttack)
        {
            regAttackSlider.value = attackCooldowns[regAttack];
        }
        else if (attack == specialAttack)
        {
            specialAttackSlider.value = attackCooldowns[specialAttack];
        }
        else if (attack == ultimateAttack)
        {
            ultimateAttackSlider.value = attackCooldowns[ultimateAttack];
        }
    }
}
