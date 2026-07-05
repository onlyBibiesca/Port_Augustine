using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DailySummaryManager : MonoBehaviour
{
    public static DailySummaryManager Instance;

    private Dictionary<string, int> relationshipChanges = new();
    private int moneyEarnedToday;

    private void Awake()
    {
        Instance = this;
    }

    public void RecordRelationshipChange(string npc, int amount)
    {
        if (relationshipChanges.ContainsKey(npc))
            relationshipChanges[npc] += amount;
        else
            relationshipChanges.Add(npc, amount);
    }

    public void RecordMoneyEarned(int amount)
    {
        if (amount > 0)
            moneyEarnedToday += amount;
    }

    public Dictionary<string, int> GetRelationshipChanges()
    {
        return relationshipChanges;
    }

    public int GetMoneyEarnedToday()
    {
        return moneyEarnedToday;
    }

    public void ResetDailyData()
    {
        relationshipChanges.Clear();
        moneyEarnedToday = 0;
    }
}