using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class OfficeMinigameManager : MonoBehaviour
{
    public float GameDuration = 120f;
    private float Timer;
    private int PapersFiled = 0;
    private int PapersMaximum = 10;
    private bool isGameRunning = false;
    public GameObject ResultsScreen;
    private void OnEnable()
    {
        StartMinigame();
    }

    public void StartMinigame()
    {
        Debug.Log("shut up hold on");
    }
}
