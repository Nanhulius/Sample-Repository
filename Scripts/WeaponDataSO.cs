using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Weapon", menuName = "Scriptable Objects/new Weapon")]
public class WeaponDataSO : ScriptableObject
{
    public enum ShootingMode { Single, Burst, Auto }

    [SerializeField] public string weaponName;
    [SerializeField] public ShootingMode shootingMode;

    [SerializeField]
    public float damage,
                 bulletSpeed,
                 bulletFrequency,
                 fireRate,
                 reloadTime;

    [SerializeField]
    public int magazineSize,
               currentAmmo;
               //bulletAmount,
               //maxBulletAmount;

    [HideInInspector] public bool shooting;

    private void OnEnable()
    {
        currentAmmo = magazineSize;
    }
}
