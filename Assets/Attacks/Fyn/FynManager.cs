using System.Collections;
using UnityEngine;

public class FynManager : WeaponManager
{
    [SerializeField]private bool isShieldActive = false; // Tracks if the shield is active
    [SerializeField]private float shieldDuration = 3f; // Example shield duration (you can adjust this)
    private float knockbackReduction = 0.5f; // 50% reduction in knockback when the shield is active
    [SerializeField]private float moveSpeed = 50;
    [SerializeField] private float buffStrength = 3f;
    [SerializeField]private float buffTime = 3f;
    private Coroutine shieldCoroutine;
    int randomNum = 1;
    protected override IEnumerator TryPerformAttack(AttackData attack){
        if (isAttacking)
        {
            Debug.Log("Already attacking, wait for the attack to finish.");
            yield break;
        }
        if (attackCooldowns[attack] <= 0f)
        { 
            
            isTutorial = 0;
            isAttacking = true; // Mark as attacking
            randomNum = UnityEngine.Random.Range(1, 3);
            GameObject particleEffect = Instantiate(attack.getParticle(randomNum), transform.position, Quaternion.identity, transform);
            particleEffect.transform.localScale = Vector3.one;
            // Schedule destruction of the claw effect just before the attack reloads
            StartCoroutine(DestroyParticleEffect(particleEffect, attack.reloadSpeed *2f));

            _playerMovement.animator.SetTrigger("attack" + randomNum);
            AudioManager.instance.Play("FynAttack");
            
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
    protected override void PerformAttack(AttackData attack)
    {
        if (target != null)
        {
            // Instantiate the claw effect at the player's position
           
            // Calculate the direction from the player to the target
            Vector3 directionToTarget = (target.position - transform.position).normalized;

            // Check if the target is within the knockback range
            float knockbackRange = attack.range;
            float distanceToTarget = Vector3.Distance(transform.position, target.position);

            // Calculate the player's facing direction based on the character's local scale
            Vector3 facingDirection = _playerMovement.characterColliderObj.localScale.x > 0 ? transform.right : -transform.right;

            // Only apply knockback if the target is within range and the player is facing the target
            if((distanceToTarget <= knockbackRange && Vector3.Dot(facingDirection, directionToTarget) > 0) || distanceToTarget <= knockbackRange * 0.25f)
            {
                float knockbackStrength = attack.knockback;
                if (targetManager != null)
                {
                    targetManager.ApplyKnockback(transform.position, knockbackStrength * knockbackModifier,0.1f, attack.damage);
                    ReduceCooldownsBasedOnKnockback(attack.knockback);
                }
            
            }
        }
    }
    // Override the special attack method to activate the shield
    private GameObject shieldParticleEffect;
    protected override void PerformSpecialAttack(AttackData attack)
    {
        if (shieldCoroutine == null)
        {
            AudioManager.instance.Play("FynSpecialAttack");
            isShieldActive = true;
            shieldParticleEffect = Instantiate(attack.getParticle(0), transform.position, Quaternion.identity, transform);

            // Schedule destruction of the claw effect just before the attack reloads
            StartCoroutine(DestroyParticleEffect(shieldParticleEffect, shieldDuration));
            // Start the shield duration countdown
            shieldCoroutine = StartCoroutine(DeactivateShieldAfterDuration(shieldDuration));

            // Call base PerformSpecialAttack() if needed for additional effects, or omit this line if special attack is only shield

        }
    }

    // Coroutine to deactivate the shield after a set duration
    private IEnumerator DeactivateShieldAfterDuration(float duration)
    {
        yield return new WaitForSeconds(duration);
        isShieldActive = false;
        shieldCoroutine = null;
        Debug.Log("Shield deactivated!");
    }

    // Method to apply knockback when the player is hit
    public override void ApplyKnockback(Vector3 attackPosition, float knockbackStrength, float knockupStrength, float damage)
    {
        if (isShieldActive)
        {
            if(knockbackStrength < 1f){
                Debug.Log("HUH");
                return;
            }
            // Reduce the knockback received by 50% and reflect the rest
            float reducedKnockback = knockbackStrength * knockbackReduction;
            healthSystem.Damage((damage * knockbackReduction), 0);
            _playerMovement.Knockback(attackPosition, reducedKnockback);
            if(knockupStrength > 0f){ 
                _playerMovement.Knockup(knockupStrength  * knockbackReduction);
            }
            // Reflect the remaining knockback to the attacker
            if (targetManager != null)
            {
                float reflectedKnockback = knockbackStrength * (1 - knockbackReduction);
                targetManager.healthSystem.Damage(damage * (1 - knockbackReduction),0);
                targetManager._playerMovement.Knockback(transform.position, reflectedKnockback * knockbackModifier);
            }
        }
        else
        {
            // No shield active, apply full knockback to the player
            
            healthSystem.Damage(damage,0);
            _playerMovement.Knockback(attackPosition, knockbackStrength);
            if(knockupStrength > 0f){ 
                _playerMovement.Knockup(knockupStrength);
            }
        }
        
        MakeHit(attackPosition);
    }

    protected override void PerformUltimateAttack(AttackData attack)
    {
        if (target != null)
        {
            
            AudioManager.instance.Play("FynUltimateAttack");
            _playerMovement.animator.SetBool("isUltimate", true);
            Debug.Log("Ultimate Attack Performed");
            // Temporarily disable collisions between this object and other colliders
            Collider thisCollider = _playerMovement.characterColliderObj.GetComponent<Collider>();
            _playerMovement.canMove = false;
            if (_playerMovement.rb != null)
            {
                 _playerMovement.rb.velocity = Vector3.zero;
                 _playerMovement.rb.angularVelocity = Vector3.zero;
                 _playerMovement.rb.useGravity = false; // Optional: disable gravity during the attack
            }
            thisCollider.enabled = false;

            // Move towards the target
            StartCoroutine(MoveTowardsTarget(target.position, attack));
        }
    }
    private IEnumerator KnockBackBuff(float duration, AttackData attack)
    {
        GameObject particleEffect = Instantiate(attack.getParticle(0), transform.position, Quaternion.identity, transform);
        particleEffect.transform.localScale = Vector3.one;
        SetKnockbackModifier(buffStrength);
        Debug.Log(buffStrength);
        yield return new WaitForSeconds(duration);
        Destroy(particleEffect);
        SetKnockbackModifier(1f);
    }
    private IEnumerator MoveTowardsTarget(Vector3 targetPosition, AttackData attack)
    {
        Vector3 directionToTarget = targetPosition - _playerMovement.gameObject.transform.position;

        // Flip the character's scale based on the direction of movement (left or right)
        if (directionToTarget.x > 0)
        {
            _playerMovement.characterColliderObj.transform.localScale = new Vector3(1, 1, 1); // Facing right
        }
        else if (directionToTarget.x < 0)
        {
            _playerMovement.characterColliderObj.transform.localScale = new Vector3(-1, 1, 1); // Facing left
        }

        while (Vector3.Distance(transform.position, targetPosition) > 1f) // Keep moving until close to target
        {
             _playerMovement.rb.MovePosition(Vector3.MoveTowards(
            _playerMovement.rb.position, 
            targetPosition, 
            moveSpeed * Time.fixedDeltaTime
            ));

            yield return new WaitForFixedUpdate(); // Match FixedUpdate frequency
            
        }

        // Re-enable collisions after reaching the target
        Collider thisCollider = _playerMovement.characterColliderObj.GetComponent<Collider>();
        thisCollider.enabled = true;
        
        _playerMovement.rb.useGravity = true;
        targetManager.ApplyKnockback(transform.position, attack.knockback * knockbackModifier,attack.knockback * 0.4f, attack.damage);
        CameraShakeManager.instance.CameraShake(impulseSource);
        _playerMovement.canMove = true;
        _playerMovement.animator.SetBool("isUltimate", false);
        StartCoroutine(KnockBackBuff(buffTime, attack));
    }
    protected override void RemoveEffects(){
        if(!isShieldActive){return;}
        Debug.Log("SHIELD OFF");
        Destroy(shieldParticleEffect);
        isShieldActive = false;
        shieldCoroutine = null;
    }

}
