using System.Collections;
using TMPro;
using UnityEngine;

public class CountdownText : MonoBehaviour
{
    public TMP_Text countdownText; // Reference to the TextMeshPro component

    private void OnEnable()
    {
        if (countdownText != null)
        {
            StartCoroutine(CountdownCoroutine());
        }
    }

    private IEnumerator CountdownCoroutine()
    {
        int countdownValue = 3; // Start countdown value
        while (countdownValue > 0)
        {
            countdownText.text = countdownValue.ToString();
            yield return new WaitForSeconds(1f);
            countdownValue--;
        }

        countdownText.text = "GO!"; // Display "GO!" after the countdown
        yield return new WaitForSeconds(1f);

        countdownText.text = ""; // Clear text if needed
    }
}