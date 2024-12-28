using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SettingsManager : MonoBehaviour
{
    public Animator settingsAnim;
    public void ShowSettings(){
        settingsAnim.SetBool("isOpen", true);
    }
    public void HideSettings(){
        
        settingsAnim.SetBool("isOpen", false);
    }
}
