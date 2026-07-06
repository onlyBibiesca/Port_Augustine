using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class TraitEntry : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    [SerializeField] private Image icon;
    [SerializeField] private TMP_Text traitNameText;

    private TraitSO trait;

    public void Initialize(TraitSO newTrait)
    {
        trait = newTrait;

        if (icon != null)
            icon.sprite = trait.icon;

        if (traitNameText != null)
            traitNameText.text = trait.traitName;
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        Debug.Log("Hovered over: " + trait.traitName);

        if (TraitTooltip.Instance != null)
        {
            Debug.Log("Tooltip exists.");
            TraitTooltip.Instance.Show(trait);
        }
        else
        {
            Debug.Log("Tooltip Instance is NULL.");
        }
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        if (TraitTooltip.Instance != null)
            TraitTooltip.Instance.Hide();
    }
}