using System.Collections;
using System.Collections.Generic;
using System.Runtime.Serialization;
using Unity.PlasticSCM.Editor.WebApi;
using UnityEditor;
using UnityEngine;
using UnityEngine.UI;

public class HealthSystem : MonoBehaviour
{
    //[SerializeField] Slider healthBar;

    [SerializeField] private float maximum;
    private float health;
    private Coroutine Healing;
    private float remainingHealing = 0f;
    private float remainingDamage = 0f;
    private Coroutine Damaging;
    public void Start()
    {
        health = maximum;
        //healthBar.value = health;
    }
    public void Damage(float damage,float duration)
    {
        // If a Damaging coroutine is already running, apply the remaining health instantly
        if (Damaging != null)
        {
            StopCoroutine(Damaging);
            DamageSubstraction(remainingDamage);  // Apply the remaining healing instantly
        }
        // Start the new healing coroutine
        Damaging = StartCoroutine(SmoothDamage(damage, duration));
    }
    public void DamageSubstraction(float damage)
    {
        if (health <= 0)
        {
            StopCoroutine(Damaging); Damaging = null;
            Debug.Log("You Died");
            return;
        }
        health = health - damage;
    }
    public void HealthGain(float healAmount)
    {
        if (health >= maximum)
        {
            health = maximum;
            StopCoroutine(Healing); Healing = null;
            return;
        }
        health = health + healAmount;
    }
    public void Heal(float healAmount, float duration)
    {
        // If a healing coroutine is already running, apply the remaining health instantly
        if (Healing != null)
        {
            StopCoroutine(Healing);
            HealthGain(remainingHealing);  // Apply the remaining healing instantly
        }
        // Start the new healing coroutine
        Healing = StartCoroutine(SmoothHeal(healAmount, duration));
    }
    private IEnumerator SmoothDamage(float damageamount, float duration)
    {
        if (duration == 0f)
        {
            DamageSubstraction(damageamount);
            yield break;  // Exit immediately for instant damage
        }

        float elapsedTime = 0f;
        float damagePerSecond = damageamount / duration;

        while (elapsedTime < duration)
        {
            float frameDamage = damagePerSecond * Time.deltaTime;

            // Apply damage for this frame
            DamageSubstraction(frameDamage);
            // Update the remaining damage in case the coroutine gets interrupted
            remainingDamage = (damageamount - (damagePerSecond * elapsedTime));

            elapsedTime += Time.deltaTime;
            yield return null;  // Wait for the next frame
        }

        // Reset remaining damage after coroutine finishes
        remainingDamage = 0f;
    }
    private IEnumerator SmoothHeal(float healAmount, float duration)
    {
        if (duration == 0f)
        {
            HealthGain(healAmount);
            yield break;  // Exit immediately for instant heal
        }

        float elapsedTime = 0f;
        float healPerSecond = healAmount / duration;

        while (elapsedTime < duration)
        {
            float frameHeal = healPerSecond * Time.deltaTime;

            // Apply heal for this frame
            HealthGain(frameHeal);

            // Update the remaining healing in case the coroutine gets interrupted
            remainingHealing = (healAmount - (healPerSecond * elapsedTime));

            elapsedTime += Time.deltaTime;
            yield return null;  // Wait for the next frame
        }

        // Reset remaining health after coroutine finishes
        remainingHealing = 0f;
    }
}