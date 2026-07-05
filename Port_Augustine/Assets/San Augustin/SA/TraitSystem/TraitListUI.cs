using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class TraitListUI : MonoBehaviour
{
    [SerializeField] private TMP_Text traitText;

    private void Start()
    {
        TraitsManager.Instance.OnTraitsChanged += Refresh;
        Refresh();
    }

    private void OnDestroy()
    {
        if (TraitsManager.Instance != null)
            TraitsManager.Instance.OnTraitsChanged -= Refresh;
    }

    private void Refresh()
    {
        traitText.text = TraitsManager.Instance.GetTraitList();
    }
}