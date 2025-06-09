using System.Text;
using UnityEngine;
using UnityEngine.UI;
using TMPro;


public class IDCard : MonoBehaviour
{
    public GameObject frontPanel;
    public GameObject backPanel;

    //public TextMeshProUGUI nameText;
    //public TextMeshProUGUI genderText;

    public TextMeshProUGUI traitsListText;

    public Button flipToBackButton;
    public Button flipToFrontButton;
    

    // do we even have a script for player info? if so, use the script below
    //[SerializeField] private PlayerData playerData;
    [SerializeField] private TraitManager traitManager;

    void Start()
    {
        flipToBackButton.onClick.AddListener(ShowBack);
        flipToFrontButton.onClick.AddListener(ShowFront);
        

        ShowFront();
    }

    public void ToggleIDCardPanel()
    {
        gameObject.SetActive(!gameObject.activeSelf);

        if (gameObject.activeSelf)
        {
            // Always show front when first opening
            ShowFront();
        }
    }

    void ShowFront()
    {
        frontPanel.SetActive(true);
        backPanel.SetActive(false);
        Debug.Log("Showing Front");

        // place here the stuff like names and shit
        //nameText.text = $"Name: {playerData.playerName}";
        //genderText.text = $"Gender: {playerData.gender}";
    }

    void ShowBack()
    {
        frontPanel.SetActive(false);
        backPanel.SetActive(true);

        StringBuilder sb = new StringBuilder();
        sb.AppendLine("Traits:");

        foreach (Trait trait in traitManager.GetAllActiveTraits())
        {
            sb.AppendLine($"- {trait.traitName}");
        }

        traitsListText.text = sb.ToString();

        Debug.Log("Showing Back");
    }

    


}
