using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UIButtonScripts : MonoBehaviour
{
    [SerializeField] private Button ChangeWeaponButton, AimButton, ChangeCharacterButton, EndTurnButton;
    [SerializeField] private Weapon playerWeapon;

    private UnitSelector unitSelector;
    private TurnManager turnManager;

    private void Start()
    {
        unitSelector = GameObject.FindWithTag("GameManager").GetComponent<UnitSelector>();
        turnManager = GameObject.FindWithTag("GameManager").GetComponent<TurnManager>();
        playerWeapon = GameObject.FindWithTag("Player").GetComponent<Weapon>();
        ChangeWeaponButton.GetComponent<Button>().onClick.AddListener(() => playerWeapon.ChangeWeapon());
        AimButton.GetComponent<Button>().onClick.AddListener(() => playerWeapon.SetAimOnOff());
        EndTurnButton.GetComponent<Button>().onClick.AddListener(() => turnManager.ChangeTurn());
        ChangeCharacterButton.GetComponent<Button>().onClick.AddListener(() => ChangeOnClickListeners()); 
    }

    private void ChangeOnClickListeners()  //Changes button listeners to listen active characters weapon
    {
        ChangeWeaponButton.onClick.RemoveAllListeners();
        AimButton.onClick.RemoveAllListeners();
        unitSelector.ChangeActiveCharacter();
        playerWeapon = unitSelector.selectedCharacterWeapon.gameObject.GetComponent<Weapon>();
        ChangeWeaponButton.GetComponent<Button>().onClick.AddListener(() => unitSelector.selectedCharacterWeapon.ChangeWeapon());
        AimButton.GetComponent<Button>().onClick.AddListener(() => unitSelector.selectedCharacterWeapon.SetAimOnOff());
    }    
}