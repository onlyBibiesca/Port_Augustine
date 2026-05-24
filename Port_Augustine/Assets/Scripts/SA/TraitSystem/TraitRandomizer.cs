using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TraitRandomizer : MonoBehaviour
{
    public List<TraitSO> possibleStartingTraits;
    public int traitsToGive = 2;

    void Start()
    {
        GenerateTraits();
    }

    void GenerateTraits()
    {
        List<TraitSO> pool =
            new List<TraitSO>(possibleStartingTraits);

        for (int i = 0; i < traitsToGive; i++)
        {
            if (pool.Count == 0)
                return;

            int randomIndex = UnityEngine.Random.Range(0, pool.Count);

            TraitSO selectedTrait = pool[randomIndex];

            TraitsManager.Instance.AddTrait(selectedTrait);

            pool.RemoveAt(randomIndex);
        }
    }
}