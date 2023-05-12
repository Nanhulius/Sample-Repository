using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

public class TurnManager : MonoBehaviour
{
    [SerializeField] public enum TurnOrder { Player, Enemy };
    [SerializeField] private TurnOrder turnOrder;
    [SerializeField] private int turnNumber = 1;

    private GameManager gameManager;

    private void Awake()
    {
        gameManager = GameObject.FindWithTag("GameManager").GetComponent<GameManager>();
    }

    private void Update()
    {
        if (turnOrder == TurnOrder.Player)
            return;
        else if (turnOrder == TurnOrder.Enemy)
        {          
            gameManager.enemyTurn = true;
        }         
    }

    public void ChangeTurn() // Changes turn to next
    {
        gameManager.ButtonNextTurn();
        ToggleTurnOrder();
        if (turnOrder == TurnOrder.Enemy)            
            return;
        else
            turnNumber++;
    }

    private void ToggleTurnOrder() // Toggles between player and enemy turn
    {      
        if (turnOrder == TurnOrder.Player)
        {
            gameManager.enemyTurn = true;
            turnOrder = TurnOrder.Enemy;
            gameManager.announcerText.text = "It's now " + turnOrder.ToString() + " turn!";
        }
        else
        {          
            gameManager.enemyTurn = false;
            turnOrder = TurnOrder.Player;
            gameManager.announcerText.text = "It's now " + turnOrder.ToString() + " turn!";
        }
        gameManager.EnableAnnouncerText();
    }
}