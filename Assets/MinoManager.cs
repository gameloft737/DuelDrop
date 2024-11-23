using System.Collections;
using UnityEngine;

public class MinoManager : WeaponManager
{
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
    
}
