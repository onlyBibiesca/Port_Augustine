using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class NPC : MonoBehaviour, InteractableObject
{
    public NPCDialogue dialogueData;
    private DialogueController dialogueUI;

    private int dialogueIndex;
    private bool isTyping, isDialogueActive;

    private enum QuestState { NotStarted, InProgress, Completed}
    private QuestState questState = QuestState.NotStarted;

    [SerializeField] 
    private TraitManager traitManager; // assign this in Inspector
    private void Start()
    {
        dialogueUI = DialogueController.Instance;
    }
    public void Interact()
    {
        Debug.Log("Interacted");

        if (isDialogueActive)
        {
            NextLine();
        }

        else
        {
            StartDialogue();
        }
    }

    void StartDialogue()
    {
        //sync quest data
        SyncQuestData();

        //set dialogue line based on questsate
        if(questState == QuestState.NotStarted)
        {
            dialogueIndex = 0;
        }
        else if(questState == QuestState.InProgress)
        {
            dialogueIndex = dialogueData.questInProgressIndex;
        }
        else if(questState == QuestState.Completed) 
        {
            dialogueIndex = dialogueData.questCompletedIndex;
        }
        
        isDialogueActive = true;

        dialogueUI.SetNPCInfo(dialogueData.npcName, dialogueData.npcPortrait);
        dialogueUI.ShowDialogueUI(true);


        DisplayCurrentLine();
        Debug.Log("Starting Dialogue");
    }

    private void SyncQuestData()
    {
        if (dialogueData.quest == null) return;

        string questID = dialogueData.quest.questID;
        if(QuestController.Instance.IsQuestActive(questID))
        {
            questState = QuestState.InProgress;
        }
        else
        {
            questState = QuestState.NotStarted;
        }
    }

    void NextLine()
    {
        Debug.Log("Next Line....");
        if (isTyping)
        {
            StopAllCoroutines();
            dialogueUI.SetDialogueText(dialogueData.dialogueLines[dialogueIndex]);
            isTyping = false;
        }

        //clearchoices
        dialogueUI.ClearChoices();

        //checkdialoguelines
        if (dialogueData.endDialogueLines.Length > dialogueIndex && dialogueData.endDialogueLines[dialogueIndex])
        {
            EndDialogue();
            return;
        }

        //checkif choices
        foreach(DialogueChoice dialogueChoice in dialogueData.choices)
        {
            if (dialogueChoice.dialogueIndex == dialogueIndex)
            {
                DisplayChoices(dialogueChoice);
                return;
            }
        }

        if (++dialogueIndex < dialogueData.dialogueLines.Length)
        {
            DisplayCurrentLine();
        }

        else
        {
            EndDialogue();
        }
    }

    IEnumerator TypeLine()
    {
        isTyping = true;
        dialogueUI.SetDialogueText("");

        foreach (char letter in dialogueData.dialogueLines[dialogueIndex])
        {
            dialogueUI.SetDialogueText(dialogueUI.dialogueText.text += letter);
            yield return new WaitForSeconds(dialogueData.typingSpeed);
        }

        isTyping = false;

        if (dialogueData.autoProgressLines.Length > dialogueIndex && dialogueData.autoProgressLines[dialogueIndex])
        {
            yield return new WaitForSeconds(dialogueData.autoProgressDelay);
            NextLine();
        }
    }
    /*
    void DisplayChoices(DialogueChoice choice)
    {
        for(int i = 0; i < choice.choices.Length; i++)
        {
            int nextIndex = choice.nextDialogueIndexes[i];
            dialogueUI.CreateChoiceButton(choice.choices[i], () => ChooseOption(nextIndex));
        }
    }
    */

    void DisplayChoices(DialogueChoice choice)
    {
        for (int i = 0; i < choice.choices.Length; i++)
        {
            bool shouldShow = true;

            // Check if trait is required for this choice
            if (choice.requiredTraits != null && choice.requiredTraits.Length > i)
            {
                string requiredTrait = choice.requiredTraits[i];
                if (!string.IsNullOrEmpty(requiredTrait) && !traitManager.HasTraitKeyword(requiredTrait))
                {
                    shouldShow = false;
                }
            }

            if (shouldShow)
            {
                int nextIndex = choice.nextDialogueIndexes[i];
                bool givesQuest = choice.givesQuest[i];
                dialogueUI.CreateChoiceButton(choice.choices[i], () => ChooseOption(nextIndex, givesQuest));
            }
        }
    }

    void ChooseOption(int nextIndex, bool givesQuest)
    {
        if(givesQuest)
        {
            QuestController.Instance.AcceptQuest(dialogueData.quest);
            questState = QuestState.InProgress;
        }
        dialogueIndex = nextIndex;
        dialogueUI.ClearChoices();
        DisplayCurrentLine();
    }

    void DisplayCurrentLine()
    {
        StopAllCoroutines();
        StartCoroutine(TypeLine());
    }

    public void EndDialogue()
    {
        StopAllCoroutines();
        isDialogueActive = false;
        dialogueUI.SetDialogueText("");
        dialogueUI.ShowDialogueUI(false);

    }

}
