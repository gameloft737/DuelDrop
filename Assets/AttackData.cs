using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "AttackData", menuName = "Attacks/AttackData")]
public class AttackData : ScriptableObject
{
    [SerializeField]protected GameObject particle;
    public float reloadSpeed = 1f;
    public float knockback = 1f;    
    public float delay;
    public float damage = 1f;  
    public float range = 1f;
    public virtual GameObject getParticle(int index){
        return particle;
    }
}
