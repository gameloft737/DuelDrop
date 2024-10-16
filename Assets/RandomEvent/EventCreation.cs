using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class EventCreation : MonoBehaviour
{
    [SerializeField] private RandomPowerUp randomPowerUp;
    [SerializeField] private RandomTeleportation randomTeleportation;
    [SerializeField] private LandDeletion landDeletion; 
    [SerializeField] private BombEvent bombEvent;
    [SerializeField] private  Text EventLabel;
    private float countDownTime = 20f;
    private bool isNextEventTime = false;
    private float countDownTimer;
    private RandomEventsManager RandomEventsManager;
    public RandomEventsManager[] RandomEvent;
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
            EventLabel.text = "Upcoming:" + nextEvent.eventName;
        }
        if(eventIconIMage != null && nextEvent.eventIcon != null)
        {
            eventIconImage.sprite  = nextEvent.eventIcon;
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
            TriggerEvent();
            StartNewCountDown();
        }
    }
    private RandomEvent GetRandomEvent()
    {
        if(RandomEvents.Lenght == 0)
        {
            return null;
        }
        int randomIndex = Random.Range(0, randomEvents.Lenght);
        return randomEvents[randomIndex];
    }
    private void TriggerEvent()
    {
        if(nextEvent != null && nextEvent.eventPrefab != null)
        {
            Instantiate(isNextEventTime.eventPrefab,transform.position,Quaternion.identity);
        }
    }
}
