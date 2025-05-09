using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class MainMenu : MonoBehaviour
{
    public GameObject Menu;

    public void StartPlaying()
    {
        SceneManager.LoadScene(1);
        Debug.Log("Playing...");
    }

    public void OptionMenu()
    {
        Menu.SetActive(true);
        Debug.Log("Options...");
    }

    public void BackOption()
    {
        Menu.SetActive(false);
        Debug.Log("Backing out...");
    }

    public void QuitGame()
    {
        Application.Quit();
        Debug.Log("Quitting...");
    }
}
