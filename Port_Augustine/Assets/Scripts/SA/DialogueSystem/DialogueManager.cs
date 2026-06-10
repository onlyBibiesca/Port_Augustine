using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class DialogueManager : MonoBehaviour
{
    public static DialogueManager Instance;
    private string currentNPCName = "Unknown";

    [Header("UI References")]
    public GameObject dialoguePanel;
    public TMPro.TMP_Text speakerNameText;
    public TMPro.TMP_Text dialogueText;
    public UnityEngine.UI.Button nextButton;

    [Header("Portrait References")]
    public GameObject portraitContainer;
    public UnityEngine.UI.Image portraitImage;

    [Header("Choice UI References")]
    public GameObject choicePanel;
    public GameObject choiceButtonPrefab;
    public Transform choiceContainer;

    [Header("Typewriter Settings")]
    public bool enableTypewriter = true;
    public float typewriterSpeed = 0.05f; // Time between each character
    public KeyCode skipTypewriterKey = KeyCode.Space;

    [Header("Choice Settings")]
    public float delayBeforeChoices = 2f;

    private Dialogue currentDialogue;
    private int currentLineIndex = 0;
    private System.Action onDialogueComplete;
    private bool waitingForChoice = false;
    private Coroutine typewriterCoroutine;
    private bool isTyping = false;
    private string fullText = "";



    public bool IsDialogueActive { get; private set; }

    void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            Debug.Log("DialogueManager initialized");
        }
        else
        {
            Destroy(gameObject);
        }
    }

    void Start()
    {
        IsDialogueActive = false;

        if (dialoguePanel != null)
        {
            dialoguePanel.SetActive(false);
        }
        else
        {
            Debug.LogError("Dialogue Panel not assigned in DialogueManager!");
        }

        if (choicePanel != null)
        {
            choicePanel.SetActive(false);
        }
        else
        {
            Debug.LogWarning("Choice Panel not assigned. Choices won't work!");
        }

        if (portraitContainer != null)
        {
            portraitContainer.SetActive(false);
        }

        if (nextButton != null)
        {
            nextButton.onClick.AddListener(DisplayNextLine);
        }
        else
        {
            Debug.LogError("Next Button not assigned in DialogueManager!");
        }
    }

    void Update()
    {
        // Allow skipping typewriter effect
        if (isTyping && Input.GetKeyDown(skipTypewriterKey))
        {
            SkipTypewriter();
        }
    }

    public void StartDialogue(Dialogue dialogue, System.Action onComplete = null, string npcName = "Unknown")
    {
        Debug.Log($"Starting dialogue: {dialogue.dialogueName}");

        IsDialogueActive = true;
        currentDialogue = dialogue;
        currentNPCName = npcName;
        currentLineIndex = 0;
        onDialogueComplete = onComplete;
        waitingForChoice = false;

        if (dialoguePanel != null)
        {
            dialoguePanel.SetActive(true);
        }

        if (choicePanel != null)
        {
            choicePanel.SetActive(false);
        }

        if (RelationshipManager.Instance != null)
        {
            RelationshipManager.Instance.ShowRelationshipSlider(npcName);
        }

        DisplayCurrentLine();
    }

    void DisplayCurrentLine()
    {
        if (currentDialogue == null || currentLineIndex >= currentDialogue.lines.Length)
        {
            EndDialogue();
            return;
        }

        DialogueLine line = currentDialogue.lines[currentLineIndex];

        if (speakerNameText != null)
            speakerNameText.text = line.speakerName;

        // Handle character portrait
        if (line.characterPortrait != null)
        {
            if (portraitImage != null)
            {
                portraitImage.sprite = line.characterPortrait;
                portraitImage.enabled = true;
            }

            if (portraitContainer != null)
            {
                portraitContainer.SetActive(true);
            }

            Debug.Log($"Displaying portrait for: {line.speakerName}");
        }
        else
        {
            // No portrait for this line, hide it
            if (portraitContainer != null)
            {
                portraitContainer.SetActive(false);
            }

            if (portraitImage != null)
            {
                portraitImage.enabled = false;
            }
        }

        // Start typewriter effect or display text immediately
        if (enableTypewriter && dialogueText != null)
        {
            fullText = line.text;
            if (typewriterCoroutine != null)
            {
                StopCoroutine(typewriterCoroutine);
            }
            typewriterCoroutine = StartCoroutine(TypewriterEffect(line.text));
        }
        else if (dialogueText != null)
        {
            dialogueText.text = line.text;
            OnTypewriterComplete();
        }

        Debug.Log($"Displaying line {currentLineIndex}: {line.text}");
    }

    System.Collections.IEnumerator TypewriterEffect(string text)
    {
        isTyping = true;
        dialogueText.text = "";

        // Disable next button while typing
        if (nextButton != null)
            nextButton.interactable = false;

        foreach (char c in text)
        {
            dialogueText.text += c;
            yield return new WaitForSeconds(typewriterSpeed);
        }

        isTyping = false;
        OnTypewriterComplete();
    }

    void SkipTypewriter()
    {
        if (typewriterCoroutine != null)
        {
            StopCoroutine(typewriterCoroutine);
            typewriterCoroutine = null;
        }

        isTyping = false;
        dialogueText.text = fullText;
        OnTypewriterComplete();
    }

    void OnTypewriterComplete()
    {
        // Re-enable next button after typing completes
        if (nextButton != null)
            nextButton.interactable = true;

        // Check if this is the last line and has choices
        bool isLastLine = (currentLineIndex == currentDialogue.lines.Length - 1);
        DialogueLine line = currentDialogue.lines[currentLineIndex];

        if (isLastLine && line.hasChoices && line.choices.Length > 0)
        {
            // Hide next button and prepare to show choices
            if (nextButton != null)
                nextButton.gameObject.SetActive(false);

            waitingForChoice = true;
            StartCoroutine(ShowChoicesAfterDelay(line.choices));
        }
        else
        {
            // Normal line, show next button
            if (nextButton != null)
                nextButton.gameObject.SetActive(true);
        }
    }

    public void DisplayNextLine()
    {
        if (waitingForChoice || isTyping)
            return;

        currentLineIndex++;
        DisplayCurrentLine();
    }

    System.Collections.IEnumerator ShowChoicesAfterDelay(Dialogue_Choice[] choices)
    {
        yield return new WaitForSeconds(delayBeforeChoices);
        ShowChoices(choices);
    }
    /*
    void ShowChoices(Dialogue_Choice[] choices)
    {
        if (choicePanel == null || choiceContainer == null || choiceButtonPrefab == null)
        {
            Debug.LogError("Choice UI not properly set up!");
            EndDialogue();
            return;
        }

        // Clear any existing choice buttons
        foreach (Transform child in choiceContainer)
        {
            Destroy(child.gameObject);
        }

        // Show choice panel
        choicePanel.SetActive(true);

        // Create a button for each choice
        foreach (Dialogue_Choice choice in choices)
        {
            GameObject buttonObj = Instantiate(choiceButtonPrefab, choiceContainer);
            UnityEngine.UI.Button button = buttonObj.GetComponent<UnityEngine.UI.Button>();

            // Try to find TMP_Text component
            TMPro.TMP_Text buttonText = buttonObj.GetComponentInChildren<TMPro.TMP_Text>();

            // If TMP not found, try standard Text
            if (buttonText == null)
            {
                UnityEngine.UI.Text standardText = buttonObj.GetComponentInChildren<UnityEngine.UI.Text>();
                if (standardText != null)
                {
                    standardText.text = choice.choiceText;
                    Debug.Log($"Set button text (Standard) to: {choice.choiceText}");
                }
                else
                {
                    Debug.LogError("Could not find Text or TMP_Text component on choice button!");
                }
            }
            else
            {
                buttonText.text = choice.choiceText;
                Debug.Log($"Set button text (TMP) to: {choice.choiceText}");
            }

            // Capture the choice in a local variable for the lambda
            Dialogue_Choice selectedChoice = choice;
            button.onClick.AddListener(() => OnChoiceSelected(selectedChoice));
        }

        Debug.Log($"Showing {choices.Length} choices");
    }
    */
    void ShowChoices(Dialogue_Choice[] choices)
    {


        if (choicePanel == null || choiceContainer == null || choiceButtonPrefab == null)
        {
            Debug.LogError("Choice UI not properly set up!");
            EndDialogue();
            return;
        }

        // Clear any existing choice buttons
        foreach (Transform child in choiceContainer)
        {
            Destroy(child.gameObject);
        }

        // Show choice panel
        choicePanel.SetActive(true);

        foreach (Dialogue_Choice choice in choices)
        {
            // TRAIT CHECK
            if (choice.requiredTrait != null)
            {
                bool hasTrait = TraitsManager.Instance.HasTrait(choice.requiredTrait);

                if (!hasTrait && choice.hideIfTraitMissing)
                {
                    continue;
                }
            }

            GameObject buttonObj = Instantiate(choiceButtonPrefab, choiceContainer);

            UnityEngine.UI.Button button =
                buttonObj.GetComponent<UnityEngine.UI.Button>();

            TMPro.TMP_Text buttonText =
                buttonObj.GetComponentInChildren<TMPro.TMP_Text>();

            if (buttonText != null)
            {
                // Optional locked text
                if (choice.requiredTrait != null &&
                    !TraitsManager.Instance.HasTrait(choice.requiredTrait))
                {
                    buttonText.text = "[Locked]";
                    button.interactable = false;
                }
                else
                {
                    buttonText.text = choice.choiceText;
                }
            }

            Dialogue_Choice selectedChoice = choice;
            button.onClick.AddListener(() => OnChoiceSelected(selectedChoice));
        }
    }


    void OnChoiceSelected(Dialogue_Choice choice)
    {
        Debug.Log($"Choice selected: {choice.choiceText}");

        // APPLY RELATIONSHIP CHANGE
        if (choice.relationshipChange != 0 && RelationshipManager.Instance != null)
        {
            RelationshipManager.Instance.ChangeRelationship(currentNPCName, choice.relationshipChange);
            Debug.Log($"{currentNPCName}: Relationship {(choice.relationshipChange > 0 ? "increased" : "decreased")} by {Mathf.Abs(choice.relationshipChange)}");
        }

        // Hide choices
        if (choicePanel != null)
            choicePanel.SetActive(false);

        waitingForChoice = false;

        // If the choice leads to another dialogue, play it
        if (choice.nextDialogue != null)
        {
            StartDialogue(choice.nextDialogue, onDialogueComplete);
        }
        else
        {
            // No follow-up dialogue, just end
            EndDialogue();
        }

        if (choice.gainedTrait != null)
        {
            TraitsManager.Instance.AddTrait(choice.gainedTrait);
        }
    }

    void EndDialogue()
    {
        Debug.Log("Dialogue ended");

        // Consume time when dialogue ends
        if (currentDialogue != null && currentDialogue.consumesTime)
        {
            if (TimeSystem.Instance != null)
            {
                TimeSystem.Instance.ConsumeTime(currentDialogue as ITimeConsumer);
                Debug.Log($"Dialogue consumed {currentDialogue.hoursToConsume}h {currentDialogue.minutesToConsume}m");
            }
            else
            {
                Debug.LogError("TimeSystem not found in scene!");
            }
        }

        // Consume stats when dialogue ends
        if (currentDialogue != null && currentDialogue.statConsumable != null)
        {
            if (PlayerStats.Instance != null)
            {
                PlayerStats.Instance.ConsumeStat(currentDialogue.statConsumable);
            }
            else
            {
                Debug.LogError("PlayerStats not found in scene!");
            }
        }

        IsDialogueActive = false;
        isTyping = false;

        if (typewriterCoroutine != null)
        {
            StopCoroutine(typewriterCoroutine);
            typewriterCoroutine = null;
        }

        if (dialoguePanel != null)
            dialoguePanel.SetActive(false);

        if (choicePanel != null)
            choicePanel.SetActive(false);

        if (portraitContainer != null)
            portraitContainer.SetActive(false);

        if (RelationshipManager.Instance != null)
        {
            RelationshipManager.Instance.HideRelationshipSlider();
        }

        currentDialogue = null;
        waitingForChoice = false;
        onDialogueComplete?.Invoke();
    }
}
