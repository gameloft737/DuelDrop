using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class HealthSystem : MonoBehaviour
{
    [SerializeField] private float maximum;
    [SerializeField] private Slider healthBar;
    [SerializeField] GameObject damagePrefab;
    [SerializeField] GameObject comboTextPrefab; // Prefab for combo text
    [SerializeField] private float comboTimeWindow = 2f; // Time window for combo hits in seconds
    private float health;
    private Coroutine Healing;
    private float remainingHealing = 0f;
    private float remainingDamage = 0f;
    private Coroutine Damaging;

    private int comboCount = 0; // Tracks the number of consecutive hits
    private float comboMultiplier = 1f; // Multiplier for combo damage
    private float lastHitTime = 0f; // Tracks the time of the last hit

    public void Start()
    {
        health = maximum;
        healthBar.maxValue = maximum;
    }

    public void Damage(float damage, float duration)
    {
        // Calculate time since last hit for combo logic
        float timeSinceLastHit = Time.time - lastHitTime;

        // If within the combo time window, increase the combo count
        if (timeSinceLastHit <= comboTimeWindow)
        {
            comboCount++;
            comboMultiplier = 1f + (comboCount * 0.5f); // Adjust the multiplier as desired 
        }
        else
        {
            // Reset the combo if time window exceeded
            comboCount = 0;
            comboMultiplier = 1f;
        }
        CreateComboText(comboCount,damage);

        // Apply the combo multiplier to the damage
        float comboDamage = damage * comboMultiplier;

        // Update the last hit time
        lastHitTime = Time.time;

        // If a damaging coroutine is already running, apply the remaining damage instantly
        if (Damaging != null)
        {
            StopCoroutine(Damaging);
            DamageSubtraction(remainingDamage);
        }

        // Start the new damaging coroutine
        Damaging = StartCoroutine(SmoothDamage(comboDamage, duration));
    }

    public void DamageSubtraction(float damage)
    {
        if (health <= 0)
        {
            if (Healing != null) { StopCoroutine(Healing); }
            Damaging = null;
            Debug.Log("You Died");
            return;
        }

        health -= damage;
        healthBar.value = health;
    }

    private IEnumerator SmoothDamage(float damageAmount, float duration)
    {
        if (duration == 0f)
        {
            DamageSubtraction(damageAmount);
            yield break;  // Exit immediately for instant damage
        }

        float elapsedTime = 0f;
        float damagePerSecond = damageAmount / duration;

        while (elapsedTime < duration)
        {
            float frameDamage = damagePerSecond * Time.deltaTime;

            // Apply damage for this frame
            DamageSubtraction(frameDamage);
            remainingDamage = (damageAmount - (damagePerSecond * elapsedTime));

            elapsedTime += Time.deltaTime;
            yield return null;
        }

        remainingDamage = 0f;
    }

    public void HealthGain(float healAmount)
    {
        if (health >= maximum)
        {
            health = maximum;
            if (Healing != null) { StopCoroutine(Healing); }
            Healing = null;
            return;
        }

        health += healAmount;
        healthBar.value = health;
    }

    public void Heal(float healAmount, float duration)
    {
        if (Healing != null)
        {
            StopCoroutine(Healing);
            HealthGain(remainingHealing);
        }

        Healing = StartCoroutine(SmoothHeal(healAmount, duration));
    }

    private IEnumerator SmoothHeal(float healAmount, float duration)
    {
        if (duration == 0f)
        {
            HealthGain(healAmount);
            yield break;
        }

        float elapsedTime = 0f;
        float healPerSecond = healAmount / duration;

        while (elapsedTime < duration)
        {
            float frameHeal = healPerSecond * Time.deltaTime;
            HealthGain(frameHeal);
            remainingHealing = (healAmount - (healPerSecond * elapsedTime));

            elapsedTime += Time.deltaTime;
            yield return null;
        }

        remainingHealing = 0f;
    }

    void CreateDamageText(float damage)
    {
        Debug.Log(damage);
        GameObject dmg = Instantiate(damagePrefab, transform.position, Quaternion.identity, transform);
        dmg.GetComponent<TMPro.TextMeshPro>().SetText(damage.ToString());
        Debug.Log(damage);
    }

    void CreateComboText(int comboCount, float damageAmount)
    {
        // Only show combo text if combo count is greater than 1 (i.e., when a combo is active)
        if (comboCount > 1)
        {
            GameObject comboText = Instantiate(comboTextPrefab, transform.position, Quaternion.identity, transform);
            comboText.GetComponent<TMPro.TextMeshPro>().SetText("Combo x" + comboCount);
        }
        else{
            CreateDamageText(damageAmount);
        }
    }
}
