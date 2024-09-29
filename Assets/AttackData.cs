using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "AttackData", menuName = "Attacks/AttackData")]
public class AttackData : ScriptableObject
{
    public GameObject particle;
    public float reloadSpeed = 1f;
    public float knockBack = 1f;    
    public float range = 1f;
}
