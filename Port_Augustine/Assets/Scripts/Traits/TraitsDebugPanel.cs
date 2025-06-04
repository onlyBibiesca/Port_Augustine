using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class TraitsDebugPanel : MonoBehaviour
{
    public TraitManager traitManager;
    public TMP_Dropdown traitDropdown;
    public Button addTraitButton;
    public Button clearTraitsButton;

    private void Start()
    {
        PopulateTraitDropdown();

        addTraitButton.onClick.AddListener(() =>
        {
            int index = traitDropdown.value;
            if (index >= 0 && index < traitManager.availableTraits.Count)
            {
                Trait selectedTrait = traitManager.availableTraits[index];
                traitManager.AddTrait(selectedTrait);
                Debug.Log($"Added trait: {selectedTrait.traitName}");
            }
        });

        clearTraitsButton.onClick.AddListener(() =>
        {
            traitManager.ClearAllTraits();
            Debug.Log("Cleared all traits.");
        });
    }

    void PopulateTraitDropdown()
    {
        traitDropdown.ClearOptions();
        var options = new System.Collections.Generic.List<string>();

        foreach (Trait trait in traitManager.availableTraits)
        {
            options.Add(trait.traitName);
        }

        traitDropdown.AddOptions(options);
    }
}