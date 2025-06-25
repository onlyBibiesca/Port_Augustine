using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RandomEventManager : MonoBehaviour
{
    [Header("GameObjects")]
    public GameObject game_1;
    public GameObject subGame;

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
        if (randomChance > 0.5f) //20 percent
        {
            DisplayGame();
        }

        if (randomChance < 0.5)
        {
            DisplaySubtraction();
        }
        /*EventType randomEvent = (EventT{ype)Random.Range(0, System.Enum.GetValues(typeof(EventType)).Length);

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

    void DisplayGame()
    {
        game_1.SetActive(true);
        Debug.Log("Freaky NPC Triggering");
    }

    void DisplaySubtraction()
    {
        subGame.SetActive(true);
        Debug.Log("Subtraction Game Triggering");
    }

    

}

