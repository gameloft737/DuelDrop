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
    public PlayerMovement _playerMovement;
    public float knockbackModifier = 1.0f;
    public HealthSystem healthSystem;
    [SerializeField] protected Transform target;
    bool isFrozen = false;
    
    [SerializeField] protected WeaponManager targetManager;
    protected CinemachineImpulseSource impulseSource;

    protected Dictionary<AttackData, float> attackCooldowns = new Dictionary<AttackData, float>();
    public void FreezeAll()
    {
        // Set each attack's cooldown to its maximum value to freeze them
        attackCooldowns[regAttack] = regAttack.reloadSpeed;
        attackCooldowns[specialAttack] = specialAttack.reloadSpeed;
        attackCooldowns[ultimateAttack] = ultimateAttack.reloadSpeed;
        isFrozen =true;
        // Update sliders to reflect the cooldowns being at maximum
        UpdateSlider(regAttack);
        UpdateSlider(specialAttack);
        UpdateSlider(ultimateAttack);

        Debug.Log("All attack cooldowns have been frozen.");
}
    private void Start()
    {
        // Initialize cooldowns for each attack
        attackCooldowns[regAttack] = regAttack.reloadSpeed;
        attackCooldowns[specialAttack] = specialAttack.reloadSpeed;
        attackCooldowns[ultimateAttack] = ultimateAttack.reloadSpeed;
        impulseSource = GetComponent<CinemachineImpulseSource>();

        target = GameObject.FindGameObjectsWithTag(targetTag + "Player")[0].transform;
        
        targetManager = GameObject.FindGameObjectsWithTag(targetTag + "Manager")[0].GetComponent<WeaponManager>();
    }

    private void Update()
    {
        if(!isFrozen){  
            UpdateCooldowns();
        }
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
        if (context.performed && !isFrozen)
        {
            StartCoroutine(TryPerformAttack(regAttack));
        }
    }

    public void OnSpecialAttack(InputAction.CallbackContext context)
    {
        if (context.performed && !isFrozen)
        {
            StartCoroutine(TryPerformSpecialAttack(specialAttack));
        }
    }

    public void OnUltimateAttack(InputAction.CallbackContext context)
    {
        if (context.performed && !isFrozen)
        {
            StartCoroutine(TryPerformUltimateAttack(ultimateAttack));
        }
    }

    protected virtual IEnumerator TryPerformAttack(AttackData attack)
    {
        if (attackCooldowns[attack] <= 0f)
        {
            attackCooldowns[attack] = attack.reloadSpeed;
            yield return new WaitForSeconds(attack.delay);
            PerformAttack(attack);
        }
        else
        {
            Debug.Log("Attack on cooldown!");
        }

        yield return null;
    }

    protected virtual IEnumerator TryPerformSpecialAttack(AttackData attack)
    {
        if (attackCooldowns[attack] <= 0f)
        {
            attackCooldowns[attack] = attack.reloadSpeed;
            yield return new WaitForSeconds(attack.delay);
            PerformSpecialAttack(attack);
        }
        else
        {
            Debug.Log("SpecialAttack on cooldown!");
        }

        yield return null;
    }

    protected virtual IEnumerator TryPerformUltimateAttack(AttackData attack)
    {
        if (attackCooldowns[attack] <= 0f)
        {
            attackCooldowns[attack] = attack.reloadSpeed;
            yield return new WaitForSeconds(attack.delay);
            PerformUltimateAttack(attack);
        }
        else
        {
            Debug.Log("UltimateAttack on cooldown!");
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

            if ((distanceToTarget <= knockbackRange && Vector3.Dot(facingDirection, directionToTarget) > 0) || distanceToTarget <= knockbackRange * 0.15f)
            {
                if (targetManager != null)
                {
                    targetManager.ApplyKnockback(transform.position, attack.knockback * knockbackModifier, 0.1f, attack.damage);
                    ReduceCooldownsBasedOnKnockback(attack.knockback);
                }
            }
        }
    }

    protected void ReduceCooldownsBasedOnKnockback(float knockback)
    {
        // Calculate cooldown reduction for special and ultimate attacks
        float specialReduction = knockback * specialAttack.reloadSpeed * 0.005f;
        float ultimateReduction = knockback * ultimateAttack.reloadSpeed * 0.005f;

        // Apply reduction while ensuring the cooldown doesn't go below zero
        attackCooldowns[specialAttack] = Mathf.Max(0, attackCooldowns[specialAttack] - specialReduction);
        attackCooldowns[ultimateAttack] = Mathf.Max(0, attackCooldowns[ultimateAttack] - ultimateReduction);
        UpdateSlider(specialAttack);
        UpdateSlider(ultimateAttack);
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
    }
}
