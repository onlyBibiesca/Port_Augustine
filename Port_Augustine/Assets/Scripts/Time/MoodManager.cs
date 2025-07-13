using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MoodManager : MonoBehaviour
{
    [Header("Lighting")]
    public Light directionalLight;
    public Color morningColor = new Color(1f, 0.95f, 0.85f);
    public Color afternoonColor = new Color(1f, 0.85f, 0.7f);
    public Color nightColor = new Color(0.2f, 0.2f, 0.4f);

    [Header("Audio")]
    public AudioSource ambientAudio;
    public AudioClip morningClip;
    public AudioClip afternoonClip;
    public AudioClip nightClip;

    public void SetMorningMood()
    {
        Debug.Log("[MoodManager] Setting Morning Mood");
        if (directionalLight != null) directionalLight.color = morningColor;
        if (ambientAudio != null && morningClip != null)
        {
            ambientAudio.clip = morningClip;
            ambientAudio.Play();
        }
    }

    public void SetAfternoonMood()
    {
        Debug.Log("[MoodManager] Setting Afternoon Mood");
        if (directionalLight != null) directionalLight.color = afternoonColor;
        if (ambientAudio != null && afternoonClip != null)
        {
            ambientAudio.clip = afternoonClip;
            ambientAudio.Play();
        }
    }

    public void SetNightMood()
    {
        Debug.Log("[MoodManager] Setting Night Mood");
        if (directionalLight != null) directionalLight.color = nightColor;
        if (ambientAudio != null && nightClip != null)
        {
            ambientAudio.clip = nightClip;
            ambientAudio.Play();
        }
    }
}