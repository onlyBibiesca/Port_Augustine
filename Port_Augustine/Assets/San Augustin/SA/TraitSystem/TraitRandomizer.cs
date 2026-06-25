using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TraitRandomizer : MonoBehaviour
{
    public List<TraitSO> originPool;
    public List<TraitSO> randomTraitPool;

    public int randomTraitCount = 2;

    private void Start()
    {
        GenerateOrigin();
        GenerateRandomTraits();
    }

    void GenerateOrigin()
    {
        if (originPool.Count == 0)
            return;

        int index = UnityEngine.Random.Range(0, originPool.Count);

        TraitSO chosenOrigin = originPool[index];

        TraitsManager.Instance.AddTrait(chosenOrigin);

        Debug.Log($"Origin Chosen: {chosenOrigin.traitName} | ID: {chosenOrigin.GetInstanceID()}");
    }

    void GenerateRandomTraits()
    {
        List<TraitSO> pool =
            new List<TraitSO>(randomTraitPool);

        for (int i = 0; i < randomTraitCount; i++)
        {
            if (pool.Count == 0)
                break;

            int index = UnityEngine.Random.Range(0, pool.Count);

            TraitSO chosenTrait = pool[index];

            TraitsManager.Instance.AddTrait(chosenTrait);

            pool.RemoveAt(index);
        }
    }
}