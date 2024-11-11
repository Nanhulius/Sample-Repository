using UnityEngine;

public class EnemyAsteroidPooler : ObjectPooler<EnemyAsteroid>, IPoolable<EnemyAsteroid>
{
    private GameManager gm;
    private float spawnTimer;

    protected override void InitializePool()
    {
        gm = GameObject.FindWithTag("GameManager").GetComponent<GameManager>();
        spawnTimer = UnityEngine.Random.Range(1f, 3f);

        base.InitializePool();
    }

    private void Update()
    {
        if (spawnTimer <= 0f)
        {
            SpawnEnemyAsteroid();
            spawnTimer = UnityEngine.Random.Range(1f, 3f);
        }
        else
            spawnTimer -= Time.deltaTime;
    }

    public void SpawnEnemyAsteroid()
    {
        EnemyAsteroid enemyAsteroid = GetFromPool();
        float randomY = UnityEngine.Random.Range(gm.cachedScreenBounds.y + 10f, gm.cachedScreenBounds.w - 10f);
        enemyAsteroid.transform.position = new Vector2(this.transform.position.x, randomY);
    }

    public override void ReturnToPool(EnemyAsteroid obj)
    {
        obj.ResetObject();
        base.ReturnToPool(obj);  
    }
}