using System.Collections;
using UnityEngine;
using TMPro;

public class OfficeMinigameManager : MonoBehaviour
{
    public float GameDuration = 120f;
    private float CurrentTime;

    public int PapersMaximum = 10;
    private int PapersRemaining;
    private bool hasGameEnded = false;

    public TextMeshProUGUI TimerText;
    public TextMeshProUGUI PapersLeftText;
    public TextMeshProUGUI TextScore;

    public GameObject MainMinigame;
    public GameObject ResultsScreen;
    public GameObject PapersParent;
    public GameObject Stats;

    [Header("Results UI")]
    public TextMeshProUGUI ResultScoreText;
    public TextMeshProUGUI ResultTimeLeftText;
    public TextMeshProUGUI ResultTotalText;
    public TextMeshProUGUI ResultLabelText;
    public TextMeshProUGUI ResultMoneyText;

    [Header("Wallet")]
    public TextMeshProUGUI PlayerMoneyText;

    [Header("Reference to PaperSpawner")]
    public PaperSpawner paperSpawner;


    private void OnEnable()
    {
        UnityEngine.Debug.Log("Panel appeared, enabling time ");
        StartMinigame();
    }

    private void OnDisable()
    {
        UnityEngine.Debug.Log("Disabling time freeze, Minigame Panel has been disabled");
        Time.timeScale = 1.0f;

        //GET RIDDA PAPUHS
        if (PapersParent != null)
        {
            foreach (Transform child in PapersParent.transform)
            {
                Destroy(child.gameObject);
            }
        }
    }

    public void StartMinigame()
    {
        UnityEngine.Debug.Log("Minigame started!");
        Time.timeScale = 0.0f;

        if (Stats != null)
            Stats.SetActive(false);

        CurrentTime = GameDuration;
        PapersRemaining = PapersMaximum;
        hasGameEnded = false;

        UpdatePapersUI();

        if (ResultsScreen != null)
            ResultsScreen.SetActive(false);
    }

    private void Update()
    {
        if (hasGameEnded) return;

        CurrentTime -= Time.unscaledDeltaTime;
        CurrentTime = Mathf.Clamp(CurrentTime, 0, GameDuration);
        UpdateTimerUI();

        if (ShouldEndGame())
        {
            EndGame();
        }
    }

    public void OnPaperSpawned()
    {
        if (PapersRemaining > 0)
        {
            PapersRemaining--;
            UpdatePapersUI();
        }
    }

    private void UpdatePapersUI()
    {
        if (PapersLeftText != null)
            PapersLeftText.text = PapersRemaining.ToString();
    }

    private void UpdateTimerUI()
    {
        if (TimerText != null)
        {
            int seconds = Mathf.CeilToInt(CurrentTime);
            TimerText.text = seconds.ToString();
        }
    }

    private bool ShouldEndGame()
    {
        return CurrentTime <= 0f;
    }

    public void FinishShift()
    {
        if (!hasGameEnded)
        {
            EndGame();
        }
    }

    private void EndGame()
    {
        hasGameEnded = true;
        UnityEngine.Debug.Log("Game done, let's do the results.");

        if (ResultsScreen != null)
            ResultsScreen.SetActive(true);

        //Setting the score stuff.
        int score = 0;
        if (TextScore != null && int.TryParse(TextScore.text, out var parsedScore))
        {
            score = parsedScore;
        }
        else
        {
            UnityEngine.Debug.LogWarning("Couldn't parse TEXT_SCORE!");
        }

        int timeLeft = GetRemainingTimeBonus();
        int total = score + timeLeft;

        if (ResultScoreText) ResultScoreText.text = score.ToString();
        if (ResultTimeLeftText) ResultTimeLeftText.text = timeLeft.ToString();
        if (ResultTotalText) ResultTotalText.text = total.ToString();

        //Grading system
        string resultLabel;
        if (total >= 200)
            resultLabel = "[S]uperb!";
        else if (total >= 180)
            resultLabel = "[A]mazing!";
        else if (total >= 160)
            resultLabel = "[B]etter!";
        else if (total >= 140)
            resultLabel = "[C]ompetent";
        else
            resultLabel = "[D]ecent";

        if (ResultLabelText) ResultLabelText.text = resultLabel;

        //MONEY REWARD BASED ON TOTAL
        int moneyEarned;
        if (total >= 200) //S
            moneyEarned = 100;

        else if (total >= 180) //A
            moneyEarned = 90;

        else if (total >= 160) //B
            moneyEarned = 85;

        else if (total >= 140) //C
            moneyEarned = 80;

        else //D
            moneyEarned = 75;

        if (ResultMoneyText) ResultMoneyText.text = moneyEarned.ToString();

        //M O N E Y
        if (PlayerMoneyText != null)
        {
            int currentMoney;
            if (int.TryParse(System.Text.RegularExpressions.Regex.Replace(PlayerMoneyText.text, "[^0-9]", ""), out currentMoney) //WE IGNORE THE $ SIGN
)
            {
                currentMoney += moneyEarned;
                PlayerMoneyText.text = "$ " + currentMoney.ToString();
            }
            else
            {
                UnityEngine.Debug.LogWarning("Could not parse current money from PlayerMoneyText!");
                PlayerMoneyText.text = moneyEarned.ToString(); // fallback
            }
        }
    }

    public void ResetMinigame()
    {
        //VALUE RESET
        CurrentTime = GameDuration;
        PapersRemaining = PapersMaximum;
        hasGameEnded = false;

        //RESET VALUES
        TextScore.text = "0";
        PapersLeftText.text = "10";
        TimerText.text = "120";

        if (TextScore != null)
            TextScore.text = "0";

        //RESET RESULTS
        if (ResultScoreText != null) ResultScoreText.text = "0";
        if (ResultTimeLeftText != null) ResultTimeLeftText.text = "0";
        if (ResultTotalText != null) ResultTotalText.text = "0";
        if (ResultLabelText != null) ResultLabelText.text = "";
        if (ResultMoneyText != null) ResultMoneyText.text = "";

        //HIDE THE RESULTS SCREEN AND THE MINIGAME
        //BUT ALSO GET THE STATS BACK UP
        if (ResultsScreen != null)
            ResultsScreen.SetActive(false);
        if (MainMinigame != null)
            MainMinigame.SetActive(false);
        if (Stats != null)
            Stats.SetActive(true);

        //STATS BACK UP
        if (Stats != null)
            Stats.SetActive(true);

        //PAPER SPAWNER CURRENT PAPERS BACK TO 10
        if (paperSpawner != null)
        {
            paperSpawner.currentPapers = paperSpawner.maxPapers;
            paperSpawner.UpdatePaperCounter();
        }


        UnityEngine.Debug.Log("Minigame has been reset.");
    }


    public int GetRemainingTimeBonus()
    {
        return Mathf.CeilToInt(CurrentTime);
    }
}
