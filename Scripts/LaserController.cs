using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LaserController : MonoBehaviour, ISystem
{
    //[SerializeField] private Transform laserStartPoint;
    [SerializeField] private CircleCollider2D laserRadiusCollider;

    [Header("Laser Turret values")]
    public float damage = 50f;
    public float cooldown = 3f;
    public float beamTime = 1f;
    public float range = 3f;
    /* If laserRange is 1, then laserRadiusCollider is 10, if laserRange is 2, then
     * laserRadiusCollider is 20 etc...
    */
    public bool doubleLaser = false;
    public bool tripleLaser = false;

    private float maxDamage = 100f;
    private float maxCooldown = 0f;
    private float maxBeamTime = 5f;
    private float maxRange = 10f;

    public LineRenderer lineRen1;
    private LineRenderer lineRen2;
    private LineRenderer lineRen3;

    private bool isLaserActive = false;
    private bool isInEMPFIeld = false;

    private int debrisLayer;
    private int debrisLayerMask;

    public Transform currentTarget1 = null;
    public Transform currentTarget2 = null;
    public Transform currentTarget3 = null;

    private Transform cachedTransform;

    private Coroutine laserRoutine;

    private void Awake()
    {
        InitializeLasers();
        //InitializeLineRenderer(lineRen1);
        //InitializeLineRenderer(lineRen2);
        //InitializeLineRenderer(lineRen3);
    }

    private void InitializeLasers()
    {
        cachedTransform = transform;
        lineRen1 = transform.Find("Laser1").GetComponent<LineRenderer>();
        lineRen2 = transform.Find("Laser2").GetComponent<LineRenderer>();
        lineRen3 = transform.Find("Laser3").GetComponent<LineRenderer>();
        lineRen1.enabled = false;
        lineRen2.enabled = false;
        lineRen3.enabled = false;

        debrisLayer = LayerMask.NameToLayer("SpaceDebris");
        debrisLayerMask = 1 << debrisLayer;

        SetLaserRange();
    }

    void Start()
    {
        if (this.enabled)
            laserRoutine = StartCoroutine(ShootLaserRoutine());
    }

    void Update()
    {
        if (isLaserActive)
            UpdateLaserPositions();
    }


    // Currently not using this. Might be useful in the future.
    private void InitializeLineRenderer(LineRenderer lineRen)
    {
        lineRen.positionCount = 2;
        lineRen.startWidth = 0.1f;
        lineRen.endWidth = 0.1f;
        lineRen.material = new Material(Shader.Find("Sprites/Default"));
        lineRen.startColor = Color.red;
        lineRen.endColor = Color.red;
    }

    private void SetLaserRange()
    {
        float maxScale = Mathf.Max(transform.lossyScale.x, transform.lossyScale.y, transform.lossyScale.z);
        laserRadiusCollider.radius = range / maxScale;
        //Debug.Log($"Laser radius set to: {laserRadiusCollider.radius}");
    }

    // When targets exit the range-collider laser is deactivated and target is set to null
    private void OnTriggerExit2D(Collider2D other)
    {
        if (other.gameObject.layer == debrisLayer)
        {
            if (currentTarget1 == other.transform)
            {
                currentTarget1 = null;
                lineRen1.enabled = false;
            }
            if (currentTarget2 == other.transform)
            {
                currentTarget2 = null;
                lineRen2.enabled = false;
            }
            if (currentTarget3 == other.transform)
            {
                currentTarget3 = null;
                lineRen3.enabled = false;
            }
        }   
    }

    //Makes Laser Radius visible in editor
    /*    private void OnDrawGizmos()
    {
        if (laserRadiusCollider != null)
        {
            Gizmos.color = Color.yellow;
            Gizmos.DrawWireSphere(transform.position, laserRadiusCollider.radius * Mathf.Max(transform.lossyScale.x, transform.lossyScale.y, transform.lossyScale.z));
        }
        if (Application.isPlaying)
        {
            int enemyLayerMask = 1 << enemyLayer;
            float detectionRadius = laserRadiusCollider.radius * Mathf.Max(transform.lossyScale.x, transform.lossyScale.y, transform.lossyScale.z);
            Collider2D[] targetsInRange = Physics2D.OverlapCircleAll(transform.position, detectionRadius, enemyLayerMask);

            Gizmos.color = Color.green;
            foreach (var target in targetsInRange)
                Gizmos.DrawWireSphere(target.transform.position, 0.5f);
        }
    }*/  

    // Updates Laser targets, if old target is being destroyed or out of range targets a new target. 
    private void UpdateLaserPositions()
    {     
        bool hasValidTargets = false;

        if (currentTarget1 == null || IsTargetBeingDestroyed(currentTarget1))
        {
            currentTarget1 = GetNewTarget(currentTarget2, currentTarget3);
            lineRen1.enabled = currentTarget1 != null;
        }
        if (currentTarget1 != null)
        {
            LaserLineDamage(lineRen1, currentTarget1);
            hasValidTargets = true;
        }
        else
            lineRen1.enabled = false;

        if (doubleLaser && (currentTarget2 == null || IsTargetBeingDestroyed(currentTarget2)))
        {
            currentTarget2 = GetNewTarget(currentTarget1, currentTarget3);
            lineRen2.enabled = currentTarget2 != null;
        }
        if (currentTarget2 != null)
        {
            LaserLineDamage(lineRen2, currentTarget2);
            hasValidTargets = true;
        }
        else
            lineRen2.enabled = false;

        if (tripleLaser && (currentTarget3 == null || IsTargetBeingDestroyed(currentTarget3)))
        {
            currentTarget3 = GetNewTarget(currentTarget1, currentTarget2);
            lineRen3.enabled = currentTarget3 != null;
        }
        if (currentTarget3 != null)
        {
            LaserLineDamage(lineRen3, currentTarget3);
            hasValidTargets = true;
        }
        else
            lineRen3.enabled = false;

        if (!hasValidTargets)
        {
            isLaserActive = false;
            lineRen1.enabled = false;
            lineRen2.enabled = false;
            lineRen3.enabled = false;
        }
    }

    private bool IsTargetBeingDestroyed(Transform target)
    {
        BaseDestructible destructible = target.GetComponent<BaseDestructible>();
        return destructible != null && destructible.isBeingDestroyed;
    }

    // Gives damage to the target.
    private void LaserLineDamage(LineRenderer lineRen, Transform target)
    {
        if (target != null)
        {
            lineRen.SetPosition(0, cachedTransform.position);
            lineRen.SetPosition(1, target.position);

            Damageable damageable = target.GetComponent<Damageable>();
            if (damageable != null)
                damageable.TakeDamage(damage * Time.deltaTime);

            lineRen.enabled = true;
        }
        else
            lineRen.enabled = false;        
    }

    // Coroutine for shooting the basic lasers. If there is no targets it waits until there is.
    private IEnumerator ShootLaserRoutine()
    {
        while (true && !isInEMPFIeld)
        {
            List<Transform> closestTargets = SetClosestTargets();

            if (closestTargets.Count > 0)
            {
                // Targets detected, resume shooting.
                currentTarget1 = closestTargets[0];
                lineRen1.enabled = true;

                if (doubleLaser && closestTargets.Count > 1)
                {
                    currentTarget2 = closestTargets[1];
                    lineRen2.enabled = true;
                }

                if (tripleLaser && closestTargets.Count > 2)
                {
                    currentTarget3 = closestTargets[2];
                    lineRen3.enabled = true;
                }

                isLaserActive = true;
                yield return new WaitForSeconds(beamTime);

                lineRen1.enabled = false;
                lineRen2.enabled = false;
                lineRen3.enabled = false;
                isLaserActive = false;
            }
            else
            {
                currentTarget1 = null;
                currentTarget2 = null;
                currentTarget3 = null;
                isLaserActive = false;
                lineRen1.enabled = false;
                lineRen2.enabled = false;
                lineRen3.enabled = false;
            }

            yield return new WaitForSeconds(cooldown);

            while (SetClosestTargets().Count == 0)
            {
                currentTarget1 = null;
                currentTarget2 = null;
                currentTarget3 = null;
                yield return null;         
            }
        }
    }

    // Sets target to the closet gameobject within "Enemy" -layer
    private List<Transform> SetClosestTargets()
    {
        float detectionRadius = laserRadiusCollider.radius * Mathf.Max(transform.lossyScale.x, transform.lossyScale.y, transform.lossyScale.z);
        Collider2D[] targetsInRange = Physics2D.OverlapCircleAll(cachedTransform.position, detectionRadius, debrisLayerMask);

        List<Transform> closestTargets = new List<Transform>();

        foreach (Collider2D target in targetsInRange)
        {
            EnemyAsteroid enemyAsteroid = target.GetComponent<EnemyAsteroid>();
            if (enemyAsteroid != null && !enemyAsteroid.isBeingDestroyed)
                closestTargets.Add(target.transform);
        }
        closestTargets.Sort((a, b) => Vector3.Distance(cachedTransform.position, a.position).CompareTo(Vector3.Distance(cachedTransform.position, b.position)));
        return closestTargets;
    }

    // Gets new target for the laser if the old target is destroyed and excludes targets that are already targeted with other lasers.
    private Transform GetNewTarget(params Transform[] excludeTargets)
    {
        List<Transform> closestTargets = SetClosestTargets();
        foreach (Transform exclude in excludeTargets)
            closestTargets.Remove(exclude);
        return closestTargets.Count > 0 ? closestTargets[0] : null;
    }

    public void UpgradeDamage()
    {
        if (damage <= maxDamage)
            damage += 5;

        damage = Mathf.Min(damage, maxDamage);
    }

    public void UpgradeRange()
    {
        if (range <= maxRange)
        {
            range += 1f;
            float maxScale = Mathf.Max(transform.lossyScale.x, transform.lossyScale.y, transform.lossyScale.z);
            laserRadiusCollider.radius = range / maxScale;
        }
        range = Mathf.Min(range, maxRange);
    }

    public void UpgradeBeamTime()
    {
        if (beamTime <= maxBeamTime)
            beamTime += 0.2f;

        beamTime = Mathf.Min(beamTime, maxBeamTime);
    }

    public void UpgradeCooldown()
    {
        if (cooldown <= maxCooldown)
            cooldown -= 0.2f;

        cooldown = Mathf.Max(cooldown, maxCooldown);
    }

    public void UpgradeTurrets()
    {
        if (!doubleLaser)
            doubleLaser = true;
        else if (!tripleLaser)
            tripleLaser = true;
    }

    private void DisableLaserBeams()
    {
        isLaserActive = false;
        currentTarget1 = null;
        currentTarget2 = null;
        currentTarget3 = null;
        lineRen1.enabled = false;
        lineRen2.enabled = false;
        lineRen3.enabled = false;
    }

    public void DisableSystem()
    {
        isInEMPFIeld = true;
        DisableLaserBeams();
        if (laserRoutine != null)
        {
            StopCoroutine(laserRoutine);
            laserRoutine = null;
        }
        this.enabled = false;
    }

    public void EnableSystem()
    {
        isInEMPFIeld = false;
        this.enabled = true;

        if (laserRoutine == null)
            StartCoroutine(ShootLaserRoutine());
    }
}