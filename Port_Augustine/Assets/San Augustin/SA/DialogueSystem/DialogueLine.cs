using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class DialogueLine
{
    public string speakerName;
    [TextArea(3, 10)]
    public string text;

    [Header("Choices (Optional)")]
    public bool hasChoices = false;
    public Dialogue_Choice[] choices;

    [Header("Character Portrait (Optional)")]
    public Sprite characterPortrait;

}
