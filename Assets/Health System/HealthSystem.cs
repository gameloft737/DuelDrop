using System;
using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class HealthSystem : MonoBehaviour
{
    public bool isFrozen;
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
        currentTag = gameObject.tag;
        health = maximumHealth;
        UpdateHealthUI();
    }

    public void Damage(float damageAmount, float duration)
    {
        if(isFrozen){return;}
        UpdateComboState();

        float comboDamage = damageAmount * comboMultiplier;

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
        if(isFrozen){return;}
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
            comboCount = 0;
            comboMultiplier = 1f;
        }

        lastHitTime = Time.time;
    }

    private void ApplyDamage(float damageAmount)
    {
        if(isFrozen){return;}
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
        if(isFrozen){return;}
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

    private void CreateDamageText(float damageAmount)
    {
        if (damagePrefab != null)
        {
            GameObject dmgText = Instantiate(damagePrefab, transform.position, Quaternion.identity, transform);
            dmgText.GetComponent<TMPro.TextMeshPro>().SetText(damageAmount.ToString("F1"));
        }
    }

    private void CreateComboText()
    {
        if (comboCount > 1 && comboTextPrefab != null)
        {
            GameObject comboText = Instantiate(comboTextPrefab, transform.position, Quaternion.identity, transform);
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
