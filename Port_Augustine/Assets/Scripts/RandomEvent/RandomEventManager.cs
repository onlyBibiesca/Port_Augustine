using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RandomEventManager : MonoBehaviour
{
    [Header("GameObjects")]
    public GameObject freakyPNG;

    //Defining events
    public enum EventType
    {
        Notification,
        Audio,
        Picture
    }
    private float eventInterval;
    private float timer;

    private void Awake()
    {
        eventInterval = Random.Range(3f, 10f);
        timer = 0f;
    }


    void Start()
    {
        eventInterval = Random.Range(10f, 20f);
    }
    void Update()
    {
        //set interval timer
        timer += Time.deltaTime;

        if (timer >= eventInterval)
        {
            Debug.Log("EventInterval " + eventInterval);
            TriggerRandomEvent();
            timer = 0f; //resets timer
        }
    }

    void TriggerRandomEvent()
    {
        Debug.Log("RandomEvent Triggering");
        //random choose event
        float randomChance = Random.Range(0f, 1f);
        Debug.Log(randomChance);

        //lowest to highest chance format
        if (randomChance >= 0.7f) //20 percent
        {
            DisplayFreaky();
        }
        /*EventType randomEvent = (EventType)Random.Range(0, System.Enum.GetValues(typeof(EventType)).Length);

        //execute chosen event
        switch (randomEvent)
        {
            case EventType.Notification:
                PlayNotification();
                break;

            case EventType.Audio:
                PlayWhisper();
                break;

            case EventType.Picture:
                DisplayPicture();
                break;
        }*/
    }

    void DisplayFreaky()
    {
        freakyPNG.SetActive(true);
        Debug.Log("Freaky NPC Triggering");
    }

    

}

