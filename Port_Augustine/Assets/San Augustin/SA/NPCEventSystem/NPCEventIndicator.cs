using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NPCEventIndicator : MonoBehaviour
{
    public GameObject exclamationMark;
    private NPC_Dialogue npcDialogue;

    void Start()
    {
        npcDialogue = GetComponent<NPC_Dialogue>();

        if (exclamationMark != null)
            exclamationMark.SetActive(false);
    }

    public void ShowEventIndicator()
    {
        if (exclamationMark != null)
        {
            exclamationMark.SetActive(true);
            if (showDebugMessages)
                Debug.Log($"Showing event indicator for {gameObject.name}");
        }
    }

    public void HideEventIndicator()
    {
        if (exclamationMark != null)
        {
            exclamationMark.SetActive(false);
        }
    }

    bool showDebugMessages = true;
}
