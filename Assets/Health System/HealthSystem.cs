using System.Collections;
using System.Collections.Generic;
using System.Runtime.Serialization;
using UnityEngine;
using UnityEngine.UI;

public class HealthSystem : MonoBehaviour
{
    //[SerializeField] Slider healthBar;

    [SerializeField] private float maximum;
    private float health;
    public void Start()
    {
        Debug.Log("At Zero Seconds");
        Debug.Log("At 5 seconds");
        health = maximum;
        //healthBar.value = health;
    }
    public void Damage(float damage)
    {
        health = health - damage;
        //Debug.Log(healthBar.value); 
        //healthBar.value = health/maximum;
    }
    public void Heal(float healing)
    {
        health = health + healing;
    }
}
