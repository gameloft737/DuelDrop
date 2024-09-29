using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;

public class WeaponManager : MonoBehaviour
{
    [SerializeField] protected AttackData regAttack;
    [SerializeField] protected AttackData specialAttack;
    [SerializeField] protected AttackData ultimateAttack;


    // References to the sliders in the UI
    [SerializeField] protected Slider regAttackSlider;
    [SerializeField] protected Slider specialAttackSlider;
    [SerializeField] protected Slider ultimateAttackSlider;

    public PlayerMovement _playerMovement; // Reference to the PlayerMovement script
    [SerializeField] protected Transform target; // Reference to the target (opponent)
    
    [SerializeField] protected WeaponManager targetManager; // Reference to the target (opponent)

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
        UpdateCooldowns();
    }
    protected virtual void UpdateCooldowns(){
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
    public void OnSpecialAttack(InputAction.CallbackContext context)
    {
        if (context.performed)
        {
            TryPerformSpecialAttack(specialAttack);
        }
    }
    public void OnUltimateAttack(InputAction.CallbackContext context)
    {
        if (context.performed)
        {
            TryPerformUltimateAttack(ultimateAttack);
        }
    }

    protected virtual void TryPerformAttack(AttackData attack)
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

    protected virtual void TryPerformSpecialAttack(AttackData attack)
    {
        if (attackCooldowns[attack] <= 0f)
        {
            PerformSpecialAttack(attack);
            attackCooldowns[attack] = attack.reloadSpeed; // Set the cooldown based on reloadSpeed
        }
        else
        {
            Debug.Log("SpecialAttack on cooldown!");
        }
    }

    protected virtual void TryPerformUltimateAttack(AttackData attack)
    {
        if (attackCooldowns[attack] <= 0f)
        {
            PerformUltimateAttack(attack);
            attackCooldowns[attack] = attack.reloadSpeed; // Set the cooldown based on reloadSpeed
        }
        else
        {
            Debug.Log("UltimateAttack on cooldown!");
        }
    }

    protected virtual void PerformAttack(AttackData attack)
    {
        if (target != null)
        {
            // Instantiate the claw effect at the player's position
            GameObject particleEffect = Instantiate(attack.particle, transform.position, Quaternion.identity, transform);

            // Schedule destruction of the claw effect just before the attack reloads
            StartCoroutine(DestroyParticleEffect(particleEffect, attack.reloadSpeed));

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
                float knockbackStrength = attack.knockback;
                if (targetManager != null)
                {
                    targetManager.ApplyKnockback(transform.position, knockbackStrength,0.1f, this);
                }
            }
        }
    }

    protected virtual void PerformSpecialAttack(AttackData attack){}
    protected virtual void PerformUltimateAttack(AttackData attack){}
    protected IEnumerator DestroyParticleEffect(GameObject particleEffect, float delay)
    {
        // Wait for the delay (time until the next attack can be performed)
        yield return new WaitForSeconds(delay);
        Destroy(particleEffect);
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
    public virtual void ApplyKnockback(Vector3 attackPosition, float knockbackStrength, float knockupStrength, WeaponManager attacker){
        
        _playerMovement.Knockback(attackPosition, knockbackStrength);
        if(knockupStrength > 0f){ 
            _playerMovement.Knockup(knockupStrength);
        }
    }
}
