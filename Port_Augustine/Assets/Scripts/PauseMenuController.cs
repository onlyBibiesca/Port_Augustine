using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class PauseMenuController : MonoBehaviour
{
    [SerializeField] GameObject pauseMenu;
    [SerializeField] GameObject optionPanel;
    [SerializeField] GameObject helpPanel;
    [SerializeField] GameObject surePanel;
    [SerializeField] Slider soundSlider;
    [SerializeField] Slider brightnessSlider;

    private int defaultvalue = 50;
    // Start is called before the first frame update
    void Start()
    {
        soundSlider.value = defaultvalue;
        brightnessSlider.value = defaultvalue;

        optionPanel.SetActive(false);
        helpPanel.SetActive(false);
    }

    public void Update()
    {
        if (!pauseMenu.activeInHierarchy && surePanel.activeSelf)
        {
            surePanel.SetActive(false);
        }

        else if (surePanel.activeSelf && Input.GetMouseButtonDown(0))
        {
            if (!IsPointerOverUIObject(surePanel))
            {
                surePanel.SetActive(false);
            }
        }

    }

    public void Options()
    {
        optionPanel.SetActive(true);
        Debug.Log("Open OptionsMenu");
    }

    public void Help()
    {
        helpPanel.SetActive(true);
        Debug.Log("Open  Help");
    }

    public void Back()
    {
        optionPanel.SetActive(false);
        helpPanel.SetActive(false);
        Debug.Log("Back to PauseMenu");
    }

    public void QuitGame()
    {
        Application.Quit();
        Debug.Log("Quit Game");
        
    }

    public void QuitToMenu()
    {
        SceneManager.LoadScene(0);
        Debug.Log("Quit to Main Menu");
    }

    bool IsPointerOverUIObject(GameObject targetUI)
    {
        PointerEventData pointerData = new PointerEventData(EventSystem.current)
        {
            position = Input.mousePosition
        };

        var raycastResults = new System.Collections.Generic.List<RaycastResult>();
        EventSystem.current.RaycastAll(pointerData, raycastResults);

        foreach (var result in raycastResults)
        {
            if (result.gameObject == targetUI || result.gameObject.transform.IsChildOf(targetUI.transform))
                return true;
        }

        return false;
    }
}
