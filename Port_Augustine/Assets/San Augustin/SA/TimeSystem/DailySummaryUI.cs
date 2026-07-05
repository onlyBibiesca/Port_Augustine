using System.Collections.Generic;
using System.Text;
using TMPro;
using UnityEngine;

public class DailySummaryUI : MonoBehaviour
{
    public static DailySummaryUI Instance;

    [Header("UI")]
    [SerializeField] private GameObject summaryPanel;
    [SerializeField] private TMP_Text summaryText;

    private void Awake()
    {
        if (Instance == null)
            Instance = this;
        else
            Destroy(gameObject);

        if (summaryPanel != null)
            summaryPanel.SetActive(false);
    }

    public void ShowSummary()
    {
        if (DailySummaryManager.Instance == null)
            return;

        Dictionary<string, int> relationships =
            DailySummaryManager.Instance.GetRelationshipChanges();

        int money =
            DailySummaryManager.Instance.GetMoneyEarnedToday();

        StringBuilder builder = new StringBuilder();

        builder.AppendLine("Day Summary");
        builder.AppendLine();
        builder.AppendLine("<b>Relationships</b>");

        if (relationships.Count == 0)
        {
            builder.AppendLine("No relationship changes.");
        }
        else
        {
            foreach (KeyValuePair<string, int> pair in relationships)
            {
                string sign = pair.Value >= 0 ? "+" : "";
                builder.AppendLine($"{pair.Key}: {sign}{pair.Value}");
            }
        }

        builder.AppendLine();
        builder.AppendLine("<b>Money Earned</b>");
        builder.AppendLine($"{money}");

        summaryText.text = builder.ToString();

        summaryPanel.SetActive(true);

        Time.timeScale = 0f;
    }

    public void CloseSummary()
    {
        summaryPanel.SetActive(false);

        Time.timeScale = 1f;

        DailySummaryManager.Instance.ResetDailyData();
    }

    
    
}