using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class QuestUI : MonoBehaviour
{
    public Transform questListContent;
    public GameObject questEntryPrefab;
    public GameObject objectiveTextPrefab;

    /*[Header("Starting Quests")]
    public Quest testQuest;
    public int testQuestAmount;
    private List<QuestProgress> testQuests = new();*/
    // Start is called before the first frame update
    void Start()
    {
        /*for(int i = 0; i < testQuestAmount; i++)
        {
            testQuests.Add(new QuestProgress(testQuest));
        }*/

        UpdateQuestUI();
    }

    // Update is called once per frame
    public void UpdateQuestUI()
    {
        //destroy existing quest entries
        foreach(Transform child in questListContent)
        {
            Destroy(child.gameObject);
        }

        //auto quest
        /*foreach (var quest in testQuests)
        {
            GameObject entry = Instantiate(questEntryPrefab, questListContent);
            TMP_Text questNameText = entry.transform.Find("QuestName").GetComponent<TMP_Text>();
            Transform objectiveList = entry.transform.Find("ObjectiveList");

            questNameText.text = quest.quest.name;
            foreach (var objective in quest.objectives)
            {
                GameObject objTextGo = Instantiate(objectiveTextPrefab, objectiveList);
                TMP_Text objText = objTextGo.GetComponent<TMP_Text>();

                if (objective.requiredAmount == 0)
                {
                    objText.text = $"{objective.description}";
                }

                else
                {
                    objText.text = $"{objective.description} ({objective.currentAmount}/{objective.requiredAmount})"; //objective quest
                }
            }
        }*/
        //build quest entries (Quest giver)
        foreach (var quest in QuestController.Instance.activateQuests)
        {
            GameObject entry = Instantiate(questEntryPrefab, questListContent);
            TMP_Text questNameText = entry.transform.Find("QuestName").GetComponent<TMP_Text>();
            Transform objectiveList = entry.transform.Find("ObjectiveList");

            questNameText.text = quest.quest.name;
            foreach(var objective in quest.objectives)
            {
                GameObject objTextGo = Instantiate(objectiveTextPrefab, objectiveList);
                TMP_Text objText = objTextGo.GetComponent<TMP_Text>();

                if(objective.requiredAmount == 0)
                {
                    objText.text = $"{objective.description}";
                }

                else
                {
                    objText.text = $"{objective.description} ({objective.currentAmount}/{objective.requiredAmount})"; //objective quest
                }
            }
        }

    }
}
