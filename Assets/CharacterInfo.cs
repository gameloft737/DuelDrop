using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CharacterInfo : MonoBehaviour
{
    public GameObject info;
    private bool isActive =false;
    public void ToggleActivity()
    {
        info.SetActive(!isActive);
        isActive = !isActive;
    }
    public void SetFalse(){
        isActive = false;   
        info.SetActive(false);
    }
}
