using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerHUD : MonoBehaviour
{
    [SerializeField] GameObject targetObject;

    public void ToggleActiveState()
    {
        if (targetObject != null)
        {
            // Reverses the current active state (!true becomes false, !false becomes true)
            bool currentState = targetObject.activeSelf;
            targetObject.SetActive(!currentState);
        }
    }
}
