using UnityEngine;
using UnityEngine.EventSystems;

public class HighlightInteractManager : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler, IPointerClickHandler
{
    [Header("Highlight and UI")]
    public GameObject UIHighlight;
    public GameObject ToggleableMenu;

    [Header("Hover Animation Settings")]
    public float hoverLiftAmount = 15f;
    public float tweenDuration = 0.5f;

    private Vector3 originalLocalPos;

    private void Start()
    {
        originalLocalPos = transform.localPosition;

        if (UIHighlight != null)
            UIHighlight.SetActive(false);
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        if (UIHighlight != null)
            UIHighlight.SetActive(true);

        Debug.Log($"Mouse has entered {gameObject.name}");

        // Animate upward
        LeanTween.moveLocalY(gameObject, originalLocalPos.y + hoverLiftAmount, tweenDuration).setEase(LeanTweenType.easeOutSine);
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        if (UIHighlight != null)
            UIHighlight.SetActive(false);

        Debug.Log($"Mouse has left {gameObject.name}");

        // Animate back to original position
        LeanTween.moveLocalY(gameObject, originalLocalPos.y, tweenDuration).setEase(LeanTweenType.easeOutSine);
    }

    public void OnPointerClick(PointerEventData eventData)
    {
        if (ToggleableMenu != null)
        {
            bool isActive = ToggleableMenu.activeSelf;
            ToggleableMenu.SetActive(!isActive);
            Debug.Log($"Toggles {gameObject.name}");
        }
    }
}
