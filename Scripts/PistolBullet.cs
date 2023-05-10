using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PistolBullet : MonoBehaviour
{
    [SerializeField] private BulletPooler bulletPooler;
    [SerializeField] private int damage = 4;

    private void Awake()
    {
        bulletPooler = GameObject.FindWithTag("PistolBulletPooler").GetComponent<BulletPooler>(); 
    }
    
    private void OnCollisionEnter(Collision collision)
    {
        //Debug.Log("Bullet hit " + collision.gameObject.name + ("!"));
        DamageCollision(collision);
    }

    private void DamageCollision(Collision collision)
    {
        IDamageable damageable = collision.gameObject.GetComponent<IDamageable>();

        if (damageable != null) 
        {
            damageable.TakeDamage(damage);
        }
        bulletPooler.ReturnBulletToPool();
    }
}