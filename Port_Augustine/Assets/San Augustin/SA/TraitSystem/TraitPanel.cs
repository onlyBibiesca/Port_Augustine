using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TraitPanel : MonoBehaviour
{
    public static TraitPanel Instance;

    [Header("UI References")]
    [SerializeField] private Transform traitContainer;
    [SerializeField] private GameObject traitEntryPrefab;

    [Header("Debug")]
    [SerializeField] private bool showDebugMessages = true;

    private Dictionary<TraitSO, GameObject> activeTraitEntries =
        new Dictionary<TraitSO, GameObject>();

    private void Awake()
    {
        if (Instance == null)
            Instance = this;
        else
            Destroy(gameObject);
    }

    private void Start()
    {
        RefreshTraits();

        if (TraitsManager.Instance != null)
            TraitsManager.Instance.OnTraitsChanged += RefreshTraits;
    }

    private void OnDestroy()
    {
        if (TraitsManager.Instance != null)
            TraitsManager.Instance.OnTraitsChanged -= RefreshTraits;
    }

    public void RefreshTraits()
    {
        // Remove old entries
        foreach (GameObject entry in activeTraitEntries.Values)
        {
            Destroy(entry);
        }

        activeTraitEntries.Clear();

        // Create new entries
        foreach (TraitSO trait in TraitsManager.Instance.ActiveTraits)
        {
            GameObject obj = Instantiate(traitEntryPrefab, traitContainer);

            TraitEntry entry = obj.GetComponent<TraitEntry>();

            if (entry != null)
                entry.Initialize(trait);

            activeTraitEntries.Add(trait, obj);

            if (showDebugMessages)
                Debug.Log($"Added Trait UI: {trait.traitName}");
        }
    }
}