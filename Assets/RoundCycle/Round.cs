using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Round
{
    public RoundState currentState;
    public float duration;
    public String winner;
    public enum RoundState { 
        Load, 
        Play, 
        Finish 
    }
    public bool isActive = false;
}
