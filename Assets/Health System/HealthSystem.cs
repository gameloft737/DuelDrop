using System.Collections;
using System.Collections.Generic;
using System.Runtime.Serialization;
using UnityEngine;
using UnityEngine.UI;

public class HealthSystem : MonoBehaviour
{
    [SerializeField] Slider HealthBar;
    private float Health;
    private float Maximum;

    public void GetMaxHealth(float Maxhealth)
    {
        HealthBar.maxValue = Maxhealth;
        Health = Maxhealth;
    }
    public void Damage(float knockBack)
    {
        Health = Health - knockBack;
        HealthBar.value = Health;
    }
}
