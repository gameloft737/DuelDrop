using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;

public class RoundUI : MonoBehaviour
{
    [SerializeField] private RoundSettings roundSettings;
    [SerializeField]public GameObject RoundIcon;
    public Vector3 spawnPoint;
    [SerializeField]public Transform roundIconPosition;
    public Text roundTimer;
    public GameObject[] Icons;
    public Round[] rounds;
    public void Start()
    {
        Icons = new GameObject[roundSettings.rounds];
        spawnPoint = roundIconPosition.transform.position;
        rounds = new Round[roundSettings.rounds];
        for (int i = 0; i < Icons.Length; i++)
        {
            Icons[i] = Instantiate(RoundIcon, roundIconPosition);
            Icons[i].transform.position = spawnPoint;
            spawnPoint.x = spawnPoint.x - 50;
        }
    }
    public void ChangeColors(string Winner)
    {
        if(Winner == "WASD") 
        {
            
        }
        if(Winner == "ArrowKeys")
        {

        }
    }
}
