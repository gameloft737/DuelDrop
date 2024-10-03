using System.Collections;
using System.Collections.Generic;
using System.Runtime.Serialization;
using UnityEngine;
using UnityEngine.UI;

public class HealthSystem : MonoBehaviour
{
    //[SerializeField] Slider healthBar;
    
    [SerializeField]private float maximum;
    private float health;
    public void Start(){
        health = maximum;
        //healthBar.value = health;
    }
    public void Damage(float knockBack)
    {
        health = health - knockBack;
        //Debug.Log(healthBar.value); 
        //healthBar.value = health/maximum;
    }
}
