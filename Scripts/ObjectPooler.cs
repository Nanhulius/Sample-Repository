using System.Collections.Generic;
using UnityEngine;

public class ObjectPooler<T> : MonoBehaviour where T : Component
{
    // This is used as a template for other poolers so there wouldn't be that much of duplicate code.
    [SerializeField] private T prefab;
    [SerializeField] private int poolSize = 32;

    [SerializeField] private Transform parentObject;
    protected Queue<T> pool;

    protected virtual void Awake()
    {
        pool = new Queue<T>();
        InitializePool();
    }

    // Spawns the wanted amount of objects to their respective pools.
    protected virtual void InitializePool()
    {
        for (int i = 0; i < poolSize; i++)
        {
            T obj = Instantiate(prefab);
            obj.gameObject.SetActive(false);
            pool.Enqueue(obj);
            obj.transform.SetParent(parentObject);
        }
    }

    // Gets an object from the pool.
    public virtual T GetFromPool()
    {
        if (pool.Count > 0)
        {
            T obj = pool.Dequeue();
            obj.gameObject.SetActive(true);
            return obj;
        }
        else // If the pool is already empty new one is being instantiated.
        {
            T obj = Instantiate(prefab);
            return obj;
        }
    }

    // Returns the object back to its pool.
    public virtual void ReturnToPool(T obj)
    {
        obj.gameObject.SetActive(false);
        pool.Enqueue(obj);
    }

    protected Queue<T> Pool => pool;
}