using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class Bullet : MonoBehaviour
{
    [SerializeField] public BulletPooler currentBulletPooler;
    [SerializeField] public GameObject parentOfParent;
    [SerializeField] public GameObject bulletPoolerObject;



    private void Awake()
    {
   
    }
    
    
    private void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.CompareTag("Target"))
        {
            Debug.Log("Bullet hits " + collision.gameObject.name + "!");
            //parentObject.ReturnBulletToPool();
        }
    }

}
