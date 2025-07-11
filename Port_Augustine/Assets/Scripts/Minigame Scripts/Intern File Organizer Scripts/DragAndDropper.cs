using UnityEngine;
using UnityEngine.EventSystems;
using TMPro;

public class DragAndDropper : MonoBehaviour, IBeginDragHandler, IDragHandler, IEndDragHandler
{
    private RectTransform rectTransform;

    public Canvas canvas;
    public TextMeshProUGUI scoreText;

    private FolderZone currentZone = null;

    private void Awake()
    {
        rectTransform = GetComponent<RectTransform>();
    }

    private void Start()
    {
        //BIG NOTE: BECAUSE IT'S A PREFAB, AUTO-ASSIGN THE SHIT!

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
        if (currentZone != null)
        {
            bool isCorrect = currentZone.AcceptsTag(gameObject.tag);

            if (isCorrect)
            {
                AddScore(10);
                Debug.Log("Correct folder!");
            }
            else
            {
                int penalty = currentZone.IsShredder ? 10 : 5;
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

    private void AddScore(int amount)
    {
        if (scoreText != null)
        {
            int currentScore = int.Parse(scoreText.text);
            currentScore += amount;
            scoreText.text = currentScore.ToString();
        }
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        FolderZone zone = collision.GetComponent<FolderZone>();
        if (zone != null)
        {
            currentZone = zone;
        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        FolderZone zone = collision.GetComponent<FolderZone>();
        if (zone != null && currentZone == zone)
        {
            currentZone = null;
        }
    }
}
