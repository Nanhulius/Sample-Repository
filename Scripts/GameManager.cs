using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using Cinemachine;
using UnityEngine.TextCore.Text;

public class GameManager : MonoBehaviour
{
    [SerializeField] public TextMeshProUGUI movesText;
    [SerializeField] public TextMeshProUGUI announcerText;
    [SerializeField] public TextMeshProUGUI currentWeaponText;
    [SerializeField] public TextMeshProUGUI ammoInClipText;

    [SerializeField] public Player activeCharacter;
    [SerializeField] public Weapon activeCharacterWeapon;
    [SerializeField] private Camera mainCamera;

    private UnitSelector unitSelector;

    private Color oldColor = new (0, 0, 0, 1),
                  newColor = new (0, 0, 0, 0); 

    public Button nextTurnButton;
    public Button changeWeapon;

    public int spawnedCharacterIndex = 0;
    [HideInInspector] public bool enemyTurn = false;

    public void Awake()
    {
        
        unitSelector = this.GetComponent<UnitSelector>();   
    }

    public void EnableAnnouncerText()                  // When player is out of moves, this UI-text appears
    {
        StartCoroutine(AnnouncerTimer(1.5f));
    }

    public void ButtonNextTurn()                        // Resets player/enemy moves and starts new turn
    {
        if (!enemyTurn)
        {
            foreach (Player p in unitSelector.selectedCharacterList)
                p.movesLeft = p.maxMoves;
        }
        else
            foreach (Enemy e in unitSelector.enemyList)
                e.movesLeft = e.maxMoves;

        movesText.text = "Moves left: " + activeCharacter.movesLeft;
    }
    public void ButtonChangeWeapon()                    // Changes weapon to next
    {
        activeCharacterWeapon.ChangeWeapon();
    }
    public void ChangeWeaponUIText()                    // Changes UItext regarding the weapon in use
    {
        currentWeaponText.text = "Weapon in use: " + activeCharacterWeapon.currentWeapon.weaponName;
        ammoInClipText.text = "Ammo in Clip " + activeCharacterWeapon.currentWeapon.currentAmmo;
    }
    public IEnumerator AnnouncerTimer(float timer) // Timer for EnableOutOfMovesText()
    {
        announcerText.gameObject.SetActive(true);
        float waitingTime = 0;

        while (waitingTime < timer)
        {
            waitingTime += Time.deltaTime;
            announcerText.color = Color.Lerp(oldColor, newColor, waitingTime / timer);
            yield return null;
        }
        announcerText.gameObject.SetActive(false);
        movesText.color = oldColor;
        StopCoroutine(AnnouncerTimer(timer));
    }  
}