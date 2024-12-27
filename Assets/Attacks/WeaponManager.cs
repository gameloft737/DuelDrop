using System;
using System.Collections;
using System.Collections.Generic;
using Cinemachine;
using UnityEngine;
using UnityEngine.InputSystem;

public class WeaponManager : MonoBehaviour
{
    public List<GameObject> damagers;
    [SerializeField] protected GameObject hitParticle;
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
    [SerializeField]protected bool isFrozen = false;
    public int isTutorial = 0;
    [SerializeField] protected WeaponManager targetManager;
    protected CinemachineImpulseSource impulseSource;

    protected Dictionary<AttackData, float> attackCooldowns = new Dictionary<AttackData, float>();
    public void FreezeAll()
    {
        // Set each attack's cooldown to its maximum value to freeze them
        attackCooldowns[regAttack] = regAttack.reloadSpeed;
        attackCooldowns[specialAttack] = specialAttack.reloadSpeed;
        attackCooldowns[ultimateAttack] = ultimateAttack.reloadSpeed;
        Debug.Log("COOLDOwn for " + _playerMovement.gameObject.name+" IS " + attackCooldowns[ultimateAttack]);

        isFrozen =true;
        
        UIManager.instance.AnimateSlider(currentTag, "UltimateAttack", "glow", false);
        UIManager.instance.AnimateSlider(currentTag, "SpecialAttack", "glowRed", false);
        // Update sliders to reflect the cooldowns being at maximum
        UpdateSlider(regAttack);
        UpdateSlider(specialAttack);
        UpdateSlider(ultimateAttack);

        Debug.Log("All attack cooldowns have been frozen.");
    }
    public void UnfreezeAll(){
        isFrozen =false;
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
        if(isTutorial != 0){
            switch(isTutorial){
                case 1:
                    if (attackCooldowns[regAttack] > 0)
                    {
                        attackCooldowns[regAttack] -= Time.deltaTime;
                        UpdateSlider(regAttack);
                    }
                    break;
                case 2:
                    if (attackCooldowns[specialAttack] > 0)
                    {
                        attackCooldowns[specialAttack] -= Time.deltaTime;
                        UpdateSlider(specialAttack);
                    }
                    break; 
                case 3:
                    if (attackCooldowns[ultimateAttack] > 0)
                    {
                        attackCooldowns[ultimateAttack] -= Time.deltaTime;
                        UpdateSlider(ultimateAttack);
                    }
                    break;   
            }
            return;
        }
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
        if (context.performed && (!isFrozen || isTutorial == 1))
        {
            StartCoroutine(TryPerformAttack(regAttack));
            
        }
    }

    public void OnSpecialAttack(InputAction.CallbackContext context)
    {
        if (context.performed && (!isFrozen || isTutorial == 2))
        {
            StartCoroutine(TryPerformSpecialAttack(specialAttack));
            
            UIManager.instance.AnimateSlider(currentTag, "SpecialAttack", "glowRed", false);
        }
    }

    public void OnUltimateAttack(InputAction.CallbackContext context)
    {
        if (context.performed && (!isFrozen || isTutorial == 3))
        {
            StartCoroutine(TryPerformUltimateAttack(ultimateAttack));
            
            UIManager.instance.AnimateSlider(currentTag, "UltimateAttack", "glow", false);
        }
    }

    protected bool isAttacking = false; // Flag to check if an attack is already being performed

    protected virtual IEnumerator TryPerformAttack(AttackData attack)
    {
        if (isAttacking)
        {
            Debug.Log("Already attacking, wait for the attack to finish.");
            yield break;
        }

        if (attackCooldowns[attack] <= 0f)
        {
            isTutorial = 0;
            isAttacking = true; // Mark as attacking
            _playerMovement.animator.SetTrigger("attack");
            AudioManager.instance.Play("MinoAttack");

            yield return new WaitForSeconds(attack.delay);

            PerformAttack(attack);

            // Start cooldown after the attack
            attackCooldowns[attack] = attack.reloadSpeed;
            isAttacking = false; // Mark attack as finished
        }
        else
        {
            AnimateSlider(attack);
            Debug.Log("Attack on cooldown!");
        }

        yield return null; // Wait for the next frame
    }


    protected bool isSpecialAttacking = false; // Flag for special attacks
    protected bool isUltimateAttacking = false; // Flag for ultimate attacks

    protected virtual IEnumerator TryPerformSpecialAttack(AttackData attack)
    {
        if (isSpecialAttacking)
        {
            Debug.Log("Already performing a special attack, wait for it to finish.");
            yield break;
        }

        if (attackCooldowns[attack] <= 0f)
        {
            isTutorial = 0;
            isSpecialAttacking = true; // Mark as performing special attack
            attackCooldowns[attack] = attack.reloadSpeed;

            yield return new WaitForSeconds(attack.delay);

            PerformSpecialAttack(attack);
            isSpecialAttacking = false; // Reset flag after attack
        }
        else
        {
            AnimateSlider(attack);
            Debug.Log("SpecialAttack on cooldown!");
        }

        yield return null;
    }

    protected virtual IEnumerator TryPerformUltimateAttack(AttackData attack)
    {
        if (isUltimateAttacking)
        {
            Debug.Log("Already performing an ultimate attack, wait for it to finish.");
            yield break;
        }

        if (attackCooldowns[attack] <= 0f)
        {
            isTutorial = 0;
            isUltimateAttacking = true; // Mark as performing ultimate attack
            attackCooldowns[attack] = attack.reloadSpeed;

            yield return new WaitForSeconds(attack.delay);

            PerformUltimateAttack(attack);
            isUltimateAttacking = false; // Reset flag after attack
        }
        else
        {
            AnimateSlider(attack);
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

    public void ReduceCooldownsBasedOnKnockback(float knockback)
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

        if (attack == regAttack){
            UIManager.instance.SetSlider(currentTag, "RegularAttack", sliderValue);
        }
        else if (attack == specialAttack) {
            UIManager.instance.SetSlider(currentTag, "SpecialAttack", sliderValue);
            if(sliderValue <= 0.01){
                UIManager.instance.AnimateSlider(currentTag, "SpecialAttack", "glowRed", true);
            }
        }
        else if (attack == ultimateAttack) {
            UIManager.instance.SetSlider(currentTag, "UltimateAttack", sliderValue);
            if(sliderValue <= 0.01){ 
                UIManager.instance.AnimateSlider(currentTag, "UltimateAttack", "glow", true);
            }
        }

        
    }
    protected void AnimateSlider(AttackData attack)
    {
        string sliderName = "";
        if (attack == regAttack) sliderName = "RegularAttack";
        else if (attack == specialAttack) sliderName = "SpecialAttack";
        else if (attack == ultimateAttack) sliderName = "UltimateAttack";

        UIManager.instance.AnimateSlider(currentTag, sliderName, "vibrate");
    }

    public void SetKnockbackModifier(float modifierValue)
    {
        knockbackModifier = modifierValue;
    }

    public virtual void ApplyKnockback(Vector3 attackPosition, float knockbackStrength, float knockupStrength, float damage)
    {

        healthSystem.Damage(damage, 0);

        _playerMovement.Knockback(attackPosition, knockbackStrength);

        MakeHit(attackPosition);

        if (knockupStrength > 0f)
        {
            _playerMovement.Knockup(knockupStrength);
        }
    }
    protected void MakeHit(Vector3 attackPosition)
{
    // Calculate the relative position of the attack
    Vector3 attackDirection = attackPosition - transform.position;

    // Determine base Z-axis rotation (0 for right, 180 for left)
    float zRotation = attackDirection.x >= 0 ? 180f : 0f ;

    // Add a tilt based on the vertical position (Y-coordinate)
    if (attackDirection.y > 0.5f)
    {
        // Tilt slightly upward (max +30)
        zRotation += 30f;
    }
    else if (attackDirection.y < -0.5f)
    {
        // Tilt slightly downward (max -30)
        zRotation -= 30f;
    }

    // Apply the calculated rotation around the Z-axis
    Quaternion particleRotation = Quaternion.Euler(0f, 0f, zRotation);

    // Debug to verify the rotation
    Debug.Log($"Particle Rotation: {particleRotation.eulerAngles}");

    // Instantiate the particle with the calculated rotation
    Instantiate(hitParticle, transform.position, particleRotation, transform);
}


    public void DeleteDamagers(){
        for(int i = 0; i < damagers.Count; i++){
            GameObject damage = damagers[i];
            if(damage != null){
                Destroy(damage);
            }
        }
        
        damagers.Clear();
        RemoveEffects();
    }
    protected virtual void RemoveEffects(){
        return;
    }
}
