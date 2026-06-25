
using Cinemachine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MapTransition : MonoBehaviour
{
    private GameObject player;

    [Header("Consumables Modular")]
    [SerializeField] TimeConsumable timeConsumable;
    [SerializeField] StatConsumable statConsumable;

    [Header("Go to Confiner")]
    [SerializeField] PolygonCollider2D mapBoundary;
    [SerializeField] Direction direction;

    [Header("Next Confiner waypoint")]
    [SerializeField] Transform teleportPosition;

    [Header("Position Increment")]
    [SerializeField] float distancePos; //increment position of player after teleporting to avoid landing on another collider

    [Header("Prefab Transition Animation")]
    [SerializeField] Animation transitionAnim; //this is for our cute designer just to drag the animation
    [SerializeField] int animationTimer; //how long does the animation last
    CinemachineConfiner confiner;

    enum Direction {  up, down, left, right, teleport } //depedning on the direction, it will be according to the distancePos

   
    private void Awake()
    {
        confiner = FindObjectOfType<CinemachineConfiner>();
        transitionAnim = GetComponent<Animation>();
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.CompareTag("Player"))
        {
            if (timeConsumable != null)
            {
                Debug.Log($"TimeConsumable Name: {timeConsumable.consumableName}");
                Debug.Log($"Consumes Time: {timeConsumable.consumesTime}");
                Debug.Log($"Hours: {timeConsumable.hoursToConsume}");
                Debug.Log($"Minutes: {timeConsumable.minutesToConsume}");
            }

            Debug.Log($"TimeSystem.Instance is null? {TimeSystem.Instance == null}");

            if (TimeSystem.Instance != null)
            {
                Debug.Log($"Current time BEFORE: {TimeSystem.Instance.GetFormattedTime()}");
            }

            if (timeConsumable != null && TimeSystem.Instance != null && statConsumable != null && PlayerStats.Instance != null)
            {
                Debug.Log(">>> CALLING ConsumeTime() <<<");
                TimeSystem.Instance.ConsumeTime(timeConsumable);
                Debug.Log($"Current time AFTER: {TimeSystem.Instance.GetFormattedTime()}");
                PlayerStats.Instance.ConsumeStat(statConsumable);
                Debug.Log($"Stat Consumed");
            }
            else if (timeConsumable == null)
            {
                Debug.LogWarning("No TimeConsumable assigned to this transition!");
            }
            else if (TimeSystem.Instance == null)
            {
                Debug.LogError("TimeSystem not found in scene!");
            }
            else if (PlayerStats.Instance == null)
            {
                Debug.LogError("PlayerStats not found in scene!");
            }

            /*if (player != null && statConsumable != null)
            {
                if (PlayerStats.Instance != null)
                {
                    PlayerStats.Instance.ConsumeStat(statConsumable);
                }
                else
                {
                    Debug.LogError("PlayerStats not found in scene!");
                }
            }*/
            StartCoroutine(DelayedFunction(collision.gameObject));
            transitionAnim.Play();
            Debug.Log("Waiting animation to be done for " + animationTimer);
            /*Invoke("UpdatePlayerPosition", animationTimer);
            transitionAnim.Play();
            Debug.Log("Waiting for " + animationTimer);*/

            /*UpdatePlayerPosition(collision.gameObject);
            confiner.m_BoundingShape2D = mapBoundary;
            Debug.Log("Entering " + mapBoundary);*/
        }

        else
        {
            Debug.LogWarning("player or mapTransition or stat consumable is null!");
        }
    }

    public void UpdatePlayerPosition(GameObject player)
    {

        /*if(direction == Direction.teleport)
        {
            player.transform.position = teleportPosition.position;
            return;
        }*/

        Vector3 newPos = player.transform.position;

        switch (direction)
        {
            case Direction.up:
                newPos.y += distancePos;
                break;
            case Direction.down:
                newPos.y -= distancePos; 
                break;
            case Direction.left:
                newPos.x -= distancePos;
                break;
            case Direction.right:
                newPos.x += distancePos;
                break;
        }

        player.transform.position = newPos;
    }

    public IEnumerator DelayedFunction(GameObject player)
    {
        yield return new WaitForSeconds(animationTimer);
        UpdatePlayerPosition(player);
        confiner.m_BoundingShape2D = mapBoundary;
        Debug.Log("Entering " + mapBoundary);
    }

    
}
