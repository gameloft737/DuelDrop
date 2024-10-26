using System;
using System.Collections;
using System.Collections.Generic;
using Cinemachine;
using UnityEngine;
using UnityEngine.InputSystem;

public class WeaponManager : MonoBehaviour
{
    [SerializeField] private string currentTag;
    [SerializeField] private string targetTag;

    [Header("Attacks")]
    [SerializeField] protected AttackData regAttack;
    [SerializeField] protected AttackData specialAttack;
    [SerializeField] protected AttackData ultimateAttack;
    public PlayerMovement _playerMovement; // Reference to the PlayerMovement script
    public float knockbackModifier = 1.0f;
    public HealthSystem healthSystem;
    [SerializeField] protected Transform target; // Reference to the target (opponent)
    
    [SerializeField] protected WeaponManager targetManager; // Reference to the target (opponent)
    protected CinemachineImpulseSource impulseSource;

    protected Dictionary<AttackData, float> attackCooldowns = new Dictionary<AttackData, float>();

    private void Start()
    {
        // Initialize cooldowns for each attack
        attackCooldowns[regAttack] = regAttack.reloadSpeed;
        attackCooldowns[specialAttack] = regAttack.reloadSpeed;
        attackCooldowns[ultimateAttack] = regAttack.reloadSpeed;
        impulseSource = GetComponent<CinemachineImpulseSource>();

        target = GameObject.FindGameObjectsWithTag(targetTag + "Player")[0].transform;
        
        targetManager = GameObject.FindGameObjectsWithTag(targetTag + "Manager")[0].GetComponent<WeaponManager>();
    }

    private void Update()
    {
        UpdateCooldowns();
    }

    protected virtual void UpdateCooldowns()
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
            StartCoroutine(TryPerformAttack(regAttack));
        }
    }

    public void OnSpecialAttack(InputAction.CallbackContext context)
    {
        if (context.performed)
        {
            StartCoroutine(TryPerformSpecialAttack(specialAttack));
        }
    }

    public void OnUltimateAttack(InputAction.CallbackContext context)
    {
        if (context.performed)
        {
            StartCoroutine(TryPerformUltimateAttack(ultimateAttack));
        }
    }

    protected virtual IEnumerator TryPerformAttack(AttackData attack)
    {
        if (attackCooldowns[attack] <= 0f)
        {
            attackCooldowns[attack] = attack.reloadSpeed; // Set the cooldown based on reloadSpeed
            yield return new WaitForSeconds(attack.delay);
            PerformAttack(attack);
        }
        else
        {
            Debug.Log("Attack on cooldown!");
        }

        yield return null; // Wait for the next frame
    }

    protected virtual IEnumerator TryPerformSpecialAttack(AttackData attack)
    {
        if (attackCooldowns[attack] <= 0f)
        {
            attackCooldowns[attack] = attack.reloadSpeed; // Set the cooldown based on reloadSpeed
            yield return new WaitForSeconds(attack.delay);
            PerformSpecialAttack(attack);
        }
        else
        {
            Debug.Log("SpecialAttack on cooldown!");
        }

        yield return null; // Wait for the next frame
    }

    protected virtual IEnumerator TryPerformUltimateAttack(AttackData attack)
    {
        if (attackCooldowns[attack] <= 0f)
        {
            attackCooldowns[attack] = attack.reloadSpeed; // Set the cooldown based on reloadSpeed
            yield return new WaitForSeconds(attack.delay);
            PerformUltimateAttack(attack);
        }
        else
        {
            Debug.Log("UltimateAttack on cooldown!");
        }

        yield return null; // Wait for the next frame
    }

    protected virtual void PerformAttack(AttackData attack)
    {
        if (target != null)
        {
            if (_playerMovement.animator != null) {
            _playerMovement.animator.SetTrigger("attack");
            }
            // Instantiate the claw effect at the player's position
            GameObject particleEffect = Instantiate(attack.getParticle(0), transform.position, Quaternion.identity, transform);
            particleEffect.transform.localScale = Vector3.one;
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
                    targetManager.ApplyKnockback(transform.position, knockbackStrength * knockbackModifier, 0.1f, attack.damage);
                }
            }
        }
    }

    protected virtual void PerformSpecialAttack(AttackData attack) { }
    protected virtual void PerformUltimateAttack(AttackData attack) { }
    protected IEnumerator DestroyParticleEffect(GameObject particleEffect, float delay)
    {
        // Wait for the delay (time until the next attack can be performed)
        yield return new WaitForSeconds(delay);
        Destroy(particleEffect);
    }

    private void UpdateSlider(AttackData attack)
    {
        // Add logic for sliders
        return;
    }

    public void SetKnockbackModifier(float modifierValue)
    {
        knockbackModifier = modifierValue;
    }

    public virtual void ApplyKnockback(Vector3 attackPosition, float knockbackStrength, float knockupStrength, float damage)
    {
        healthSystem.Damage(damage, 0);
        _playerMovement.Knockback(attackPosition, knockbackStrength);
        if (knockupStrength > 0f)
        {
            _playerMovement.Knockup(knockupStrength);
        }
    }
}
