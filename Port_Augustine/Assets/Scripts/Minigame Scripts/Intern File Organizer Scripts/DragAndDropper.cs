using UnityEngine;
using UnityEngine.EventSystems;
using TMPro;

public class DragAndDropper : MonoBehaviour, IBeginDragHandler, IDragHandler, IEndDragHandler
{
    private RectTransform rectTransform;

    public Canvas canvas;
    public TextMeshProUGUI scoreText;

    private void Awake()
    {
        rectTransform = GetComponent<RectTransform>();
    }

    private void Start()
    {
        //BIG NOTE: BECAUSE IT'S A PREFAB, AUTO-ASSIGN THE SHIT!
        //BIG NOTE 2: AVOID PHYSICS BASED TRIGGERS. WE ARE USING TIMESCALE 0 TO PREVENT INTERACTIONS IN THE BACKGROUND!

        //Auto assign the canvas
        if (canvas == null)
        {
            GameObject canvasObj = GameObject.Find("Minigames Canvas");
            if (canvasObj != null)
            {
                canvas = canvasObj.GetComponent<Canvas>();
                Debug.Log("Found Minigames Canvas.");
            }
            else
            {
                Debug.LogWarning("Minigames Canvas not found.");
            }
        }

        //Auto assign the TMP
        if (scoreText == null)
        {
            GameObject scoreObj = GameObject.Find("TEXT_SCORE");
            if (scoreObj != null)
            {
                scoreText = scoreObj.GetComponent<TextMeshProUGUI>();
                Debug.Log("Found TEXT_SCORE.");
            }
            else
            {
                Debug.LogWarning("TEXT_SCORE not found.");
            }
        }
    }

    public void OnBeginDrag(PointerEventData eventData)
    {
        Debug.Log("Picked up paper!");
    }

    public void OnDrag(PointerEventData eventData)
    {
        if (canvas == null) return;
        rectTransform.anchoredPosition += eventData.delta / canvas.scaleFactor;
    }

    public void OnEndDrag(PointerEventData eventData)
    {
        FolderZone closestZone = FindClosestFolderZone();

        // Only accept it if it’s within a reasonable distance (like 100 pixels)
        if (closestZone != null && IsNearZone(closestZone, 100f))
        {
            bool isCorrect = closestZone.AcceptsTag(gameObject.tag);

            if (isCorrect)
            {
                AddScore(10);
                Debug.Log("Correct folder!");
            }
            else
            {
                int penalty = closestZone.IsShredder ? 10 : 5;
                AddScore(-penalty);
                Debug.Log("Wrong folder!");
            }

            Destroy(gameObject);
        }
        else
        {
            Debug.Log("Not inside any folder zone.");
            // DO NOTHING: paper stays where it was dropped
        }
    }

    private FolderZone FindClosestFolderZone()
    {
        FolderZone[] zones = FindObjectsOfType<FolderZone>();
        FolderZone closest = null;
        float minDist = float.MaxValue;

        foreach (var zone in zones)
        {
            float dist = Vector2.Distance(transform.position, zone.transform.position);
            if (dist < minDist)
            {
                minDist = dist;
                closest = zone;
            }
        }

        return closest;
    }

    private bool IsNearZone(FolderZone zone, float maxDistance)
    {
        return Vector2.Distance(transform.position, zone.transform.position) <= maxDistance;
    }

    private void AddScore(int amount)
    {
        if (scoreText != null)
        {
            int currentScore = int.Parse(scoreText.text);
            currentScore += amount;
            scoreText.text = currentScore.ToString();
        }
    }
}
