using System;
using System.Collections;
using UnityEngine;
using UnityEngine.UIElements;

public class MinoManager : WeaponManager
{
    public float rampageDuration = 7f;
    public bool isRampage = false;
    public float hornMoveSpeed = 5f;
    [SerializeField] private GameObject hornPrefab;
    protected override IEnumerator TryPerformAttack(AttackData attack){
        if (isAttacking)
        {
            Debug.Log("Already attacking, wait for the attack to finish.");
            yield break;
        }
        if (attackCooldowns[attack] <= 0f)
        {    
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
            Debug.Log("Attack on cooldown!");
        }

        yield return null; // Wait for the next frame
    }    
    protected override void PerformAttack(AttackData attack)
    {
        if (target != null)
        {
            if(isRampage){
                GameObject particleEffect = Instantiate(attack.getParticle(2), transform.position, Quaternion.identity, transform);
                particleEffect.transform.localScale = Vector3.one;
                StartCoroutine(DestroyParticleEffect(particleEffect, attack.reloadSpeed * 2));
            }
            else{
                GameObject particleEffect = Instantiate(attack.getParticle(1), transform.position, Quaternion.identity, transform);
                particleEffect.transform.localScale = Vector3.one;
                StartCoroutine(DestroyParticleEffect(particleEffect, attack.reloadSpeed * 2));
            }
            

            Vector3 directionToTarget = (target.position - transform.position).normalized;
            float knockbackRange = attack.range;
            float distanceToTarget = Vector3.Distance(transform.position, target.position);
            Vector3 facingDirection = _playerMovement.characterColliderObj.localScale.x > 0 ? transform.right : -transform.right;

            if ((distanceToTarget <= knockbackRange && Vector3.Dot(facingDirection, directionToTarget) > 0) || distanceToTarget <= knockbackRange * 0.15f)
            {
                if (targetManager != null)
                {
                    if(isRampage){ targetManager.ApplyKnockback(transform.position, attack.knockback * knockbackModifier, 0.1f, attack.damage * 2); }
                    else{ targetManager.ApplyKnockback(transform.position, attack.knockback * knockbackModifier, 0.1f, attack.damage); }
                    ReduceCooldownsBasedOnKnockback(attack.knockback);
                }
            }
            if(isRampage){
                GameObject projectile = Instantiate(hornPrefab, transform.position,Quaternion.identity);
                MoveVelocity velocity = projectile.GetComponentInChildren<MoveVelocity>();
                velocity.velocity = new Vector3(_playerMovement.isRight ? hornMoveSpeed : -hornMoveSpeed, 0, 0);
                velocity.StartMovement();
                InstantKnockback knockbacker = projectile.GetComponentInChildren<InstantKnockback>();
                knockbacker.targetManager = targetManager;
                knockbacker.thisManager = this;  
                StartCoroutine(DestroyParticleEffect(projectile, 3f));
            }
        }
    }
    protected override void PerformSpecialAttack(AttackData attack)
    {
        
        AudioManager.instance.Play("MinoSpecialAttack");
        GameObject particleS = Instantiate(attack.getParticle(1), transform.position, transform.rotation, transform);
        StartCoroutine(SpecialAfterTime(0.35f));
        StartCoroutine(DestroyParticleEffect(particleS, 1f));
        StartCoroutine(SpawnProjectile(attack, 1f));

    }
    private IEnumerator SpecialAfterTime(float delay){
        yield return new WaitForSeconds(delay);
        _playerMovement.animator.SetTrigger("special");
    }
    private IEnumerator SpawnProjectile(AttackData attack, float delay){
        yield return new WaitForSeconds(delay);
        GameObject shot = Instantiate(attack.getParticle(0), transform.position, transform.rotation);
        Vector3 directionToTarget = (target.position - transform.position).normalized;
        shot.transform.forward = directionToTarget;
        MoveVelocity moveVelocity = shot.GetComponentInChildren<MoveVelocity>();
        if (moveVelocity != null)
        {
            moveVelocity.velocity = directionToTarget * attack.range;
            moveVelocity.StartMovement();
        }
        else{Debug.Log("NULL");}

        InstantKnockback knockback = shot.GetComponentInChildren<InstantKnockback>();
        if (knockback != null)
        {
            knockback.targetManager = targetManager;
            knockback.attackData = attack;
        }
        else{Debug.Log("NULL Knockback");}
        Destroy(shot, 10f );
    }
    protected override void PerformUltimateAttack(AttackData attack)
    {
        
        _playerMovement.animator.SetTrigger("ultimate");
        AudioManager.instance.Play("MinoUltimateAttack");
        isRampage = true;
        _playerMovement.moveSpeed = _playerMovement.moveSpeed * 2;
        _playerMovement.acceleration = _playerMovement.acceleration * 2;
        
        StartCoroutine(RampageTime(attack));
    }
    private IEnumerator RampageTime(AttackData attack)
    {
        GameObject particleEffect = Instantiate(attack.getParticle(0), transform.position, Quaternion.identity, transform);
        particleEffect.transform.localScale = Vector3.one;
        yield return new WaitForSeconds(rampageDuration);
        Destroy(particleEffect);
        isRampage = false;
        _playerMovement.moveSpeed = _playerMovement.moveSpeed / 2;
        _playerMovement.acceleration = _playerMovement.acceleration / 2;
    }
}

