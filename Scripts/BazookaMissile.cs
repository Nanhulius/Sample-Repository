using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class BazookaMissile : MonoBehaviour
{
    [SerializeField] private BulletPooler bulletPooler;



    private void Awake()
    {
        bulletPooler = GameObject.FindWithTag("BazookaMissilePooler").GetComponent<BulletPooler>(); 
    }
    
    
    private void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.CompareTag("Target"))
        {
            Debug.Log("Bullet hits " + collision.gameObject.name + "!");
            
        }
        bulletPooler.ReturnBulletToPool();
    }

}
