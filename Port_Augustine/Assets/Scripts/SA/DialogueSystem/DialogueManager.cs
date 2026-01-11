using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class DialogueManager : MonoBehaviour
{
    public static DialogueManager Instance;

    [Header("UI References")]
    public GameObject dialoguePanel;
    public TMP_Text speakerNameText;
    public TMP_Text dialogueText;
    public UnityEngine.UI.Button nextButton;

    [Header("Choice UI References")]
    public GameObject choicePanel;
    public GameObject choiceButtonPrefab;
    public Transform choiceContainer;

    [Header("Portrait References")]
    public GameObject portraitContainer;
    public UnityEngine.UI.Image portraitImage;

    [Header("Choice Settings")]
    public float delayBeforeChoices = 2f;

    private Dialogue currentDialogue;
    private int currentLineIndex = 0;
    private System.Action onDialogueComplete;
    private bool waitingForChoice = false;

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

    public void StartDialogue(Dialogue dialogue, System.Action onComplete = null)
    {
        Debug.Log($"Starting dialogue: {dialogue.dialogueName}");

        IsDialogueActive = true;
        currentDialogue = dialogue;
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

        if (dialogueText != null)
            dialogueText.text = line.text;

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

        Debug.Log($"Displaying line {currentLineIndex}: {line.text}");

        // Check if this is the last line and has choices
        bool isLastLine = (currentLineIndex == currentDialogue.lines.Length - 1);
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
        if (waitingForChoice)
            return;

        currentLineIndex++;
        DisplayCurrentLine();
    }

    System.Collections.IEnumerator ShowChoicesAfterDelay(Dialogue_Choice[] choices)
    {
        yield return new WaitForSeconds(delayBeforeChoices);
        ShowChoices(choices);
    }

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
            TMPro.TMP_Text buttonText = buttonObj.GetComponentInChildren<TMP_Text>();

            // If TMP not found, try standard Text
            if (buttonText == null)
            {
                TMPro.TMP_Text standardText = buttonObj.GetComponentInChildren<TMP_Text>();
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

    void OnChoiceSelected(Dialogue_Choice choice)
    {
        Debug.Log($"Choice selected: {choice.choiceText}");

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
    }

    void EndDialogue()
    {
        Debug.Log("Dialogue ended");

        IsDialogueActive = false;

        if (dialoguePanel != null)
            dialoguePanel.SetActive(false);

        if (choicePanel != null)
            choicePanel.SetActive(false);

        if (portraitContainer != null)
            portraitContainer.SetActive(false);

        currentDialogue = null;
        waitingForChoice = false;
        onDialogueComplete?.Invoke();
    }
}
