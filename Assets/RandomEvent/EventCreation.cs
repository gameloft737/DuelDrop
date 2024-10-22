using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class EventCreation : MonoBehaviour
{
    public List<RandomEvent> randomEvents; // List of possible random events as ScriptableObjects
    public Text eventNameText; // UI Text to display the next event name
    public Image eventIconImage; // UI Image to display the next event icon
    public Text timerText; // UI Text to display the timer

    public float timeBetweenEvents = 10f; // Time in seconds between events
    private float currentTimer;
    private RandomEvent nextEvent;

    void Start()
    {
        currentTimer = timeBetweenEvents;
        SelectRandomEvent();
        UpdateUI();
    }

    void Update()
    {
        currentTimer -= Time.deltaTime;
        UpdateTimerUI();

        if (currentTimer <= 0)
        {
            TriggerNextEvent();
            currentTimer = timeBetweenEvents; // Reset timer
            SelectRandomEvent(); // Select the next random event
            UpdateUI(); // Update the UI with new event info
        }
    }

    private void SelectRandomEvent()
    {
        if (randomEvents.Count > 0)
        {
            int randomIndex = UnityEngine.Random.Range(0, randomEvents.Count);
            nextEvent = randomEvents[randomIndex];
        }
    }

    private void TriggerNextEvent()
    {
        if (nextEvent != null)
        {
            nextEvent.Trigger();
        }
    }

    private void UpdateUI()
    {
        if (nextEvent != null)
        {
            eventNameText.text = nextEvent.eventName;
            eventIconImage.sprite = nextEvent.eventIcon;
        }
    }

    private void UpdateTimerUI()
    {
        timerText.text = "Time Left: " + Mathf.CeilToInt(currentTimer).ToString() + "s";
    }
}
