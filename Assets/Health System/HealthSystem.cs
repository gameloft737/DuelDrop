using System;
using System.Collections;
using UnityEngine;

public class HealthSystem : MonoBehaviour
{
    public bool isFrozen;

    [Header("Health Settings")]
    [SerializeField] private float maximumHealth = 100f;
    [SerializeField] private float comboTimeWindow = 2f;

    [Header("UI and Prefabs")]
    [SerializeField] private GameObject damagePrefab;
    [SerializeField] private GameObject comboTextPrefab;

    [Header("References")]
    [SerializeField] private string currentTag;

    public float health;
    private Coroutine healingCoroutine;
    private Coroutine damagingCoroutine;
    private float remainingHealing;
    private float remainingDamage;

    private int comboCount;
    private float comboMultiplier = 1f;
    private float lastHitTime;

    private void Start()
    {
        currentTag = gameObject.tag;
        health = maximumHealth;
        UpdateHealthUI();
    }

    public void Damage(float damageAmount, float duration)
    {
        if (isFrozen) return;

        // Update combo state
        UpdateComboState();

        if (comboCount > 1)
        {
            CreateComboText();
        }

        // Apply combo multiplier to damage
        float comboDamage = damageAmount * comboMultiplier;

        // Create damage text for the initial damage
        CreateDamageText(comboDamage);

        // Handle ongoing damage application
        if (damagingCoroutine != null)
        {
            StopCoroutine(damagingCoroutine);
            damagingCoroutine = null;
            ApplyDamage(remainingDamage);
        }

        damagingCoroutine = StartCoroutine(SmoothDamage(comboDamage, duration));
    }

    public void Heal(float healAmount, float duration)
    {
        if (isFrozen) return;

        if (healingCoroutine != null)
        {
            StopCoroutine(healingCoroutine);
            healingCoroutine = null;
            ApplyHealing(remainingHealing);
        }

        healingCoroutine = StartCoroutine(SmoothHeal(healAmount, duration));
    }

    public void SetMaxHealth()
    {
        if (healingCoroutine != null)
        {
            StopCoroutine(healingCoroutine);
            healingCoroutine = null;
        }

        if (damagingCoroutine != null)
        {
            StopCoroutine(damagingCoroutine);
            damagingCoroutine = null;
        }

        remainingDamage = 0f;
        remainingHealing = 0f;
        health = maximumHealth;
        UpdateHealthUI();
    }

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
            comboCount = 1; // Reset to 1 instead of 0
            comboMultiplier = 1f;
        }

        lastHitTime = Time.time;
    }

    private void ApplyDamage(float damageAmount)
    {
        if (isFrozen) return;

        health -= damageAmount;
        health = Mathf.Clamp(health, 0, maximumHealth);

        UpdateHealthUI();

        if (health <= 0)
        {
            Die();
        }
    }

    private void ApplyHealing(float healAmount)
    {
        if (isFrozen) return;

        health += healAmount;
        health = Mathf.Clamp(health, 0, maximumHealth);

        UpdateHealthUI();
    }

    private IEnumerator SmoothDamage(float damageAmount, float duration)
    {
        if (duration <= 0)
        {
            ApplyDamage(damageAmount);
            damagingCoroutine = null;
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
        damagingCoroutine = null;
    }

    private IEnumerator SmoothHeal(float healAmount, float duration)
    {
        if (duration <= 0)
        {
            ApplyHealing(healAmount);
            healingCoroutine = null;
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
        healingCoroutine = null;
    }

    private void UpdateHealthUI()
    {
        float healthPercentage = health / maximumHealth;
        UIManager.instance.SetSlider(currentTag, "Health", healthPercentage);
        Debug.Log($"Health: {health}/{maximumHealth}");
    }
    public float height = 1f;
    private void CreateDamageText(float damageAmount)
    {
        if (damagePrefab != null)
        {
            GameObject dmgText = Instantiate(damagePrefab, transform.position + Vector3.up * height, Quaternion.identity, transform);
            dmgText.GetComponent<TMPro.TextMeshPro>().SetText((damageAmount * 5).ToString("F0")); // Display as an integer
        }
    }


    private void CreateComboText()
    {
        if (comboTextPrefab != null)
        {
            GameObject comboText = Instantiate(comboTextPrefab, transform.position + Vector3.up * 1.5f * height, Quaternion.identity, transform);
            comboText.GetComponent<TMPro.TextMeshPro>().SetText($"Combo x{comboCount}");
        }
    }

    private void Die()
    {
        RoundsManager.instance.DeclareDeath(currentTag);
        Debug.Log($"{currentTag} has died.");
    }

    private void Update()
    {
        CheckOutOfBounds();
    }

    private void CheckOutOfBounds()
    {
        if (Mathf.Abs(transform.position.x) > EnvironmentManager.instance.voidMinMax ||
            transform.position.y < EnvironmentManager.instance.voidHeight ||
            transform.position.y > EnvironmentManager.instance.height * 3)
        {
            Die();
        }
    }
}
