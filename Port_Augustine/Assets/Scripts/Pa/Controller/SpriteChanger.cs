using UnityEngine;

public class ToggleVisibility : MonoBehaviour
{
    public GameObject objectA;
    public GameObject objectB;

    private bool showingA = true;

    // Call this function from a UI Button's OnClick event
    public void Toggle()
    {
        if (objectA == null || objectB == null)
        {
            Debug.LogWarning("ToggleVisibility: One or both objects are not assigned.");
            return;
        }

        showingA = !showingA;
        objectA.SetActive(showingA);
        objectB.SetActive(!showingA);
    }
}
