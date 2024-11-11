using System.Collections.Generic;
using UnityEngine;

public class Player : MonoBehaviour
{
    public GameManager gm;
    private Camera mainCamera;

    public PlayerData playerData;

    public ProjectileManager projectileManager;
    private CargoController cargoController;   
    public EngineController engineController;
    public ShieldController shieldController;
    public AntiMatterController antiMatterController;
    public LaserController laserController;
    public VacuumController vacuumController;
    public UtilityController utilityController;
    private Transform cachedTransform;

    public delegate void InputAction();
    public event InputAction OnPrimayFirePressed;
    public event InputAction OnPrimayFireReleased;
    public event InputAction OnUtilityPressed;
    public event InputAction OnUtilityReleased;
    
    public SpriteRenderer spriteRenderer;

    public GameObject vacuum;
    public GameObject weapon;

    public PolygonCollider2D playerCollider;

    public Color alpha;
    public bool playerIsDead = false;

    private int enemyLayer;
    private int beltLayer;
    private int debrisLayer;
    private int projectileLayer;

    private HashSet<int> damageLayers;

    [SerializeField] private bool immortality = false;

    private void Awake()
    {
        InitializePlayer();
    }

    private void InitializePlayer()
    {
        gm = GameObject.FindWithTag("GameManager").GetComponent<GameManager>();
        mainCamera = GameObject.FindWithTag("MainCamera").GetComponent<Camera>();
        projectileManager = GetComponentInChildren<ProjectileManager>();
        spriteRenderer = GetComponent<SpriteRenderer>();
        alpha = spriteRenderer.GetComponent<SpriteRenderer>().color;
        playerCollider = GetComponent<PolygonCollider2D>();
        antiMatterController = GetComponentInChildren<AntiMatterController>();
        
        laserController = GetComponentInChildren<LaserController>();
        shieldController = GetComponentInChildren<ShieldController>();
        cargoController = GetComponentInChildren<CargoController>();
        engineController = GetComponentInChildren<EngineController>();
        
        vacuumController = GetComponentInChildren<VacuumController>();
        utilityController = GetComponent<UtilityController>();
        cachedTransform = transform;

        OnPrimayFirePressed += () => UseMainWeapon(true);
        OnPrimayFireReleased += () => UseMainWeapon(false);
        OnUtilityPressed += () => utilityController.UseActiveUtility(true);
        OnUtilityReleased += () => utilityController.UseActiveUtility(false);

        enemyLayer = LayerMask.NameToLayer("Enemy");
        beltLayer = LayerMask.NameToLayer("BeltAsteroid");
        debrisLayer = LayerMask.NameToLayer("SpaceDebris");
        projectileLayer = LayerMask.NameToLayer("EnemyProjectile");
        damageLayers = new HashSet<int> { beltLayer, debrisLayer, enemyLayer, projectileLayer };

        GatherPlayerData();
    }

    private void GatherPlayerData()
    {
        if (playerData != null)
        {
            utilityController.activeUtility = playerData.startingUtility;
        }

        if (playerData.hasAntiMatterBlast)
        {
            antiMatterController.cooldown = playerData.antiMatterCooldown;
            antiMatterController.damage = playerData.antiMatterDamage;
            antiMatterController.speed = playerData.projectileSpeed;
        }

        if (playerData.hasCargo)
        {
            cargoController.currentCargoSlots = playerData.currentCargoSlots;
            cargoController.maxCargoSlots = playerData.maxCargoSlots;
        }

        if (playerData.hasEngine)
        {
            engineController.acceleration = playerData.acceleration;
            engineController.deceleration = playerData.deceleration;
            engineController.speed = playerData.speed;
            engineController.bwSpeed = playerData.backwardsSpeed;
        }

        if (playerData.hasLaser)
        {
            laserController.beamTime = playerData.beamTime;
            laserController.cooldown = playerData.laserCooldown;
            laserController.damage = playerData.laserDamage;
            laserController.range = playerData.range;
            if (playerData.hasDoubleLaserAtStart)
                laserController.doubleLaser = true;
            if (playerData.hasTripleLaserAtStart)
                laserController.tripleLaser = true;
        }

        if (playerData.hasShield)
        {
            shieldController.shieldRadius = playerData.shieldRadius;
            shieldController.shieldCooldownDuration = playerData.shieldCooldown;
        }

        if (playerData.hasVacuum)
        {
            vacuumController.pullForce = playerData.pullForce;
            vacuumController.radius = playerData.vacuumRadius;
        }
    }

    private void Update()
    {
        if (!playerIsDead && !utilityController.isChoosingUtility)
            HandleInput();
    }

    private void HandleInput()
    {
        if (Input.GetMouseButton(0))
            OnPrimayFirePressed?.Invoke();
        else if (Input.GetMouseButtonUp(0))
            OnPrimayFireReleased?.Invoke();

        if (Input.GetMouseButton(1))
            OnUtilityPressed?.Invoke();
        else if (Input.GetMouseButtonUp(1))
            OnUtilityReleased?.Invoke();

        for (int i = 0; i < gm.utilityManager.currentSlotCount; i++)
        {
            if (Input.GetKeyDown(KeyCode.Alpha1 + i))
            {
                gm.utilityManager.SelectUtilityByIndex(i);
                break;
            }
        }
    }

    private void UseMainWeapon(bool isPressed)
    {
        if (isPressed && antiMatterController.isAntiMatterReady)
        {
            Vector3 targetPosition = mainCamera.ScreenToWorldPoint(Input.mousePosition);
            targetPosition.z = 0f;

            projectileManager.FireAntiMatterBlast(utilityController.projectileSpawnPosition.position, targetPosition);
            StartCoroutine(antiMatterController.AntiMatterCooldown());
        }
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (damageLayers.Contains(collision.gameObject.layer))
        {
            if (!IsShieldActive())                           
                 KillPlayer();
        }
    }

    public void KillPlayer()
    {
        if (immortality)
            return;

        OnPrimayFirePressed -= () => UseMainWeapon(true);
        OnPrimayFireReleased -= () => UseMainWeapon(false);
        OnUtilityPressed -= () => utilityController.UseActiveUtility(true);
        OnUtilityReleased -= () => utilityController.UseActiveUtility(false);

        gm.OnPlayerDeath();

        if (cargoController != null)
            cargoController.ScatterCargo();
    }

    public bool IsShieldActive()
    {
        return shieldController != null && shieldController.IsShieldActive();
    }
}