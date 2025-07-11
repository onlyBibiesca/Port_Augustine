using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class PaperSpawner : MonoBehaviour
{
    public GameObject[] paperPrefabs;
    public Transform spawnPoint;
    public Transform ParentObject;

    private void Start()
    {
        if (ParentObject == null)
            Debug.LogWarning("ParentObject is not assigned. Check the inspector.");
    }

    public void SpawnPaper()
    {
        int randomIndex = Random.Range(0, paperPrefabs.Length);
        GameObject paper = Instantiate(paperPrefabs[randomIndex], spawnPoint.position, Quaternion.identity, ParentObject);
        Debug.Log("Paper spawned.");

        if (ParentObject != null)
        {
            paper.transform.SetParent(ParentObject.transform, true);
            Debug.Log($"spawned {paper.name} under {ParentObject.name}.");
        }
        else
        {
            Debug.Log("Okay, it's not parented to the assigned thing. Check inspector.");
        }

        //we slap the TEXT_SCORE tmp inot the paper's drag script.
        DragAndDropper dragScript = paper.GetComponent<DragAndDropper>();
        if (dragScript != null)
        {
            GameObject scoreObj = GameObject.Find("TEXT_SCORE");
            if (scoreObj != null)
            {
                TextMeshProUGUI scoreText = scoreObj.GetComponent<TextMeshProUGUI>();
                if (scoreText != null)
                {
                    dragScript.scoreText = scoreText;
                    Debug.Log("Score text assigned to paper successfully!");
                }
                else
                {
                    Debug.LogWarning("TEXT_SCORE does not have a TextMeshProUGUI component.");
                }
            }
            else
            {
                Debug.LogWarning("TEXT_SCORE GameObject not found in scene.");
            }
        }
        else
        {
            Debug.LogWarning("DragAndDrop script not found on spawned paper.");
        }
    }
}
