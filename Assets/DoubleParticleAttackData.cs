using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "AttackData", menuName = "Attacks/DoubleParticleAttackData")]
public class DoubleParticleAttackData : AttackData
{
    [SerializeField]protected GameObject particle2;
    public override GameObject getParticle(int index){
        if(index == 1){
            return particle;
        }
        else{
            return particle2;
        }
    }
}
