using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.AI;

public class Enemy : MonoBehaviour, IDamageable
{
    [SerializeField] private int maxHealth, currentHealth = 1;
    [SerializeField] private float jumpHeight, moveSpeed, visibility, range, targetDistance;
    [SerializeField] public float maxMoves, movesLeft;
    [SerializeField] private bool damageable = true;
    [SerializeField] public Player target;

    private GameManager gameManager;
    private UnitSelector unitSelector;
    private EnemyWeapon enemyWeapon;

    private NavMeshAgent navAgent;
    private Rigidbody rb;

    private void Start()
    {      
        InitializeEnemy();
    }

    private void Update()
    {
        if (!gameManager.enemyTurn)
            return;
        else
        {
            CheckTarget();
            EnemyMovesOrShoots();
        }
    }

    private void InitializeEnemy() // Adds this enemy to enemyList
    {
        currentHealth = maxHealth;
        movesLeft = maxMoves;
        rb = GetComponent<Rigidbody>();    
        gameManager = GameObject.FindWithTag("GameManager").GetComponent<GameManager>();
        enemyWeapon = gameObject.GetComponentInChildren<EnemyWeapon>();
        unitSelector = gameManager.GetComponent<UnitSelector>();
        //target = gameManager.activeCharacter.GetComponent<Transform>().transform;
        navAgent = GetComponent<NavMeshAgent>();
        unitSelector.enemyList.Add(this);
    }

    void IDamageable.TakeDamage(int damage)
    {
        if (!damageable)
            return;
        else
        {
            Debug.Log(this.gameObject.name + " is taking Damage!");
            currentHealth -= damage;
            if (currentHealth <= 0)
                Destroy(gameObject);
        }
    }

    private void EnemyMovement() // Moves this enemy towards the closest Player Character that has been made as a target
    {    
        if (movesLeft >= 0)
        {
            transform.LookAt(target.transform.position);           
            movesLeft -= Time.deltaTime;
        }
        else
            navAgent.SetDestination(this.transform.position);
    }

    private void CheckTarget()  // Takes the nearest Player Character as a target
    {
        foreach (Player p in unitSelector.selectedCharacterList)
        {
            if (!target.characterDown) 
            {
                float distanceToNextTarget = Vector3.Distance(p.transform.position, this.transform.position);   

            }
            targetDistance = Vector3.Distance(p.transform.position, this.transform.position);

          //  if (targetDistance >= distanceToNextTarget)
           //     targetDistance = distanceToNextTarget;
            Debug.Log("Target name is " + p.name);
            Debug.Log("Target distance is " + targetDistance);
           // Debug.Log("DistanceToNextTarget is " + distanceToNextTarget);
            //if (targetDistance > distanceToNextTarget)
              //  target = p.gameObject.GetComponent<Player>().transform.position;


            //if (distanceToTarget < targetDistance)
        }
        navAgent.SetDestination(target.transform.position);
    }

    private void EnemyAttack()
    {
        if (targetDistance <= range && movesLeft >= 1)
        {
            enemyWeapon.EnemyShoots();
        } 
    }
    

    private void EnemyMovesOrShoots()
    {
        if (targetDistance <= range)
        {
            navAgent.SetDestination(this.transform.position);
            EnemyAttack();
        }
        //else if (targetDistance > range)
          //  EnemyMovement();
    }
}