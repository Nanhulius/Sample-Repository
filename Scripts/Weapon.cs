using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using Random = UnityEngine.Random;


public class Weapon : MonoBehaviour
{
    [SerializeField] List<ScriptableObject> weaponList = new List<ScriptableObject>(); // List for weapons
    //[SerializeField] List<ScriptableObject> poolerList = new List<ScriptableObject>(); // List for poolers
    [SerializeField] List<GameObject> bulletPoolerList = new List<GameObject>(); // List for pooler gameobjects

    private Player player;
    private GameManager gameManager;

    //[SerializeField] public PoolerDataSO currentPooler;
    [Header("Weapon in use Info")]
    [SerializeField] public WeaponDataSO currentWeapon;
    [SerializeField] public BulletPooler currentBulletPooler;
    [SerializeField] public GameObject currentBulletPoolerObject;
    [SerializeField] private Transform bulletSpawner;
    [SerializeField] private GameObject ammoInUse;

    private Rigidbody rb;

    private Vector3 shootingDirection;
    private Vector3 randomizedAccuracy;

    private int weaponIndex;
    private int poolerIndex;
    private int bulletPoolerIndex;
    public bool aimOn = false;

    private void Awake()
    {
        player = GameObject.FindWithTag("Player").GetComponent<Player>();
        gameManager = GameObject.FindWithTag("GameManager").GetComponent<GameManager>();
        currentWeapon = (WeaponDataSO)weaponList[0];
        weaponIndex = weaponList.IndexOf(currentWeapon);
        SetBulletPoolers();
        currentBulletPoolerObject = bulletPoolerList[weaponIndex];
        gameManager.ChangeWeaponUIText();
        currentBulletPooler = currentBulletPoolerObject.GetComponent<BulletPooler>(); 
        ammoInUse = currentBulletPooler.currentBulletPooler.bulletPrefab;
        poolerIndex = bulletPoolerList.IndexOf(currentBulletPoolerObject);
        currentWeapon.shooting = true;
    }

    private void Update()
    {
        if (aimOn)
            DrawShootingLine();
    }

    public void SetAimOnOff()
    {
        if (!aimOn)
            aimOn = true;
        else
            aimOn = false;
    }

    private void DrawShootingLine() //Draws line and checks collisions
    {
        Ray ray = player.mainCamera.ScreenPointToRay(Input.mousePosition);

        if (Physics.Raycast(ray, out RaycastHit raycastHit))
        {
            shootingDirection = raycastHit.point - this.transform.position;

            if (raycastHit.collider.gameObject.CompareTag("AreaBounds")) 
            {
                Debug.DrawRay(bulletSpawner.transform.position, shootingDirection, Color.red);
            }
            else
            {
                Debug.DrawRay(bulletSpawner.transform.position, shootingDirection, Color.green);
            }

            if (currentWeapon.shooting == true && !CheckMouseOverUI())
            {
                //if (currentWeapon.shootingMode.ToString() == "Burst")
                //{
                //  if (Input.GetMouseButtonDown(0))
                //  {
                //      ShootWithCurrentWeapon();
                //  } 
                //}
                if (Input.GetMouseButton(0))
                {
                    ShootWithCurrentWeapon();
                }
            }
        }
    }

    private void ShootWithCurrentWeapon() // Checks if current weapoon has ammo in clip and shoots in way defined by weapon
    {
        if (currentWeapon.currentAmmo > 0)
        {
            switch (currentWeapon.shootingMode)
            {
                case WeaponDataSO.ShootingMode.Single:
                    //Debug.Log("Shooting mode is " + WeaponDataSO.ShootingMode.Single);
                    StartCoroutine(Shoot());
                    break;

                case WeaponDataSO.ShootingMode.Burst:
                    //Debug.Log("Shooting mode is " + WeaponDataSO.ShootingMode.Burst);
                    StartCoroutine(BurstFire());
                    break;

                case WeaponDataSO.ShootingMode.Auto:
                    Debug.Log("Shooting mode is " + WeaponDataSO.ShootingMode.Auto);
                    StartCoroutine(Shoot());
                    break;

                default:
                    Debug.Log("Shooting mode is default");
                    StartCoroutine(Shoot());
                    break;
            }
        }
        else
            StartCoroutine(ReloadWeapon());
    }

    private IEnumerator Shoot() // Starts coroutine for single fire
    {
        //RandomizeAccuracy();


        currentWeapon.shooting = false;
        GameObject bullet = currentBulletPooler.GetBulletFromPool();
        bullet.transform.position = bulletSpawner.transform.position;
        bullet.transform.rotation = Quaternion.identity;
        rb = bullet.GetComponent<Rigidbody>();
        //rb.velocity = (shootingDirection + randomizedAccuracy).normalized * currentWeapon.bulletSpeed;
        rb.AddRelativeForce(shootingDirection * currentWeapon.bulletSpeed);
        currentWeapon.currentAmmo--;
        gameManager.ammoInClipText.text = "Ammo in Clip " + currentWeapon.currentAmmo;
        if (currentWeapon.currentAmmo > 0 )
            yield return new WaitForSeconds(currentWeapon.fireRate);
        else
            StartCoroutine(ReloadWeapon());
        currentWeapon.shooting = true;
    }

    private IEnumerator BurstFire()  // Starts coroutine for burstfire
    {
        currentWeapon.shooting = false;
        ShootBurst();
        yield return new WaitForSeconds(currentWeapon.bulletFrequency);
        ShootBurst();
        yield return new WaitForSeconds(currentWeapon.bulletFrequency);
        ShootBurst();
        yield return new WaitForSeconds(currentWeapon.bulletFrequency);
        yield return new WaitForSeconds(currentWeapon.bulletFrequency);
        currentWeapon.shooting = true;
    } 

    private void ShootBurst() // Shoots a bullet during burstfire
    {
        //RandomizeAccuracy();
        GameObject bullet = currentBulletPooler.GetBulletFromPool();
        bullet.transform.position = bulletSpawner.transform.position;
        bullet.transform.rotation = Quaternion.identity;
        rb = bullet.GetComponent<Rigidbody>();
        rb.velocity = (shootingDirection + randomizedAccuracy).normalized * currentWeapon.bulletSpeed;
        rb.AddForce(shootingDirection * currentWeapon.bulletSpeed);
        currentWeapon.currentAmmo--;
        if (currentWeapon.currentAmmo == 0)
            StartCoroutine(ReloadWeapon());
        gameManager.ammoInClipText.text = "Ammo in Clip " + currentWeapon.currentAmmo;
    }

    public void ChangeWeapon()  // Changes current weapon to next in the weaponList and AmmoInUse to weapons bulletPrefab
    {
        weaponIndex++;
        poolerIndex++;
        if (weaponIndex > weaponList.Count - 1)
        {
            weaponIndex = 0;
            poolerIndex= 0;
            currentBulletPoolerObject = bulletPoolerList[poolerIndex];
            currentBulletPooler = currentBulletPoolerObject.GetComponent<BulletPooler>();   
            currentWeapon = (WeaponDataSO)weaponList[weaponIndex];
            //currentPooler = (PoolerDataSO)poolerList[poolerIndex];
        }
        else
        {
            currentBulletPoolerObject = bulletPoolerList[poolerIndex];
            currentBulletPooler = currentBulletPoolerObject.GetComponent<BulletPooler>();
            currentWeapon = (WeaponDataSO)weaponList[weaponIndex];
            //currentPooler = (PoolerDataSO)poolerList[poolerIndex];
        }

        gameManager.ChangeWeaponUIText();
        ammoInUse = currentBulletPooler.currentBulletPooler.bulletPrefab;
        currentWeapon.shooting = true;
    }

    private IEnumerator ReloadWeapon()   // Makes weapon unusable for time it needs to reload
    {
        Debug.Log("Started to Reload");
        currentWeapon.shooting = false;
        yield return new WaitForSeconds(currentWeapon.reloadTime);
        currentWeapon.currentAmmo = currentWeapon.magazineSize;
        gameManager.ammoInClipText.text = "Ammo in Clip " + currentWeapon.magazineSize;
        currentWeapon.shooting = true;
    }

    private bool CheckMouseOverUI()
    {
        return EventSystem.current.IsPointerOverGameObject();
    }

    private void SetBulletPoolers()
    {
        //Debug.Log("Setting bulletpoolers");
        switch (gameManager.spawnedCharacterIndex)
        {
            case 0:
                bulletPoolerList[0] = GameObject.FindWithTag("PistolBulletPooler");
                bulletPoolerList[1] = GameObject.FindWithTag("SubmachinegunBulletPooler");
                bulletPoolerList[2] = GameObject.FindWithTag("AssaultRifleBulletPooler");
                bulletPoolerList[3] = GameObject.FindWithTag("BazookaMissilePooler");
                break;

            case 1: 
                bulletPoolerList[0] = GameObject.FindWithTag("WolfPistolBulletPooler");
                bulletPoolerList[1] = GameObject.FindWithTag("WolfSubmachinegunBulletPooler");
                bulletPoolerList[2] = GameObject.FindWithTag("WolfAssaultRifleBulletPooler");
                bulletPoolerList[3] = GameObject.FindWithTag("WolfBazookaMissilePooler");
                break;

            default:
                Debug.Log("Setting bulletPoolers didn't work!");
                break;
        }
        gameManager.spawnedCharacterIndex++;
    }

    private void RandomizeAccuracy() // Randomizes minVector and maxVector numbers, these are used to randomize vector3 shootingDirection
    {
        /* TO DO
         * Add different minVector/maxVector variables for different weapons and for shooting while moving/jumping
        */

        var minVector = -20.0f;
        var maxVector = 20.0f;
        randomizedAccuracy = new Vector3(Random.Range(minVector, maxVector), Random.Range(minVector, maxVector), Random.Range(minVector, maxVector));
    }

    public void ToggleBulletSpawner()
    {
        if (bulletSpawner.gameObject.activeInHierarchy)
            bulletSpawner.gameObject.SetActive(false);
        else
            bulletSpawner.gameObject.SetActive(true);
    }
}