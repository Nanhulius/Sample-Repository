using UnityEngine;

public class EnemyPlasmaBulletPooler : ObjectPooler<PlasmaBullet>, IPoolable<PlasmaBullet>
{
    protected override void InitializePool()
    {
        base.InitializePool();
        foreach (var plasmaBullet in pool)
            plasmaBullet.InitializeProjectile(this);
    }

    public void SpawnProjectile(Vector3 spawnPosition, Vector3 targetPosition)
    {
        PlasmaBullet plasmaBullet = GetFromPool();
        if (plasmaBullet != null)
            plasmaBullet.FireProjectile(spawnPosition, targetPosition);
        else
            Debug.LogWarning("No PlasmaBullets available in pool");
    }

    public override void ReturnToPool(PlasmaBullet obj)
    {
        obj.ResetProjectile();
        base.ReturnToPool(obj);
    }
}
