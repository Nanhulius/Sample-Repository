using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class UnitSelector : MonoBehaviour
{
    [SerializeField] public List<Player> selectedCharacterList = new List<Player>();
    [SerializeField] public List<Enemy> enemyList = new List<Enemy>();

    [SerializeField] private Camera mainCamera;
    [SerializeField] private Player selectedCharacter;
    [SerializeField] public Weapon selectedCharacterWeapon;

    private GameManager gameManager;

    private Vector3 moveToVector;
    private Vector3 startMoveVector;
    public int activeCharacterIndex;

    private void Start()
    {
        mainCamera = GameObject.FindWithTag("PlayerCamera").GetComponent<Camera>();
        gameManager = GameObject.FindWithTag("GameManager").GetComponent<GameManager>();
        selectedCharacter = gameManager.activeCharacter;
        selectedCharacter = selectedCharacterList[0];
        activeCharacterIndex = selectedCharacterList.IndexOf(selectedCharacter);
        selectedCharacterWeapon = selectedCharacter.GetComponent<Weapon>();
    }
    private void Update()
    {
        //CheckSelectedCharacter();
    }

    private void CheckSelectedCharacter()
    {
        Ray ray = mainCamera.ScreenPointToRay(Input.mousePosition);

        if (Physics.Raycast(ray, out RaycastHit raycastHit))
        {
            moveToVector = raycastHit.point - selectedCharacter.transform.position;
            Debug.Log("Trying to draw moveRay");
            Debug.DrawRay(startMoveVector, moveToVector, Color.blue);

            if (Input.GetMouseButtonDown(0))
            {
               // Debug.Log(Input.mousePosition);
            }
        }        
    }

    public void ChangeActiveCharacter() // Changes active character to next index in selectedCharacterList
    {   
        //Debug.Log("Trying to change character!");
        if (selectedCharacterWeapon.aimOn)
            selectedCharacterWeapon.aimOn = false;

        CheckActiveCamera();
        SetActiveCharacter();
    }

    private void CheckActiveCamera() // Changes camera to selected character, if character is down then do it again
    {
        selectedCharacter.cameraController.gameObject.SetActive(false);
        activeCharacterIndex++;
        if (activeCharacterIndex > selectedCharacterList.Count - 1)
            activeCharacterIndex = 0;
        
        selectedCharacter = selectedCharacterList[activeCharacterIndex];

        if (selectedCharacter.characterDown == true)
            CheckActiveCamera();

        selectedCharacterWeapon = selectedCharacter.GetComponent<Weapon>();
        selectedCharacter.cameraController.gameObject.SetActive(true);
    }

    private void SetActiveCharacter() // Sets active character and weapon
    {
        gameManager.activeCharacter = selectedCharacter.GetComponent<Player>();
        gameManager.activeCharacterWeapon = selectedCharacter.GetComponent<Weapon>();
        CharacterChange();
    }

    public void CharacterChange() // Changes UI to active character when changing character
    {
        gameManager.UpdateUIText();
        gameManager.ChangeWeaponUIText();
    }
}