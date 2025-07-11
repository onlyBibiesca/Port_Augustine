using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MoodManager: MonoBehaviour
{
    public GameObject moodOverlay;

    public void SetNightLighting()
    {
        if (moodOverlay != null)
        {
            moodOverlay.SetActive(true);
            Debug.Log("Night lighting activated.");
        }
    }
}