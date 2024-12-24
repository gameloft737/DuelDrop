using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ButtonAnimation : MonoBehaviour
{
    [SerializeField]private Animator animator;
    [SerializeField]private string boolName;
    public void OnMouseEnter(){
        animator.SetBool(boolName, true);
    }
    public void OnMouseExit(){
        animator.SetBool(boolName, false);
    }
}
