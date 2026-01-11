using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.Video;
using UnityEngine.UI;

public class CutsceneManager : MonoBehaviour
{
    public VideoPlayer videoPlayer;
    public RawImage rawImage;
    public string nextSceneName = "GameScene"; // Change to your scene

    void Start()
    {
        videoPlayer.loopPointReached += OnVideoFinished;
        videoPlayer.Play();
    }

    void Update()
    {
        // Allow skipping
        if (Input.GetKeyDown(KeyCode.Space))
        {
            videoPlayer.Stop();
            LoadNextScene();
        }
    }

    void OnVideoFinished(VideoPlayer vp)
    {
        LoadNextScene();
    }

    void LoadNextScene()
    {
        SceneManager.LoadScene(nextSceneName);
    }
}