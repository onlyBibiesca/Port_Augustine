using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class TraitObjectPair
{
    public TraitSO trait;
    public GameObject targetObject;
}

public class OriginActivator : MonoBehaviour
{
    public List<TraitObjectPair> originObjects;

    private void OnEnable()
    {
        if (TraitsManager.Instance != null)
            TraitsManager.Instance.OnTraitsChanged += UpdateOriginObjects;
    }

    private void OnDisable()
    {
        if (TraitsManager.Instance != null)
            TraitsManager.Instance.OnTraitsChanged -= UpdateOriginObjects;
    }

    private void Start()
    {
        UpdateOriginObjects(); // initial safe refresh
    }

    public void UpdateOriginObjects()
    {
        if (TraitsManager.Instance == null)
            return;

        foreach (var pair in originObjects)
        {
            bool hasTrait = TraitsManager.Instance.HasTrait(pair.trait);

            Debug.Log(
                $"Checking {pair.trait.traitName} | Has Trait: {hasTrait}"
            );

            pair.targetObject.SetActive(hasTrait);
        }
    }
}