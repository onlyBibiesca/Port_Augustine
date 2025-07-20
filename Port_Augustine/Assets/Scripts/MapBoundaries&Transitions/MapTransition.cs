
using Cinemachine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MapTransition : MonoBehaviour
{
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
    }

    public void UpdatePlayerPosition(GameObject player)
    {

        if(direction == Direction.teleport)
        {
            player.transform.position = teleportPosition.position;
            return;
        }

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
