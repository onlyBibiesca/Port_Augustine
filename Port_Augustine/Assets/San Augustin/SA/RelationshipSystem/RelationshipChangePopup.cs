using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class RelationshipChangePopup : MonoBehaviour
{
    public static RelationshipChangePopup Instance;

    [Header("Popup Settings")]
    public Canvas popupCanvas;
    public GameObject popupPrefab;

    [Header("Animation")]
    public float popupDuration = 2f; // How long popup stays visible
    public float popupMoveDistance = 50f; // How far it moves up
    public float fadeDuration = 1.5f; // How long fade takes

    [Header("Colors")]
    public Color positiveColor = Color.green;
    public Color negativeColor = Color.red;
    public Color neutralColor = Color.white;

    [Header("Font Size")]
    public int fontSize = 48;

    void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }
        else
        {
            Destroy(gameObject);
        }
    }

    void Start()
    {
        // Create canvas if not assigned
        if (popupCanvas == null)
        {
            CreatePopupCanvas();
        }

        // Create popup prefab if not assigned
        if (popupPrefab == null)
        {
            CreatePopupPrefab();
        }
    }

    // Show relationship change popup
    public void ShowRelationshipChangePopup(int relationshipChange, Vector3 worldPosition = default)
    {
        if (popupPrefab == null || popupCanvas == null)
        {
            Debug.LogWarning("Popup prefab or canvas not set up!");
            return;
        }

        // Create popup instance
        GameObject popupInstance = Instantiate(popupPrefab, popupCanvas.transform);

        // Get text component
        TextMeshProUGUI popupText = popupInstance.GetComponent<TextMeshProUGUI>();
        if (popupText == null)
        {
            popupText = popupInstance.GetComponentInChildren<TextMeshProUGUI>();
        }

        if (popupText == null)
        {
            Debug.LogError("Popup prefab doesn't have TextMeshProUGUI component!");
            Destroy(popupInstance);
            return;
        }

        // Set text
        string sign = relationshipChange > 0 ? "+" : "";
        popupText.text = $"{sign}{relationshipChange}";

        // Set color based on change
        if (relationshipChange > 0)
        {
            popupText.color = positiveColor;
        }
        else if (relationshipChange < 100)
            popupText.color = negativeColor;
        else
            popupText.color = neutralColor;

        // Start animation
        StartCoroutine(AnimatePopup(popupInstance, popupText));
    }

    IEnumerator AnimatePopup(GameObject popupInstance, TextMeshProUGUI popupText)
    {
        RectTransform rectTransform = popupInstance.GetComponent<RectTransform>();
        Vector3 startPosition = rectTransform.localPosition;
        Vector3 endPosition = startPosition + Vector3.up * popupMoveDistance;

        float elapsedTime = 0f;

        // Animate
        while (elapsedTime < popupDuration)
        {
            elapsedTime += Time.deltaTime;

            // Move up
            float moveProgress = Mathf.Clamp01(elapsedTime / popupDuration);
            rectTransform.localPosition = Vector3.Lerp(startPosition, endPosition, moveProgress);

            // Fade out
            if (elapsedTime > popupDuration - fadeDuration)
            {
                float fadeProgress = (elapsedTime - (popupDuration - fadeDuration)) / fadeDuration;
                Color color = popupText.color;
                color.a = Mathf.Lerp(1f, 0f, fadeProgress);
                popupText.color = color;
            }

            yield return null;
        }

        // Destroy popup
        Destroy(popupInstance);
    }

    void CreatePopupCanvas()
    {
        // Find existing canvas
        popupCanvas = FindObjectOfType<Canvas>();
        if (popupCanvas == null)
        {
            GameObject canvasObj = new GameObject("PopupCanvas");
            popupCanvas = canvasObj.AddComponent<Canvas>();
            popupCanvas.renderMode = RenderMode.ScreenSpaceOverlay;
            popupCanvas.sortingOrder = 100; // High sorting order
        }

        Debug.Log("Popup canvas created/found");
    }

    void CreatePopupPrefab()
    {
        // Create popup GameObject
        GameObject popup = new GameObject("RelationshipChangePopup");

        // Add RectTransform
        RectTransform rectTransform = popup.AddComponent<RectTransform>();
        rectTransform.sizeDelta = new Vector2(200, 100);
        rectTransform.anchoredPosition = Vector2.zero;

        // Add TextMeshProUGUI
        TextMeshProUGUI tmpText = popup.AddComponent<TextMeshProUGUI>();
        tmpText.text = "+10";
        tmpText.fontSize = fontSize;
        tmpText.alignment = TextAlignmentOptions.Center;
        tmpText.color = Color.green;

        // Add outline for better visibility
        Outline outline = popup.AddComponent<Outline>();
        outline.effectColor = Color.black;
        outline.effectDistance = new Vector2(2, -2);


        // Set as prefab-like object but keep in scene for now
        popupPrefab = popup;
        popup.SetActive(false); // Hide it, we'll instantiate copies

        Debug.Log("Popup prefab created");
    }
}
