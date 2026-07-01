using Cinemachine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering.VirtualTexturing;
using static TimeSystem;

public class InteractableTransitions : MonoBehaviour, InteractableObject
{
    private GameObject player;

    private InteractableObject nearbyInteractable;
    private MapTransition mapTransition;

    [Header("InteractUI")]
    [SerializeField] GameObject interactUI;

    [Header("Sounds")]
    [SerializeField] AudioSource buttonSound;

    [Header("Go to Confiner")]
    [SerializeField] PolygonCollider2D mapBoundary;
    [SerializeField] Direction direction;

    [Header("Next Confiner waypoint")]
    [SerializeField] Transform teleportPosition;

    [Header("Prefab Transition Animation")]
    [SerializeField] Animation transitionAnim; //this is for our cute designer just to drag the animation
    [SerializeField] int animationTimer; //how long does the animation last
    CinemachineConfiner confiner;

    [Header("Commute Price")]
    [SerializeField] int commutePrice;
    [SerializeField] Wallet wallet;

    [Header("Consumables Modular")]
    [SerializeField] TimeConsumable timeConsumable;
    [SerializeField] StatConsumable statConsumable;

    [Header("Virtual Camera")]
    [SerializeField] private CinemachineVirtualCamera virtualCamera;
    [SerializeField] private Transform playerTransform;

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
        buttonSound.Play();
        Debug.Log("========== INTERACT CALLED ==========");
        Debug.Log($"Player is null? {player == null}");
        Debug.Log($"MapBoundary is null? {mapBoundary == null}");

        if (wallet.money >= commutePrice)
        {
            wallet.money -= commutePrice;
            if (player != null && mapBoundary != null)
            {
                // DETAILED TIME CONSUMPTION DEBUGGING
                Debug.Log("--- TIME CONSUMPTION CHECK ---");
                Debug.Log($"timeConsumable is null? {timeConsumable == null}");

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

                if (timeConsumable != null && TimeSystem.Instance != null)
                {
                    Debug.Log(">>> CALLING ConsumeTime() <<<");
                    TimeSystem.Instance.ConsumeTime(timeConsumable);
                    Debug.Log($"Current time AFTER: {TimeSystem.Instance.GetFormattedTime()}");
                }
                else if (timeConsumable == null)
                {
                    Debug.LogWarning("No TimeConsumable assigned to this transition!");
                }
                else if (TimeSystem.Instance == null)
                {
                    Debug.LogError("TimeSystem not found in scene!");
                }

                if (PlayerStats.Instance != null && statConsumable != null)
                {
                    if (PlayerStats.Instance != null)
                    {
                        PlayerStats.Instance.ConsumeStat(statConsumable);
                    }
                    else
                    {
                        Debug.LogError("PlayerStats not found in scene!");
                    }
                }

                Debug.Log("========== INTERACT END ==========");



                StartCoroutine(DelayedAction(player));
                transitionAnim.Play();
                Debug.Log("Waiting animation to be done for " + animationTimer);
            }
            else
            {
                Debug.LogWarning("player or mapTransition or stat consumable is null!");
            }
        }
        else
        {
            Debug.Log("Insuffecient funds");
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
        Debug.Log("Entering " + mapBoundary);
    }

    public void ChangePlayerPosition(GameObject player)
    {
        if (direction == Direction.teleport)
        {
            Vector3 delta = teleportPosition.position - player.transform.position; // delta BEFORE moving

            player.transform.position = teleportPosition.position; // move player

            virtualCamera.OnTargetObjectWarped(playerTransform, delta); // snap camera
            SwapConfiner(); // swap confiner
            return;
        }
    }

    private void SwapConfiner()
    {
        var confiner = virtualCamera.GetComponent<CinemachineConfiner>();
        confiner.m_BoundingShape2D = mapBoundary;
        confiner.InvalidatePathCache(); // important after swapping!
    }

}
