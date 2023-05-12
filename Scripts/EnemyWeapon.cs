using System;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using UnityEngine;

public class EnemyWeapon : MonoBehaviour
{
    [SerializeField] List<ScriptableObject> enemyWeaponList = new List<ScriptableObject>(); // List for weapons
    [SerializeField] List<GameObject> enemyBulletPoolerList = new List<GameObject>(); // List for pooler gameobjects

    [SerializeField] public WeaponDataSO currentWeapon;
    [SerializeField] public BulletPooler currentBulletPooler;
    [SerializeField] public GameObject currentBulletPoolerObject;
    [SerializeField] private Transform bulletSpawner;

    private Vector3 target;

    private Enemy enemy;
    private Rigidbody rb;

    private int enemyWeaponIndex;
    private int enemyPoolerIndex;
    private WeaponDataSO.ShootingMode shootingMode;

    private Vector3 randomizedAccuracy;

    private void Awake()
    {
        enemy = GetComponent<Enemy>();
        enemyWeaponIndex = enemyWeaponList.IndexOf(currentWeapon);
        SetEnemyBulletPoolers();
        currentBulletPoolerObject = enemyBulletPoolerList[enemyWeaponIndex];
        currentBulletPooler = currentBulletPoolerObject.GetComponent<BulletPooler>();
        SetEnemyAttackMode();
        currentWeapon.shooting = true;
    }

    private void SetEnemyBulletPoolers() //Sets correct bulletPooler for enemy
    {
        switch(enemyWeaponIndex)
        {
            case 0:
                enemyBulletPoolerList[0] = GameObject.FindWithTag("EnemyPistolPooler");
                break;
            default:
                break;                  
        }
    }

    private void SetEnemyAttackMode() // Sets Enemy shootingMode depenging on the weapon they are using
    {
        switch (currentWeapon.shootingMode)
        {
            case WeaponDataSO.ShootingMode.Single:
                Debug.Log("Shooting mode is " + WeaponDataSO.ShootingMode.Single);
                //StartCoroutine(Shoot());
                shootingMode = WeaponDataSO.ShootingMode.Single;
                break;

            case WeaponDataSO.ShootingMode.Burst:
                Debug.Log("Shooting mode is " + WeaponDataSO.ShootingMode.Burst);
                //StartCoroutine(BurstFire());
                break;

            case WeaponDataSO.ShootingMode.Auto:
                Debug.Log("Shooting mode is " + WeaponDataSO.ShootingMode.Auto);
                //StartCoroutine(Shoot());
                break;

            default:
                Debug.Log("Shooting mode is default");
                //StartCoroutine(Shoot());
                break;
        }
    }

    public void EnemyShoots() // Shoots depending on their shootingMode
    {
        if (shootingMode == 0 && currentWeapon.shooting == true)
        {     
            StartCoroutine(ShootSingle());
        }
    }

    private IEnumerator ShootSingle() // Shoots a bullet if shootingMode = Single
    {
        currentWeapon.shooting = false;
        target = enemy.target.transform.position;
        GameObject bullet = currentBulletPooler.GetBulletFromPool();
        bullet.transform.position = bulletSpawner.position;
        bullet.transform.rotation = Quaternion.identity;
        rb = bullet.GetComponent<Rigidbody>();
        rb.velocity = Vector3.zero;
        rb.AddForce((target - bulletSpawner.position) * currentWeapon.bulletSpeed);
        yield return new WaitForSeconds(currentWeapon.fireRate);
        currentWeapon.shooting = true;
        enemy.movesLeft -= 1;
    }
}
