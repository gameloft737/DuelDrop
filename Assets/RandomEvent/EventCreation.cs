using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class EventCreation : RandomEventsManager
{
    private RandomPowerUp randomPowerUp;
    private RandomTeleportation randomTeleportation;
    private LandDeletion landDeletion;
    private BombEvent bombEvent;
    private Text EventLabel;
    private float countDownTime = 20f;
    private bool isNextEventTime = false;
    private float countDownTimer;
    public RandomEventsManager[] RandomEvents;
    private RandomEventsManager nextEvent;
    private void Start()
    {
        StartNewCountDown();
    }
    private void Update()
    {
        CountDownTimer();
    }
    private void StartNewCountDown()
    {
        countDownTimer = countDownTime;
        nextEvent = GetRandomEvent();
        if( EventLabel != null)
        {
            EventLabel.text = "Upcoming:" + nextEvent.name;
        }
    }
    private void CountDownTimer()
    {
        if(countDownTimer > 0)
        {
            countDownTimer -= Time.deltaTime;
        }
        else
        {
            //TriggerEvent();
            StartNewCountDown();
        }
    }
    private RandomEvent GetRandomEvent()
    {
        if(RandomEvents.Length == 0)
        {
            return null;
        }
        int randomIndex = Random.Range(0, RandomEvents.Length);
        return RandomEvents[randomIndex];
    }
    private void TriggerEvent()
    {
        if(nextEvent != null && nextEvent.eventPrefab != null)
        {
            nextEvent.Action;
        }
    }
}

