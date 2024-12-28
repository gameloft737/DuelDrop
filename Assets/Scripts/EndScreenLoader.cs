using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class EndScreenLoader : MonoBehaviour
{
    public GameObject[] characters;
    public string[] characterNames;
    public Text winnerText;
    void Start()
    {
        GameObject character = characters[PlayerPrefs.GetInt("winner")];
        Instantiate(character, transform.position, Quaternion.identity, transform);
        winnerText.text = characterNames[PlayerPrefs.GetInt("winner")] + " WINS!";
        winnerText.color = ( PlayerPrefs.GetInt("winnerColor") == 0) ? Color.red : Color.blue;
    }
}
