
using Cinemachine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MapTransition : MonoBehaviour
{

    [SerializeField] PolygonCollider2D mapBoundary;
    [SerializeField] Direction direction;
    [SerializeField] Transform teleportPosition;
    [SerializeField] float distancePos;
    CinemachineConfiner confiner;

    enum Direction {  up, down, left, right, teleport }

    private void Awake()
    {
        confiner = FindObjectOfType<CinemachineConfiner>(); 
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.CompareTag("Player"))
        {

            confiner.m_BoundingShape2D = mapBoundary;
            UpdatePlayerPosition(collision.gameObject);
            Debug.Log("Entering " + mapBoundary);
        }
    }

    private void UpdatePlayerPosition(GameObject player)
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
}
