using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ButtonActions : MonoBehaviour
{
    [SerializeField] private RectTransform sidePanel;
    [SerializeField] public float moveAmount = 54.6f;
    [SerializeField] private float tweenDuration = 0.5f;

    private bool isPanelVisible = true;

    public void ToggleSidePanel()
    {
        isPanelVisible = !isPanelVisible;

        
        float direction = isPanelVisible ? -moveAmount : moveAmount;
        float targetX = sidePanel.localPosition.x + direction;

        LeanTween.moveLocalX(sidePanel.gameObject, targetX, tweenDuration)
            .setEase(LeanTweenType.easeInOutSine);
    }
}