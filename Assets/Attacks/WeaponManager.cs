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
        attackCooldowns[specialAttack] = 0f; // Knockback-based fill for special and ultimate attacks starts at 0
        attackCooldowns[ultimateAttack] = 0f;
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
        // Update only the time-based cooldown for the regular attack
        if (attackCooldowns[regAttack] > 0)
        {
            attackCooldowns[regAttack] -= Time.deltaTime;
            UpdateSlider(regAttack);
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
        if (attackCooldowns[attack] >= 1f) // Check if gauge is fully filled (1.0)
        {
            attackCooldowns[attack] = 0f; // Reset the cooldown
            yield return new WaitForSeconds(attack.delay);
            PerformSpecialAttack(attack);
        }
        else
        {
            Debug.Log("SpecialAttack not fully charged!");
        }

        yield return null;
    }

    protected virtual IEnumerator TryPerformUltimateAttack(AttackData attack)
    {
        if (attackCooldowns[attack] >= 1f) // Check if gauge is fully filled (1.0)
        {
            attackCooldowns[attack] = 0f; // Reset the cooldown
            yield return new WaitForSeconds(attack.delay);
            PerformUltimateAttack(attack);
        }
        else
        {
            Debug.Log("UltimateAttack not fully charged!");
        }

        yield return null;
    }

    protected virtual void PerformAttack(AttackData attack)
    {
        if (target != null)
        {
            if (attack is not DoubleParticleAttackData)
            {
                GameObject particleEffect = Instantiate(attack.getParticle(0), transform.position, Quaternion.identity, transform);
                particleEffect.transform.localScale = Vector3.one;
                StartCoroutine(DestroyParticleEffect(particleEffect, attack.reloadSpeed));
            }

            Vector3 directionToTarget = (target.position - transform.position).normalized;
            float knockbackRange = attack.range;
            float distanceToTarget = Vector3.Distance(transform.position, target.position);
            Vector3 facingDirection = _playerMovement.characterColliderObj.localScale.x > 0 ? transform.right : -transform.right;

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
        yield return new WaitForSeconds(delay);
        Destroy(particleEffect);
    }

    private void UpdateSlider(AttackData attack)
    {
        float cooldown = attackCooldowns[attack];
        float maxCooldown = attack.reloadSpeed;
        float sliderValue = Mathf.Clamp01(cooldown / maxCooldown);

        string sliderName = "";
        if (attack == regAttack) sliderName = "RegularAttack";
        else if (attack == specialAttack) sliderName = "SpecialAttack";
        else if (attack == ultimateAttack) sliderName = "UltimateAttack";

        UIManager.instance.SetSlider(currentTag, sliderName, sliderValue);
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

        if (targetManager != null)
        {
            targetManager.FillCooldownFromKnockback(knockbackStrength);
        }
    }

    public void FillCooldownFromKnockback(float knockbackStrength)
    {
        // For special and ultimate attacks, increase cooldown based on knockback
        foreach (var attack in new AttackData[] { specialAttack, ultimateAttack })
        {
            float requiredKnockback = attack.reloadSpeed;
            attackCooldowns[attack] += knockbackStrength / requiredKnockback;
            UpdateSlider(attack);
        }
    }
}
