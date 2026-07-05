using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class GameEndManager : MonoBehaviour
{
    public static GameEndManager Instance;

    [Header("Day 7 Settings")]
    public int endGameDay = 7;

    [Header("UI References")]
    public GameObject gameEndCanvas;
    public TextMeshProUGUI endGameTitle;
    public TextMeshProUGUI endGameMessage;

    [Header("Debug")]
    public bool showDebugMessages = true;

    private bool gameEnded = false;

    void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }
        else
        {
            Destroy(gameObject);
        }
    }

    void Start()
    {
        // Hide canvas at start
        if (gameEndCanvas != null)
            gameEndCanvas.SetActive(false);

        // Subscribe to day changes
        if (TimeSystem.Instance != null)
        {
            TimeSystem.Instance.OnDayChanged += CheckForGameEnd;
        }
    }

    void OnDestroy()
    {
        if (TimeSystem.Instance != null)
        {
            TimeSystem.Instance.OnDayChanged -= CheckForGameEnd;
        }
    }

    void CheckForGameEnd(int currentDay)
    {
        // Check if reached the end game day
        if (currentDay > endGameDay && !gameEnded)
        {
            EndGame();
        }
    }

    public void EndGame()
    {
        gameEnded = true;

        if (showDebugMessages)
            Debug.Log($"Game ended! Player reached the end of Day {endGameDay}");

        // Show end game canvas
        if (gameEndCanvas != null)
        {
            gameEndCanvas.SetActive(true);
        }
        else
        {
            Debug.LogWarning("Game End Canvas not assigned!");
        }

        // Stop the game
        Time.timeScale = 0f;

        if (showDebugMessages)
            Debug.Log("Game paused (Time.timeScale = 0)");
    }

    public void RestartGame()
    {
        // Resume game
        Time.timeScale = 1f;

        // Reload scene
        UnityEngine.SceneManagement.SceneManager.LoadScene(
            UnityEngine.SceneManagement.SceneManager.GetActiveScene().name
        );
    }

    public void QuitGame()
    {
        // Resume game first
        Time.timeScale = 1f;

    }

    public bool HasGameEnded()
    {
        return gameEnded;
    }
}
