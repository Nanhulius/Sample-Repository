using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using Cinemachine;
using Unity.Android.Types;

public class GameManager : MonoBehaviour
{
    [SerializeField] public TextMeshProUGUI movesText;
    [SerializeField] public TextMeshProUGUI outOfMovesText;
    [SerializeField] public TextMeshProUGUI currentWeaponText;
    [SerializeField] public TextMeshProUGUI ammoInClipText;
    [SerializeField] private GameObject compass;
    [SerializeField] private Player player;
    [SerializeField] private Weapon playerWeapon;
    [SerializeField] private Camera mainCamera;
    [SerializeField] private CinemachineVirtualCamera virtualCamera1;
    [SerializeField] private CinemachineVirtualCamera virtualCamera2;
    [SerializeField] private CinemachineVirtualCamera virtualCamera3;
    [SerializeField] private CinemachineVirtualCamera virtualCamera4;
    public Button nextTurnButton;
    public Button changeWeapon;

    private int cameraIndex = 0;


    private void Awake()
    {
        player = GameObject.FindWithTag("Player").GetComponent<Player>();
        playerWeapon = player.GetComponent<Weapon>();
        mainCamera = GameObject.FindWithTag("MainCamera").GetComponent<Camera>();
    }

    private void Update()
    {
        CheckCameraRotation();
    }
    public void EnableOutOfMovesText()                  // When player is out of moves, this UI-text appears
    {
        outOfMovesText.text = "You are out of moves!";
        outOfMovesText.gameObject.SetActive(true);
    }

    public void ButtonNextTurn()                        // Resets player moves and starts new turn
    {
        player.movesLeft = 10f;
        movesText.text = "Moves left: " + player.movesLeft;
        outOfMovesText.gameObject.SetActive(false);
    }
    public void ButtonChangeWeapon()                    // Changes weapon to next
    {
        playerWeapon.ChangeWeapon();
    }

    public void ChangeWeaponUIText()                    // Changes UItext regarding the weapon in use
    {
        currentWeaponText.text = "Weapon in use: " + playerWeapon.currentWeapon.weaponName;
        ammoInClipText.text = "Ammo in Clip " + playerWeapon.currentWeapon.currentAmmo;
    }

    public void CheckCameraRotation()   // Checks input for Camera rotation and turns compass
    {
        if (Input.GetKeyDown(KeyCode.Q)) 
        {
            cameraIndex++;
            if (cameraIndex > 3)
                cameraIndex = 0;
            SetActiveVirtualCamera();
            compass.transform.Rotate(0, 0, 90);
        }
        else if (Input.GetKeyDown(KeyCode.E))
        {
            cameraIndex--;
            if (cameraIndex < 0)
                cameraIndex = 3;
            SetActiveVirtualCamera();
            compass.transform.Rotate(0, 0, -90);
        }
    }

    public void SetActiveVirtualCamera() // Sets active VirtualCamera
    {
        switch (cameraIndex)
        {
            case 0:
                virtualCamera1.Priority = 11;
                virtualCamera2.Priority = 10;
                virtualCamera3.Priority = 10;
                virtualCamera4.Priority = 10;
                break;
            case 1:
                virtualCamera1.Priority = 10;
                virtualCamera2.Priority = 11;
                virtualCamera3.Priority = 10;
                virtualCamera4.Priority = 10;
                break;
            case 2:
                virtualCamera1.Priority = 10;
                virtualCamera2.Priority = 10;
                virtualCamera3.Priority = 11;
                virtualCamera4.Priority = 10;
                break;
            case 3:
                virtualCamera1.Priority = 10;
                virtualCamera2.Priority = 10;
                virtualCamera3.Priority = 10;
                virtualCamera4.Priority = 11;
                break;
            default:
                Debug.Log("SetActiveVirtualCamera is bugging");
                break;
        }
    }
}
