using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class EventCreation : MonoBehaviour
{
    public static EventCreation instance;
    public List<RandomEvent> randomEvents; // List of possible random events as ScriptableObjects
    public Text eventNameText; // UI Text to display the next event name
    public Image eventIconImage; // UI Image to display the next event icon
    public Text timerText; // UI Text to display the timer

    public float timeBetweenEvents = 10f; // Time in seconds between events
    private float currentTimer;
    private RandomEvent nextEvent;
    private RandomEvent prevEvent;
    public bool isFrozen = true;
    public List<GameObject> currentEvents;
    public void DestroyEvents()
    {
        // Iterate through all events in the list
        foreach (GameObject currentEvent in currentEvents)
        {
            if (currentEvent != null)
            {
                var eventAction = currentEvent.GetComponent<EventAction>();
                if (eventAction != null)
                {
                    eventAction.StopEvent();
                }

                Destroy(currentEvent);
            }
        }

        currentEvents.Clear();
    }
    private void Awake()
    {
        if(instance == null){
            instance = this;
        }
        else{
            Destroy(gameObject);
            return;
        }   
    }
    void Start()
    {
        currentTimer = timeBetweenEvents;
        SelectRandomEvent();
        UpdateUI();
    }

    void Update()
    {   
        if(!isFrozen){
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
        else{
            currentTimer = timeBetweenEvents;
            UpdateTimerUI();
        }
        
    }

    private void SelectRandomEvent()
    {
        prevEvent = nextEvent;

        if (randomEvents.Count > 0)
        {
            int randomIndex = UnityEngine.Random.Range(0, randomEvents.Count);
            
            // Ensure a different event is selected if more than one event exists
            if (randomEvents.Count > 1 && randomEvents[randomIndex] == prevEvent)
            {
                // Shift the index by 1, wrapping around if necessary
                randomIndex = (randomIndex + 1) % randomEvents.Count;
            }

            nextEvent = randomEvents[randomIndex];
        }
    }

    private void TriggerNextEvent()
    {
        if (nextEvent != null)
        {   
            currentEvents.Add(nextEvent.Trigger());
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
