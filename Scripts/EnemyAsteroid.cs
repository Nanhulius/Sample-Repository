using TMPro.Examples;
using UnityEngine;

public class EnemyAsteroid : BaseDestructible
{
    private EnemyHealthBar healthBar;

    [SerializeField] private Canvas canvas;
    [SerializeField] private float upgradeProbability = 0.9f;
    [SerializeField] private float spinSpeedRange = 50f;
    [SerializeField] private float moveSpeed = -20f;

    private Vector3 startingScale;
    public float spinSpeed;

    private SpriteRenderer sprite;
    public PolygonCollider2D polyCollider;
    public bool isInBlackhole = false;
    public bool hasUpgrade = false;

    private const float initialHealth = 30f;

    protected override void Awake()
    {
        base.Awake();
        InitializeEnemyAsteroid();
    }

    public void InitializeEnemyAsteroid()
    {
        polyCollider = GetComponent<PolygonCollider2D>();
        damageable = GetComponent<Damageable>();
        healthBar = GetComponentInChildren<EnemyHealthBar>();
        startingScale = cachedTransform.localScale;
        sprite = GetComponent<SpriteRenderer>();
        RandomizeIfHasUpgrade();
        SetRandomSpinSpeed();
    }

    void Update()
    {
        MoveEnemyAsteroid();
    }

    private void MoveEnemyAsteroid()
    {
        cachedTransform.Rotate(0f, 0f, spinSpeed * Time.deltaTime, Space.Self);
        cachedTransform.position += new Vector3(moveSpeed * Time.deltaTime, 0, 0);
    }

    // Handles object destruction when health is zero.
    protected override void HandlePostDestruction()
    {
        canvas.enabled = false;
        sprite.enabled = false;        

        if (!hasUpgrade)
            gm.pointCapsulePooler.SpawnPointCapsule(this.transform.position);
        else
            gm.upgradeCapsulePooler.SpawnUpgradeCapsule(this.transform.position);

        ReturnToPool();
    }

    // Resets the values of the EnemyAsteroid when it's returned to the pool.
    public void ResetObject()
    {
        SetRandomSpinSpeed();
        transform.position = gm.enemyAsteroidPooler.transform.position;
        cachedTransform.rotation = Quaternion.identity;
        cachedTransform.localScale = startingScale;
        canvas.enabled = true;
        damageable.health = initialHealth;
        healthBar.slider.value = initialHealth;
        polyCollider.enabled = true;
        sprite.enabled = true;
        isBeingDestroyed = false;
        RandomizeIfHasUpgrade();
    }

    protected override void ReturnToPool()
    {
        gm.enemyAsteroidPooler.ReturnToPool(this);
    }

    private void SetRandomSpinSpeed()
    {
        spinSpeed = Random.Range(-spinSpeedRange, spinSpeedRange);
    }

    // Checks if the asteroid has an upgrade.
    private void RandomizeIfHasUpgrade()
    {
        hasUpgrade = Random.value <= upgradeProbability;
    }

    // For ScannerBubbles to rotate the sprite.
    public float GetCurrentRotation()
    {
        return transform.rotation.eulerAngles.z;
    }
}