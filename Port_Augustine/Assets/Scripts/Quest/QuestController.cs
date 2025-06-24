using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class QuestController : MonoBehaviour
{
    public static QuestController Instance {  get; private set; }
    public List<QuestProgress> activateQuests = new();
    private QuestUI questUI;

    public void Awake()
    {
        if (Instance == null) Instance = this;
        else Destroy(gameObject);

        questUI = FindObjectOfType<QuestUI>();
        QuestInventoryController.Instance.OnInventoryChanged += CheckInventoryForQuests;
    }

    public void AcceptQuest(Quest quest)
    {
        if (IsQuestActive(quest.questID)) return;
        
            activateQuests.Add(new QuestProgress(quest));

            questUI.UpdateQuestUI();
        
    }

    public bool IsQuestActive(string questID) =>activateQuests.Exists(q => q.QuestID == questID);  

    public void CheckInventoryForQuests()
    {
        Dictionary<int, int> itemCounts = QuestInventoryController.Instance.GetItemCounts();

        foreach(QuestProgress quest in activateQuests)
        {
            foreach (QuestObjective questObjective in quest.objectives)
            {
                if(questObjective.type != ObjectiveType.CollectItem) continue;
                if (!int.TryParse(questObjective.objectiveID, out int itemID)) continue;

                int newAmount = itemCounts.TryGetValue(itemID, out int count) ? Mathf.Min(count, questObjective.requiredAmount) : 0;

                if(questObjective.currentAmount != newAmount)
                {
                    questObjective.currentAmount = newAmount;
                }
            }
        }
        questUI.UpdateQuestUI();
    }
}
