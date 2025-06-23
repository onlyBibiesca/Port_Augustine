using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class NPC : MonoBehaviour, InteractableObject
{
    private DialogueController dialogueUI;
    private PlayerManager player;
    private TraitManager traitManager;

    public NPCDialogue dialogueData; // Still public for debugging, but auto-assigned

    private int dialogueIndex;
    private bool isTyping, isDialogueActive;

    private enum QuestState { NotStarted, InProgress, Completed }
    private QuestState questState = QuestState.NotStarted;

    private void Start()
    {
        player = FindObjectOfType<PlayerManager>();
        traitManager = FindObjectOfType<TraitManager>();
        dialogueUI = DialogueController.Instance;

        if (player == null) Debug.LogError("PlayerManager not found!");
        if (traitManager == null) Debug.LogError("TraitManager not found!");
        if (dialogueUI == null) Debug.LogError("DialogueController.Instance not found!");

        // Auto-load dialogue asset based on the GameObject name
        if (dialogueData == null)
        {
            string npcName = gameObject.name;
            dialogueData = Resources.Load<NPCDialogue>("NPCDialogues/" + npcName);

            if (dialogueData == null)
                Debug.LogWarning($"Dialogue asset for '{npcName}' not found in Resources/NPCDialogues/");
        }
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
                bool givesQuest = choice.givesQuest != null && choice.givesQuest.Length > i && choice.givesQuest[i];
                bool opensShop = choice.opensShop != null && choice.opensShop.Length > i && choice.opensShop[i];
                int capturedIndex = i;

                dialogueUI.CreateChoiceButton(
                    choice.choices[i],
                    () => ChooseOption(nextIndex, givesQuest, opensShop, choice, capturedIndex)
                );
            }
        }
    }

    private string GetChoiceType(DialogueChoice choice, int index)
    {
        if (choice.choiceTypes != null && choice.choiceTypes.Length > index)
            return choice.choiceTypes[index];

        return string.Empty;
    }

    void ChooseOption(int nextIndex, bool givesQuest, bool opensShop, DialogueChoice choice, int choiceIndex)
    {
        if (givesQuest)
        {
            QuestController.Instance.AcceptQuest(dialogueData.quest);
            questState = QuestState.InProgress;
        }

        string currentChoiceType = GetChoiceType(choice, choiceIndex);

        foreach (Trait trait in traitManager.GetAllActiveTraits())
        {
            foreach (DialogueReaction reaction in trait.dialogueReactions)
            {
                if (reaction.choiceType.Equals(currentChoiceType, System.StringComparison.OrdinalIgnoreCase))
                {
                    if (reaction.healthChange != 0) player.ChangeHealth(reaction.healthChange);
                    if (reaction.hungerChange != 0) player.ChangeHunger(reaction.hungerChange);
                    if (reaction.energyChange != 0) player.ChangeEnergy(reaction.energyChange);
                    if (reaction.socialBatteryChange != 0) player.ChangeSocialBattery(reaction.socialBatteryChange);
                    if (reaction.moneyChange != 0) player.AddMoney(reaction.moneyChange);
                }
            }
        }

        if (opensShop)
        {
            ShopController.Instance.OpenShop(() =>
            {
                if (!string.IsNullOrEmpty(dialogueData.shopThankYouMessage))
                {
                    dialogueUI.ClearChoices();
                    dialogueUI.SetDialogueText(dialogueData.shopThankYouMessage);
                }
                else
                {
                    EndDialogue();
                }
            });

            return;
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
