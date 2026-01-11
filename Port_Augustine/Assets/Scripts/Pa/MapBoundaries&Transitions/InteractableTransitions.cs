using Cinemachine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InteractableTransitions : MonoBehaviour, InteractableObject
{
    private GameObject player;
    
    private InteractableObject nearbyInteractable;
    private MapTransition mapTransition;

    [Header("InteractUI")]
    [SerializeField] GameObject interactUI;

    [Header("Go to Confiner")]
    [SerializeField] PolygonCollider2D mapBoundary;
    [SerializeField] Direction direction;

    [Header("Next Confiner waypoint")]
    [SerializeField] Transform teleportPosition;

    [Header("Prefab Transition Animation")]
    [SerializeField] Animation transitionAnim; //this is for our cute designer just to drag the animation
    [SerializeField] int animationTimer; //how long does the animation last
    CinemachineConfiner confiner;

    enum Direction { teleport }

    private void Start()
    {
        if (interactUI != null)
            interactUI.SetActive(false);
    }

    private void Awake()
    {
        confiner = FindObjectOfType<CinemachineConfiner>();
        transitionAnim = GetComponent<Animation>();
    }

    public void Interact()
    {
        if (player != null && mapBoundary != null)
        {
            StartCoroutine(DelayedAction(player));
            transitionAnim.Play();
            Debug.Log("Waiting animation to be done for " + animationTimer);
        }
        else
        {
            Debug.LogWarning("player or mapTransition is null!");
        }

    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.CompareTag("Player"))
        {
            player = collision.gameObject;
            InteractableObject interactable = collision.GetComponent<InteractableObject>();
            if (interactable != null)
            {
                nearbyInteractable = interactable;
                if (interactUI != null)
                    interactUI.SetActive(true);
            }
        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.GetComponent<InteractableObject>() == nearbyInteractable)
        {
            nearbyInteractable = null;
            if (interactUI != null)
                interactUI.SetActive(false);
        }
    }

    public IEnumerator DelayedAction(GameObject player)
    {
        yield return new WaitForSeconds(animationTimer);
        ChangePlayerPosition(player);
        confiner.m_BoundingShape2D = mapBoundary;
        Debug.Log("Entering " + mapBoundary);
    }

    public void ChangePlayerPosition(GameObject player)
    {

        if (direction == Direction.teleport)
        {
            player.transform.position = teleportPosition.position;
            return;
        }

        Vector3 newPos = player.transform.position;


        player.transform.position = newPos;
    }

}
