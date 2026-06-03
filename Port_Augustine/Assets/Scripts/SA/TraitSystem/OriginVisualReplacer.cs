using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class OriginVisualReplacer : MonoBehaviour
{
    public Image targetImage;

    void OnEnable()
    {
        TraitsManager.Instance.OnTraitsChanged += Refresh;
    }

    void OnDisable()
    {
        if (TraitsManager.Instance != null)
            TraitsManager.Instance.OnTraitsChanged -= Refresh;
    }

    void Start()
    {
        Refresh();
    }

    void Refresh()
    {
        var origins =
            TraitsManager.Instance.GetTraitsOfCategory(TraitCategory.Origin);

        if (origins.Count == 0)
        {
            Debug.Log("No origin found yet");
            return;
        }

        var origin = origins[0];

        if (origin.originPreviewSprite == null)
        {
            Debug.LogWarning($"No sprite assigned to {origin.traitName}");
            return;
        }

        targetImage.sprite = origin.originPreviewSprite;

        Debug.Log($"Origin visual updated: {origin.traitName}");
    }
}