using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Quests/Quest")]
public class Quest : ScriptableObject
{
    public string questID;
    public string questName;
    public string description;
    public List<QuestObjective> objectives;

    private void OnValidate()
    {
        if (string.IsNullOrEmpty(questID))
        {
            questID = questName + Guid.NewGuid().ToString();
        }
    }

    
}

[System.Serializable]
public class QuestObjective
{
    public string objectiveID; //match id
    public string description;
    public ObjectiveType type;
    //public string objectiveDescription;
    public int requiredAmount;
    public int currentAmount;

    public bool isCompleted => currentAmount >= requiredAmount;

}

public enum ObjectiveType { CollectItem, ReachLocation, TalkNPC, Custom }

[System.Serializable]
public class QuestProgress
{
    public Quest quest;
    public List<QuestObjective> objectives;

    public QuestProgress(Quest quest)
    {
        this.quest = quest;
        objectives = new List<QuestObjective>();

        foreach (var ob in quest.objectives)
        {
            objectives.Add(new QuestObjective
            {
                //objectiveID = ob.objectiveID,
                description = ob.description,
                type = ob.type,
                requiredAmount = ob.requiredAmount,
                currentAmount = 0
            });
        }
    }

    public bool isCompleted => objectives.TrueForAll(o => o.isCompleted);

    public string QuestID => quest.questID;
}
