using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class IconSetter : MonoBehaviour
{
    public Sprite[] sprites;
    [SerializeField] private Image img;
    [SerializeField] private bool isWASD;

    void Start(){
        if(isWASD){
            
            img.sprite = sprites[PlayerPrefs.GetInt("selectedWASD")];
        }   
        else{
           img.sprite = sprites[PlayerPrefs.GetInt("selectedArrowKeys")];
        } 
            
        
    }
}
