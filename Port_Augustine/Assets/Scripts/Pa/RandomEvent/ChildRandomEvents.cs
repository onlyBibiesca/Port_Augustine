using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ChildRandomEvents : MonoBehaviour
{
    //this script collects the triggers that are under the parent script
    private RandomEventManager randomEventManager;
    // Start is called before the first frame update
    void Start()
    {
        randomEventManager = transform.parent.GetComponent<RandomEventManager>();
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        randomEventManager.TriggeredColliders(collision);
    }
}
