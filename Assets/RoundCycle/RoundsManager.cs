using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RoundsManager : MonoBehaviour
{
    public void RoundStart()
    {

    }
    public RoundCycle roundCycle;
    private IEnumerator Clock(int roundtime)
    {
        while (roundtime > 0)
        {
            roundtime--;
            yield return new WaitForSeconds(1);
        }
    }
}
