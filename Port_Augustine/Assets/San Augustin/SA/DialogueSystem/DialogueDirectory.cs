using System.Collections;
using System.Collections.Generic;
using UnityEngine;


[CreateAssetMenu(fileName = "Dialogue Directory", menuName = "Dialogue System/Dialogue Directory")]
public class DialogueDirectory : ScriptableObject
{
    public List<Dialogue> dialogues = new List<Dialogue>();

    public Dialogue GetDialogueByName(string name)
    {
        return dialogues.Find(d => d.dialogueName == name);
    }

    public Dialogue GetDialogueByIndex(int index)
    {
        if (index >= 0 && index < dialogues.Count)
            return dialogues[index];
        return null;
    }
}

