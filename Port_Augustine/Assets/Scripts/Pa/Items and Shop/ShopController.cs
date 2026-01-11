using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ShopController : MonoBehaviour
{
    public static ShopController Instance;

    public GameObject shopPanel;
    private System.Action onCloseCallback;

    private void Awake()
    {
        Instance = this;
    }

    public void OpenShop(System.Action callback = null)
    {
        onCloseCallback = callback;
        shopPanel.SetActive(true);
        Time.timeScale = 0;
    }

    public void CloseShop()
    {
        shopPanel.SetActive(false);
        Time.timeScale = 1;

        if (onCloseCallback != null)
        {
            onCloseCallback.Invoke();
            onCloseCallback = null;
        }
    }
}