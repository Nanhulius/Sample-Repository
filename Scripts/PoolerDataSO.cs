using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Pooler", menuName = "Scriptable Objects/New Pooler")]
public class PoolerDataSO : ScriptableObject
{
    public GameObject bulletPrefab;
    public int bulletPoolSize;
}
