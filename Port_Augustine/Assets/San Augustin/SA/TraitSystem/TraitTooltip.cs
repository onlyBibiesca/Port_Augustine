using TMPro;
using UnityEngine;

public class TraitTooltip : MonoBehaviour
{
    public static TraitTooltip Instance;

    [Header("UI")]
    [SerializeField] private GameObject panel;
    [SerializeField] private TMP_Text descriptionText;

    [Header("Cursor")]
    [SerializeField] private Vector2 offset = new Vector2(20f, -20f);

    private RectTransform rectTransform;

    private void Awake()
    {
        Instance = this;

        rectTransform = panel.GetComponent<RectTransform>();

        panel.SetActive(false);
    }

    private void Update()
    {
        if (!panel.activeSelf)
            return;

        rectTransform.position = (Vector2)Input.mousePosition + offset;
    }

    public void Show(TraitSO trait)
    {
        if (trait == null)
            return;

        descriptionText.text = trait.description;
        panel.SetActive(true);
    }

    public void Hide()
    {
        panel.SetActive(false);
    }
}