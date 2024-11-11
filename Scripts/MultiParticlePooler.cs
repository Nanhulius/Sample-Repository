using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MultiParticlePooler : MonoBehaviour
{
    [System.Serializable]
    public class ParticlePool
    {
        public string poolName;
        public ParticleSystem particlePrefab;
        public int poolSize;
    }

    public List<ParticlePool> particlePools;
    private Dictionary<string, Queue<ParticleSystem>> poolDictionary;

    [SerializeField] private Transform parentObject;

    private void Start()
    {
        InitializePool();
    }

    private void InitializePool()
    {
        poolDictionary = new Dictionary<string, Queue<ParticleSystem>>();

        foreach (ParticlePool pool in particlePools)
        {
            Queue<ParticleSystem> particleQueue = new Queue<ParticleSystem>();

            for (int i = 0; i < pool.poolSize; i++)
            {
                ParticleSystem newParticle = Instantiate(pool.particlePrefab);
                newParticle.gameObject.SetActive(false);
                particleQueue.Enqueue(newParticle);
                newParticle.transform.SetParent(parentObject);
            }

            poolDictionary.Add(pool.poolName, particleQueue);
        }
    }

    public void SpawnParticles(string poolName, Vector3 spawnPosition)
    {
        if (poolDictionary.ContainsKey(poolName))
        {
            ParticleSystem particleSystem = poolDictionary[poolName].Dequeue();

            if (particleSystem != null)
            {
                particleSystem.transform.position = spawnPosition;
                particleSystem.gameObject.SetActive(true);
                particleSystem.Play();

                StartCoroutine(WaitForParticlesToFinish(particleSystem, poolName));
            }
        }
        else
            Debug.LogWarning($"No Particles available in pool: {poolName}");    
    }

    public IEnumerator WaitForParticlesToFinish(ParticleSystem particleSystem, string poolName)
    {
        yield return new WaitUntil(() => !particleSystem.isPlaying);
        particleSystem.gameObject.SetActive(false);
        poolDictionary[poolName].Enqueue(particleSystem);
    }
}
