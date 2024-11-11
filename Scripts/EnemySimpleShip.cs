using System.Collections;
using UnityEngine;

public class EnemySimpleShip : BaseEnemyShip, IEMPable
{
    private EnemyPlasmaBulletPooler plasmaBulletPooler;

    [SerializeField] private float burstCooldown = 0.2f;
    [SerializeField] private float waitAfterBurst = 3f;
    [SerializeField] private float burstAmount = 3f;

    [SerializeField] private GameObject projectilePrefab;

    private Vector3 targetPosition;
    private Coroutine shootingRoutine;

    private bool isInEMPField = false;

    protected override void Awake()
    {
        base.Awake();
        plasmaBulletPooler = GameObject.FindWithTag("EnemyProjectilePooler").GetComponent<EnemyPlasmaBulletPooler>();
    }

    private void Update()
    {
        PerformMovement();
        CheckOutOfBounds();
    }

    private IEnumerator ShootPlayer()
    {
        while (gameObject.activeInHierarchy && !isInEMPField)
        {
            if (playerTransform != null)
            {
                for (int i = 0; i < burstAmount; i++)
                {
                    targetPosition = playerTransform.position;
                    Vector3 spread = new Vector3(Random.Range(-0.1f, 0.1f), Random.Range(-0.1f, 0.1f), 0);
                    plasmaBulletPooler.SpawnProjectile(projectileSpawnPoint.position, targetPosition + spread);
                    yield return new WaitForSeconds(burstCooldown);
                }
                yield return new WaitForSeconds(waitAfterBurst);
            }
            else
                yield return null;
        }
        shootingRoutine = null;
    }

    protected override void StartAttack()
    {
        if (shootingRoutine == null)
            shootingRoutine = StartCoroutine(ShootPlayer());
    }

    private void CheckOutOfBounds()
    {
        float leftBoundary = gm.cachedScreenBounds.x;

        if (cachedTransform.position.x < leftBoundary)
        {
            if (shootingRoutine != null)
            {
                StopCoroutine(shootingRoutine);
                shootingRoutine = null;
            }
        }
    }

    protected override void ResetOptions()
    {
        transform.position = gm.enemySimpleShipPooler.transform.position;
        if (shootingRoutine != null)
            StopCoroutine(shootingRoutine);

        shootingRoutine = null;
    }

    protected override void ReturnToPool()
    {
        gm.enemySimpleShipPooler.ReturnToPool(this);
    }

    public void OnEnterEMPField()
    {
        DisableSystems();
    }

    public void OnExitEMPField()
    {
        EnableSystems();
    }

    private void DisableSystems()
    {
        isInEMPField = true;
        if (shootingRoutine != null)
            StopCoroutine(shootingRoutine);
        shootingRoutine = null;
    }

    private void EnableSystems()
    {
        isInEMPField = false;
        StartAttack();
    }
}
