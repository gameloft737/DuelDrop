using System;
using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class HealthSystem : MonoBehaviour
{
    [Header("Health Settings")]
    [SerializeField] private float maximumHealth = 100f; // Maximum health
    [SerializeField] private float comboTimeWindow = 2f; // Time window for combo hits (seconds)

    [Header("UI and Prefabs")]
    [SerializeField] private GameObject damagePrefab;     // Prefab for damage text
    [SerializeField] private GameObject comboTextPrefab;  // Prefab for combo text

    [Header("References")]
    [SerializeField] private string currentTag; // Tag identifying this GameObject

    public float health; // Current health
    private Coroutine healingCoroutine;
    private Coroutine damagingCoroutine;
    private float remainingHealing;
    private float remainingDamage;

    private int comboCount;
    private float comboMultiplier = 1f;
    private float lastHitTime;

    private void Start()
    {
        // Initialize health and UI
        currentTag = gameObject.tag;
        health = maximumHealth;
        UpdateHealthUI();
    }

    /// <summary>
    /// Applies damage over a duration. Supports combos for consecutive hits.
    /// </summary>
    public void Damage(float damageAmount, float duration)
    {
        UpdateComboState();

        // Apply combo multiplier
        float comboDamage = damageAmount * comboMultiplier;

        // Stop existing damage coroutine if running
        if (damagingCoroutine != null)
        {
            StopCoroutine(damagingCoroutine);
            ApplyDamage(remainingDamage);
        }

        // Start new damage coroutine
        damagingCoroutine = StartCoroutine(SmoothDamage(comboDamage, duration));
    }

    /// <summary>
    /// Heals over a duration. Clamps health to the maximum.
    /// </summary>
    public void Heal(float healAmount, float duration)
    {
        // Stop existing heal coroutine if running
        if (healingCoroutine != null)
        {
            StopCoroutine(healingCoroutine);
            ApplyHealing(remainingHealing);
        }

        // Start new heal coroutine
        healingCoroutine = StartCoroutine(SmoothHeal(healAmount, duration));
    }

    /// <summary>
    /// Updates the combo state based on the time since the last hit.
    /// </summary>
    private void UpdateComboState()
    {
        float timeSinceLastHit = Time.time - lastHitTime;

        if (timeSinceLastHit <= comboTimeWindow)
        {
            comboCount++;
            comboMultiplier = 1f + (comboCount * 0.5f);
        }
        else
        {
            comboCount = 0;
            comboMultiplier = 1f;
        }

        lastHitTime = Time.time;
    }

    /// <summary>
    /// Applies immediate damage.
    /// </summary>
    private void ApplyDamage(float damageAmount)
    {
        health -= damageAmount;
        health = Mathf.Clamp(health, 0, maximumHealth);

        UpdateHealthUI();

        if (health <= 0)
        {
            Die();
        }
    }

    /// <summary>
    /// Applies immediate healing.
    /// </summary>
    private void ApplyHealing(float healAmount)
    {
        health += healAmount;
        health = Mathf.Clamp(health, 0, maximumHealth);

        UpdateHealthUI();
    }

    /// <summary>
    /// Smoothly applies damage over a duration.
    /// </summary>
    private IEnumerator SmoothDamage(float damageAmount, float duration)
    {
        if (duration <= 0)
        {
            ApplyDamage(damageAmount);
            yield break;
        }

        float elapsedTime = 0f;
        float damagePerSecond = damageAmount / duration;

        while (elapsedTime < duration)
        {
            float frameDamage = damagePerSecond * Time.deltaTime;
            ApplyDamage(frameDamage);

            remainingDamage = damageAmount - (damagePerSecond * elapsedTime);
            elapsedTime += Time.deltaTime;

            yield return null;
        }

        remainingDamage = 0f;
    }

    /// <summary>
    /// Smoothly applies healing over a duration.
    /// </summary>
    private IEnumerator SmoothHeal(float healAmount, float duration)
    {
        if (duration <= 0)
        {
            ApplyHealing(healAmount);
            yield break;
        }

        float elapsedTime = 0f;
        float healPerSecond = healAmount / duration;

        while (elapsedTime < duration)
        {
            float frameHeal = healPerSecond * Time.deltaTime;
            ApplyHealing(frameHeal);

            remainingHealing = healAmount - (healPerSecond * elapsedTime);
            elapsedTime += Time.deltaTime;

            yield return null;
        }

        remainingHealing = 0f;
    }

    /// <summary>
    /// Updates the health slider in the UI.
    /// </summary>
    /// 
    public void SetMaxHealth()
    {
        health = maximumHealth; // Set health to maximum
        UpdateHealthUI(); // Update the health bar UI
    }
    private void UpdateHealthUI()
    {
        float healthPercentage = health / maximumHealth;
        UIManager.instance.SetSlider(currentTag, "Health", healthPercentage);
        Debug.Log($"Health: {health}/{maximumHealth}");
    }

    /// <summary>
    /// Creates floating damage text.
    /// </summary>
    private void CreateDamageText(float damageAmount)
    {
        if (damagePrefab != null)
        {
            GameObject dmgText = Instantiate(damagePrefab, transform.position, Quaternion.identity, transform);
            dmgText.GetComponent<TMPro.TextMeshPro>().SetText(damageAmount.ToString("F1"));
        }
    }

    /// <summary>
    /// Creates combo text when applicable.
    /// </summary>
    private void CreateComboText()
    {
        if (comboCount > 1 && comboTextPrefab != null)
        {
            GameObject comboText = Instantiate(comboTextPrefab, transform.position, Quaternion.identity, transform);
            comboText.GetComponent<TMPro.TextMeshPro>().SetText($"Combo x{comboCount}");
        }
    }

    /// <summary>
    /// Handles player death.
    /// </summary>
    private void Die()
    {
        RoundsManager.instance.DeclareDeath(currentTag);
        Debug.Log($"{currentTag} has died.");
    }

    private void Update()
    {
        CheckOutOfBounds();
    }

    /// <summary>
    /// Checks if the player is out of bounds and handles death if true.
    /// </summary>
    private void CheckOutOfBounds()
    {
        if (Mathf.Abs(transform.position.x) > EnvironmentManager.instance.voidMinMax ||
            transform.position.y < EnvironmentManager.instance.voidHeight ||
            transform.position.y > EnvironmentManager.instance.height * 2)
        {
            Die();
        }
    }
}
