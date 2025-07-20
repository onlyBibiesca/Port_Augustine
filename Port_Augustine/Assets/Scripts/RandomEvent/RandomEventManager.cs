using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RandomEventManager : MonoBehaviour
{

    //this script is a single-father handling all the children colliders
    [Header("GameObjects")]
    public GameObject game_1;
    public GameObject game_2;

    //Defining events
   
    private float eventInterval;
    private float timer;

    private void Awake()
    {
        eventInterval = Random.Range(3f, 10f);
        timer = 0f;
    }


    void Start()
    {
        eventInterval = Random.Range(3f, 10f);
    }
    /*void Update()
    {
        //set interval timer
        timer += Time.deltaTime;

        if (timer >= eventInterval)
        {
            Debug.Log("EventInterval " + eventInterval);
            TriggerRandomEvent();
            timer = 0f; //resets timer
        }
    }*/

    public void TriggeredColliders(Collider2D collision)
    {
        //set interval timer
        timer += Time.deltaTime;

        if (timer >= eventInterval)
        {
            Debug.Log("EventInterval " + eventInterval);
            TriggerRandomEvent();
            timer = 0f; //resets timer
        }

        TriggerRandomEvent();
    }

    void TriggerRandomEvent()
    {
        Debug.Log("RandomEvent Triggering");
        //random choose event
        int randomChance = Random.Range(1, 10);
        Debug.Log(randomChance);

        //lowest to highest chance format
        if (randomChance == 1) //20 percent
        {
            DisplayGame();
        }

        if (randomChance == 2)
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
        Debug.Log(game_1 + "Triggered");
    }

    void DisplaySubtraction()
    {
        game_2.SetActive(true);
        Debug.Log(game_2 + "Triggering");
    }
}

