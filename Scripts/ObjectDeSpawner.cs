using System.Collections.Generic;
using UnityEngine;

public class ObjectDeSpawner : MonoBehaviour
{
    private Dictionary<string, System.Action<GameObject>> poolerMethods;
    private List<string> ignoreTags;

    private void Awake()
    {
        poolerMethods = new Dictionary<string, System.Action<GameObject>>();
        ignoreTags = new List<string> { "Blackhole", "Laser", "Vacuum" };

        AddToDictionary<AntiMatterBlastPooler, AntiMatterBlast>("AntiMatterBlastPooler", "AntiMatterBlast");
        AddToDictionary<BeltAsteroidPooler, BeltAsteroid>("BeltAsteroidPooler", "BeltAsteroid");
        //AddToDictionary<BlackholePooler, Blackhole>("BlackholePooler", "Blackhole");
        AddToDictionary<EMPFieldPooler, EMPField>("EMPFieldPooler", "EMPField");
        AddToDictionary<EnemyAsteroidPooler, EnemyAsteroid>("EnemyAsteroidPooler", "SpaceDebris");
        AddToDictionary<EnemyPlasmaBulletPooler, PlasmaBullet>("EnemyProjectilePooler", "PlasmaBullet");
        AddToDictionary<EnemyPlasmaMissilePooler, PlasmaMissile>("EnemyProjectilePooler", "PlasmaMissile");
        AddToDictionary<EnemySimpleShipPooler, EnemySimpleShip>("EnemySimpleShipPooler", "EnemySimpleShip");
        AddToDictionary<EnemyLaserShipPooler, EnemyLaserShip>("EnemyLaserShipPooler", "EnemyLaserShip");
        AddToDictionary<PointCapsulePooler, PointCapsule>("PointCapsulePooler", "PointCapsule");
        AddToDictionary<StarbasePooler, Starbase>("EnemyStarbasePooler", "EnemyStarbase");
        AddToDictionary<UpgradeCapsulePooler, UpgradeCapsule>("UpgradeCapsulePooler", "UpgradeCapsule");
    }

    private void AddToDictionary<TPooler, TObject>(string tag, string key)
        where TPooler : MonoBehaviour, IPoolable<TObject>
        where TObject : MonoBehaviour
    {
        TPooler pooler = GameObject.FindWithTag(tag).GetComponent<TPooler>();
        if (pooler != null)
        {
            poolerMethods.Add(key, (obj) =>
            {
                TObject poolable = obj.GetComponent<TObject>();
                if (poolable != null)
                    pooler.ReturnToPool(poolable);
                else
                   Debug.LogWarning($"Object {obj.name} cannot be cast to {typeof(TObject)}"); 
            });
        }
        else
            Debug.LogWarning($"Pooler with tag {tag} and type {typeof(TPooler)} not found");
        
    }

    private void OnTriggerEnter2D(Collider2D col)
    {
        if (ignoreTags.Contains(col.gameObject.tag)) return;
        else
            TellPoolerToReturnObject(col.gameObject);
        
        //Debug.Log("Collision happened with: " + col.gameObject.tag);
    }

    public void TellPoolerToReturnObject(GameObject obj) 
    {
        if (ignoreTags.Contains(obj.gameObject.tag)) return;
        else
        {
            string key = obj.gameObject.tag;
            //Debug.Log($"Attempting to return object with tag {key} to pool.");
            if (poolerMethods.ContainsKey(key))                      
                poolerMethods[key](obj);            
            else
                Debug.LogWarning("No pooler method found for object: " + obj.gameObject.name);
        }
    }
}
