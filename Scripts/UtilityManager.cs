using System.Collections;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using TMPro.Examples;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class UtilityManager : MonoBehaviour
{
    private GameManager gm;
    private Button selectedButton;
    private List<Button> utilityButtons = new List<Button>();

    public GameObject buttonPrefab;
    public Transform buttonContainer;

    [HideInInspector] public int currentSlotCount;

    private void Awake()
    {
        gm = GameObject.FindWithTag("GameManager").GetComponent<GameManager>();
    }

    private void Start()
    {
        InitializeUtilityButtons();

        if (utilityButtons.Count > 0)
            SetButtonAlpha(utilityButtons[0]);
    }

    public void InitializeUtilityButtons()
    {
        ClearSlots();

        if (gm.player.playerData.hasAnnihilatorMissile)
            CreateUtilityButton(UtilityType.AnnihilatorMissile);

        if (gm.player.playerData.hasDeflector)
            CreateUtilityButton(UtilityType.Deflector);

        if (gm.player.playerData.hasEngineBooster)
            CreateUtilityButton(UtilityType.EngineBooster);

        if (gm.player.playerData.hasScanner)
            CreateUtilityButton(UtilityType.Scanner);
    }

    public void ToggleButton(UtilityType utilityType, bool isEnabled)
    {
        Button buttonToggle = utilityButtons.Find(button => 
            button.GetComponentInChildren<TextMeshProUGUI>().text == utilityType.ToString());

        if (buttonToggle != null)
        {
            buttonToggle.interactable = isEnabled;
            if (!isEnabled)
                SetNextActiveUtility();
        }
    }

    private void SetNextActiveUtility()
    {
        foreach (Button button in utilityButtons)
        {
            if (button.interactable)
            {
                UtilityType nextUtility = (UtilityType)System.Enum.Parse(typeof(UtilityType),
                    button.GetComponentInChildren<TextMeshProUGUI>().text);
                gm.player.utilityController.activeUtility = nextUtility;
                break;
            }
        }
    }

    /*

    public void AddNewSlot()
    {
        GameObject newButton = Instantiate(buttonPrefab, buttonContainer);
        Button buttonComponent = newButton.GetComponent<Button>();

        int slotIndex = utilityButtons.Count;

        buttonComponent.onClick.AddListener(() => OnUtilityButtonClick(slotIndex));
        utilityButtons.Add(buttonComponent);
        currentSlotCount++;
        buttonContainer.transform.position = new Vector3 (buttonContainer.transform.position.x + 1, buttonContainer.transform.position.y);
    }*/

    private void ClearSlots()
    {
        foreach (Button button in utilityButtons)
            Destroy(button.gameObject);

        utilityButtons.Clear();
    }

    private void CreateUtilityButton(UtilityType utilityType)
    {
        GameObject newButton = Instantiate(buttonPrefab, buttonContainer);
        Button buttonComponent = newButton.GetComponent<Button>();

        TextMeshProUGUI buttonText = newButton.GetComponentInChildren<TextMeshProUGUI>();
        if (buttonText != null)
            buttonText.text = utilityType.ToString();

        buttonComponent.onClick.AddListener(() => OnUtilityButtonClick(utilityType, buttonComponent));
        utilityButtons.Add(buttonComponent);

        EventTrigger trigger = newButton.AddComponent<EventTrigger>();
        EventTrigger.Entry pointerEnter = new EventTrigger.Entry();
        pointerEnter.eventID = EventTriggerType.PointerEnter;
        pointerEnter.callback.AddListener((eventData) => { gm.player.utilityController.isChoosingUtility = true; });
        trigger.triggers.Add(pointerEnter);

        EventTrigger.Entry pointerExit = new EventTrigger.Entry();
        pointerExit.eventID = EventTriggerType.PointerExit;
        pointerExit.callback.AddListener((eventData) => { gm.player.utilityController.isChoosingUtility = false; });
        trigger.triggers.Add(pointerExit);

        currentSlotCount++;
    }

    private void OnUtilityButtonClick(UtilityType selectedUtility, Button selectedButton)
    {
        Debug.Log($"Utility {selectedUtility} selected");
        gm.player.utilityController.activeUtility = selectedUtility;

        selectedButton = utilityButtons.FirstOrDefault(b => b.GetComponentInChildren<TextMeshProUGUI>().text == selectedUtility.ToString());
        SetButtonAlpha(selectedButton);
    }

    public void SelectUtilityByIndex(int index)
    {
        if (index >= 0 && index < utilityButtons.Count)
        {
            Button selectedButton = utilityButtons[index];
            UtilityType selectedUtility = (UtilityType)System.Enum.Parse(typeof(UtilityType),
                selectedButton.GetComponentInChildren<TextMeshProUGUI>().text);

            gm.player.utilityController.activeUtility = selectedUtility;
            SetButtonAlpha(selectedButton);
        }
    }

    public void UpgradeSlotCount()
    {
        int newSlotCount = currentSlotCount + 1;
        //InitializeSlots(newSlotCount);
    }

    private void SetButtonAlpha(Button selectedButton)
    {
        foreach (Button button in utilityButtons)
        {
            var colors = button.colors;

            if (button == selectedButton)
                colors.normalColor = new Color(colors.normalColor.r, colors.normalColor.g, colors.normalColor.b, 1f);
            else
            {
                colors.normalColor = new Color(colors.normalColor.r, colors.normalColor.g, colors.normalColor.b, 0.6f);
                colors.highlightedColor = new Color(colors.highlightedColor.r, colors.highlightedColor.g, colors.highlightedColor.b, 1f);
            }
            
            button.colors = colors;
        }
    }
}