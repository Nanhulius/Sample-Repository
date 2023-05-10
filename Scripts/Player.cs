using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UIElements;
using Unity.Collections;
using Cinemachine;

public class Player : MonoBehaviour
{
    [SerializeField] public Camera mainCamera;
    [SerializeField] public CameraController cameraController;

    private GameManager gameManager;
    private Weapon weapon;
    private UnitSelector unitSelector;
   
    [SerializeField] private float jumpHeight, moveSpeed;
    [SerializeField] public float maxMoves, movesLeft; 

    private TextMeshProUGUI movesText;
    private bool moving = false;
    private bool outOfMovesShown = false;
    private Rigidbody rb;

    private void Awake()
    {
        movesLeft = maxMoves;
        rb = GetComponent<Rigidbody>();
        mainCamera = gameObject.GetComponentInChildren<Camera>();
        cameraController = gameObject.GetComponentInChildren<CameraController>();
        gameManager = GameObject.FindWithTag("GameManager").GetComponent<GameManager>();
        weapon = gameObject.GetComponentInChildren<Weapon>();
        unitSelector = GameObject.FindWithTag("GameManager").GetComponent<UnitSelector>();
        InitializeCharacter();
    }

    private void Update()
    {
        if (!gameManager.enemyTurn) // If it's enemy turn player cannot move
            if (this == gameManager.activeCharacter)
                MoveCharacter();
            else
                return;
    }

    private void MoveCharacter()  // Moves active character, if aiming is on movemenet speed is 1/3 of normal
    {
        if (movesLeft >= 0)
        {
            var dir = new Vector3(Input.GetAxis("Horizontal"), 0f, Input.GetAxis("Vertical"));
            
            if (!weapon.aimOn)
                transform.Translate(dir * moveSpeed * Time.deltaTime);
            else
                transform.Translate(dir * (moveSpeed / 3) * Time.deltaTime);

            if (rb.velocity.magnitude != 0)
            {
                moving = true;
                movesLeft -= Time.deltaTime;
                
            }    
            else
                moving = false;

            if (Input.GetKeyDown(KeyCode.Space))
            {
                rb.AddForce(new Vector3(0, jumpHeight), ForceMode.Impulse);
                movesLeft--;
                movesLeft -= Time.deltaTime;
            }
            UpdateUIText();
        }
        else
        {
            gameManager.announcerText.text = "Out of moves!";
            if (!outOfMovesShown) // If moves are depleted show text
            {
                gameManager.EnableAnnouncerText();
                outOfMovesShown = true;
            }
            else                  // If clicking any key while moves are depleted show text
            {
                if (Input.anyKey)
                    gameManager.EnableAnnouncerText();
            }
        }
    }

    public void UpdateUIText()
    {
        gameManager.movesText.text = "Moves left: " + movesLeft.ToString("F2");
    }

    private void InitializeCharacter() // Sets active character at the start of the game
    {
        unitSelector.selectedCharacterList.Add(this);
        if (gameManager.activeCharacter == null)
        {
            gameManager.activeCharacter = this;
            gameManager.activeCharacterWeapon = gameManager.activeCharacter.GetComponent<Weapon>();
        }
        else
        {
            cameraController.gameObject.SetActive(false);
        }
    }
    
    public void CharacterChange() // Changes UI to active character when changing character
    {
        UpdateUIText();
        gameManager.ChangeWeaponUIText();       
    }
}