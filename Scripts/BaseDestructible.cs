using System;
using System.Collections;
using UnityEngine;

public abstract class BaseDestructible : MonoBehaviour, IDestructible
{
    protected Damageable damageable;
    protected GameManager gm;

    protected Transform cachedTransform;
    public bool isBeingDestroyed = false;

    protected virtual void Awake()
    {
        InitializeBaseDestructible();
    }

    protected void InitializeBaseDestructible()
    {
        gm = GameObject.FindWithTag("GameManager").GetComponent<GameManager>();
        damageable = GetComponent<Damageable>();
        cachedTransform = transform;

        damageable.OnDeath -= HandleDestruction;
        damageable.OnDeath += HandleDestruction;
    }

    protected virtual void OnDestroy()
    {
        if (damageable != null)
            damageable.OnDeath -= HandleDestruction;
    }

     // When object health goes to 0, damageable calls this to handle the destruction.
    public virtual void HandleDestruction()
    {
        if (isBeingDestroyed) return;

        isBeingDestroyed = true;

        gm.multiParticlePooler.SpawnParticles("DestroyParticles", this.cachedTransform.position);

        HandlePostDestruction();
    }

    // This is used for extra object specific destruction stuff.
    protected abstract void HandlePostDestruction();

    protected abstract void ReturnToPool();
}
