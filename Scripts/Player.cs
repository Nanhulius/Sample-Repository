using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class Player : MonoBehaviour
{
    [SerializeField] public Camera mainCamera;
    private GameManager gameManager;

    [SerializeField] private GameObject playerPrefab;
   
    [SerializeField] private float moveSpeed;
    [SerializeField] private float jumpHeight;
    [SerializeField] public float movesLeft;

    
    private TextMeshProUGUI movesText;
    private bool moving = false;
    private Rigidbody rb;

    private void Awake()
    {
        rb = GetComponent<Rigidbody>();
        gameManager = GameObject.FindWithTag("GameManager").GetComponent<GameManager>();
    }

    private void Update()
    {
        MovePlayer();
    }

    private void MovePlayer()
    {
        if (movesLeft >= 0)
        {
            var dir = new Vector3(Input.GetAxis("Horizontal"), 0f, Input.GetAxis("Vertical"));
            transform.Translate(dir * moveSpeed * Time.deltaTime);
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
            gameManager.EnableOutOfMovesText();
        }
    }

    private void UpdateUIText()
    {
        gameManager.movesText.text = "Moves left: " + movesLeft.ToString("F2");
    }
}
