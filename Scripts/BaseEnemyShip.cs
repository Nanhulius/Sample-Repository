using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class BaseEnemyShip : BaseDestructible
{
    private EnemyHealthBar healthBar;
    private OscillatingMovement oscillatingMovement;

    [SerializeField] private float moveSpeed, floatAmplitude, floatFrequency;

    [SerializeField] private GameObject sprites;
    [SerializeField] private Canvas canvas;

    public Transform playerTransform;
    public Transform projectileSpawnPoint;

    private PolygonCollider2D polyCollider;
    private Rigidbody2D rb;
    private Vector3 startingScale;

    public bool hasUpgrade = false;

    private const float initialHealth = 100f;

    protected override void Awake()
    {
        base.Awake();      
    }

    private void Start()
    {
        InitializeEnemy();
    }

    public void InitializeEnemy()
    {
        healthBar = GetComponentInChildren<EnemyHealthBar>();
        damageable = GetComponent<Damageable>();
        polyCollider = GetComponent<PolygonCollider2D>();
        rb = GetComponent<Rigidbody2D>();
        startingScale = cachedTransform.localScale;
        playerTransform = GameObject.FindWithTag("Player").transform;
    }

    public virtual void OnSpawned()
    {
        if (oscillatingMovement == null)
            oscillatingMovement = new OscillatingMovement(cachedTransform.position, moveSpeed, floatAmplitude, floatFrequency);
        else
            oscillatingMovement.UpdatePosition(cachedTransform.position);

        StartAttack();
    }

    protected virtual void PerformMovement()
    {
        Vector3 newPosition = oscillatingMovement.GetNewPosition(Time.deltaTime, cachedTransform.position);
        cachedTransform.position = newPosition;
    }

    protected override void HandlePostDestruction()
    {
        canvas.enabled = false;
        sprites.SetActive(false);

        if (!hasUpgrade)
            gm.pointCapsulePooler.SpawnPointCapsule(this.transform.position);
        else
            gm.upgradeCapsulePooler.SpawnUpgradeCapsule(this.transform.position);

        ReturnToPool();
        
    }

    public virtual void ResetObject()
    {
        ResetOptions();
        cachedTransform.localScale = startingScale;
        canvas.enabled = true;
        damageable.health = initialHealth;
        healthBar.slider.value = initialHealth;
        sprites.SetActive(true);
        isBeingDestroyed = false;
        StopAllCoroutines();
    }

    protected abstract void StartAttack();

    protected abstract void ResetOptions();
}