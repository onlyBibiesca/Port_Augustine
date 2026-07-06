using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CharacterPanelController : MonoBehaviour
{
    [Header("Pages")]
    [SerializeField] private GameObject questPage;
    [SerializeField] private GameObject traitsPage;
    [SerializeField] private GameObject relationshipsPage;

    [Header("Settings")]
    [SerializeField] private bool openOnQuestPage = true;

    private void Start()
    {
        if (openOnQuestPage)
            ShowQuestPage();
        else
            HideAllPages();
    }

    private void HideAllPages()
    {
        if (questPage != null)
            questPage.SetActive(false);

        if (traitsPage != null)
            traitsPage.SetActive(false);

        if (relationshipsPage != null)
            relationshipsPage.SetActive(false);
    }

    private void SetActivePage(GameObject page)
    {
        HideAllPages();

        if (page != null)
            page.SetActive(true);
    }

    public void ShowQuestPage()
    {
        SetActivePage(questPage);
    }

    public void ShowTraitsPage()
    {
        SetActivePage(traitsPage);
    }

    public void ShowRelationshipsPage()
    {
        SetActivePage(relationshipsPage);
    }
}

