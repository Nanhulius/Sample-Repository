using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BulletPooler : MonoBehaviour
{
    public static BulletPooler instance;

    [SerializeField] public PoolerDataSO currentBulletPooler;

    [SerializeField] public Queue<GameObject> bulletPool = new Queue<GameObject>();

    [SerializeField] public Transform poolerPosition;

    private GameObject bullet = null;

    private void Awake()
    {
        poolerPosition = this.GetComponent<Transform>();
        ReserveBulletsToPool();
    }


    private void ReserveBulletsToPool() // Reserves amount of bullets defined by PoolerDataSO bulletPoolSize
    {
        for (int i = 0; i < currentBulletPooler.bulletPoolSize; i++) 
        {
            bullet = Instantiate(currentBulletPooler.bulletPrefab, poolerPosition.transform.position, Quaternion.identity);
            bulletPool.Enqueue(bullet);
            bullet.transform.parent = this.transform;
            bullet.SetActive(false);
            //Debug.Log("parent is " + bullet.transform.parent.name); 
        }
        //Debug.Log("bullet pool count is: " + bulletPool.Count);
    }

    public GameObject GetBulletFromPool() // When shooting takes a bullet from pool
    {
        if (bulletPool.Count > 0)
        {
            bullet = bulletPool.Dequeue();         
            bullet.SetActive(true);
            return bullet;
        }
        else
            Debug.Log("Pool Is Empty");
        return null;
    }

    //public void CheckBulletCollision(Collision collision)
    //{
    //    Debug.Log("Chekcking collison...");
    //    ReturnBulletToPool();
    //}

    
    public void ReturnBulletToPool() // Returns bullet back into pool
    {
        bullet.transform.position = poolerPosition.position;
        bulletPool.Enqueue(bullet);
        bullet.SetActive(false); 
    }
    
}
