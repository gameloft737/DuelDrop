using System;
using System.Collections;
using UnityEngine;
using UnityEngine.UI;


[CreateAssetMenu(fileName = "RoundSettings", menuName = "Custom/RoundSettings")]
public class RoundSettings : ScriptableObject
{
    public int rounds;
    public float roundDuration;
}
