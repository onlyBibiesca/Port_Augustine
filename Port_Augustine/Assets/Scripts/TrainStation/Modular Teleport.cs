using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class ModularTeleportSystem : MonoBehaviour
{
    [Header("UI and Fade")]
    public Image blackOverlay;
    public float fadeDuration = 0.5f;

    [Header("Optional Scripts To Disable")]
    public MonoBehaviour[] scriptsToDisable;

    private GameObject currentTeleporter;
    private bool isTeleporting = false;

    void Update()
    {
        /*if (Input.GetKeyDown(KeyCode.E) && currentTeleporter != null && !isTeleporting)
        {
            //Transform destination = currentTeleporter.GetComponent<Teleporters>().GetDestination();
            if (destination != null)
            {
                StartCoroutine(TeleportPlayer(destination.position));
            }
        }*/
    }

    private IEnumerator TeleportPlayer(Vector3 targetPosition)
    {
        isTeleporting = true;

        // Disable optional scripts
        foreach (var script in scriptsToDisable)
            if (script != null) script.enabled = false;

        // Fade to black
        yield return StartCoroutine(FadeOverlay(0f, 1f));

        // Move player
        transform.position = targetPosition;

        yield return new WaitForSeconds(0.1f); // brief pause

        // Fade back in
        yield return StartCoroutine(FadeOverlay(1f, 0f));

        // Re-enable scripts
        foreach (var script in scriptsToDisable)
            if (script != null) script.enabled = true;

        isTeleporting = false;
    }

    private IEnumerator FadeOverlay(float from, float to)
    {
        float timer = 0f;
        Color color = blackOverlay.color;

        while (timer < fadeDuration)
        {
            float alpha = Mathf.Lerp(from, to, timer / fadeDuration);
            blackOverlay.color = new Color(color.r, color.g, color.b, alpha);
            timer += Time.deltaTime;
            yield return null;
        }

        blackOverlay.color = new Color(color.r, color.g, color.b, to);
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Teleporter"))
        {
            currentTeleporter = collision.gameObject;
        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.CompareTag("Teleporter") && collision.gameObject == currentTeleporter)
        {
            currentTeleporter = null;
        }
    }
}
