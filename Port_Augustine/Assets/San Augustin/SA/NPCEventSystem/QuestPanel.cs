using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class QuestPanel : MonoBehaviour
{
    public static QuestPanel Instance;

    [Header("UI References")]
    public Transform questContainer; // Parent object with Vertical Layout Group
    public GameObject questItemPrefab; // Prefab for each quest item

    [Header("Debug")]
    public bool showDebugMessages = true;

    private Dictionary<string, GameObject> activeQuests = new Dictionary<string, GameObject>();

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
        // Validate setup
        if (questContainer == null)
        {
            Debug.LogError("Quest Container not assigned!");
        }

        if (questItemPrefab == null)
        {
            Debug.LogError("Quest Item Prefab not assigned!");
        }
    }

    // Add quest to panel
    public void AddQuest(string questKey, string questName, string questDescription)
    {
        if (activeQuests.ContainsKey(questKey))
        {
            if (showDebugMessages)
                Debug.Log($"Quest already exists: {questKey}");
            return;
        }

        if (questContainer == null || questItemPrefab == null)
        {
            Debug.LogError("Quest Panel not properly configured!");
            return;
        }

        // Create quest item
        GameObject questItem = Instantiate(questItemPrefab, questContainer);

        // Set quest name text
        TextMeshProUGUI questText = questItem.GetComponent<TextMeshProUGUI>();
        if (questText == null)
        {
            questText = questItem.GetComponentInChildren<TextMeshProUGUI>();
        }

        if (questText != null)
        {
            questText.text = questName;
        }

        // Set quest description if there's a description component
        Transform descriptionTransform = questItem.transform.Find("Description");
        if (descriptionTransform != null)
        {
            TextMeshProUGUI questDescriptionText = descriptionTransform.GetComponent<TextMeshProUGUI>();
            if (questText == null)
            {
                questText = questItem.GetComponentInChildren<TextMeshProUGUI>();
            }
            if (questDescriptionText != null)
            {
                questDescriptionText.text = questDescription;
            }
        }

    }

    // Remove quest from panel
    public void RemoveQuest(string questKey)
    {
        if (!activeQuests.ContainsKey(questKey))
        {
            if (showDebugMessages)
                Debug.Log($"Quest not found: {questKey}");
            return;
        }

        GameObject questItem = activeQuests[questKey];
        activeQuests.Remove(questKey);
        Destroy(questItem);

        if (showDebugMessages)
            Debug.Log($"Quest removed: {questKey}");
    }

    // Clear all quests
    public void ClearAllQuests()
    {
        foreach (var questItem in activeQuests.Values)
        {
            Destroy(questItem);
        }
        activeQuests.Clear();

        if (showDebugMessages)
            Debug.Log("All quests cleared");
    }

    // Check if quest exists
    public bool HasQuest(string questKey)
    {
        return activeQuests.ContainsKey(questKey);
    }
}
