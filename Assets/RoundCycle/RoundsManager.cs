using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.UIElements;

public class RoundsManager : MonoBehaviour
{
    [SerializeField] public RoundCycle roundCycle;
    [SerializeField] 
    public int roundDuration;
    public int roundNumber;
    GameObject arrowKeyPlayer = GameObject.FindGameObjectWithTag("ArrowKeysPlayer");
    GameObject WASDPlayer = GameObject.FindGameObjectWithTag("WASDPlayer");
   
    public void RoundStart()
    {
        roundDuration = roundCycle.roundDuration;
        roundNumber = roundCycle.rounds;
        for (int i = roundNumber; i > 0 ; i--)
        {
            
            StartCoroutine(Clock(roundDuration));
        }

    }
    private IEnumerator Clock(int roundtime)
    {
        while (roundtime > 0)
        {
            roundtime--;
            yield return new WaitForSeconds(1);
        }
    }
    public void UpdateUI()
    {
            
    }
}
