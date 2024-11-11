using System;
using UnityEngine;

public class Damageable : MonoBehaviour
{
    [SerializeField] EnemyHealthBar healthBar;

    public float maxHealth = 100f;
    public float health = 100f;

    public event Action OnDeath;

    private void Awake()
    {
        healthBar = GetComponentInChildren<EnemyHealthBar>();
    }

    public void TakeDamage(float damage) 
    {
        if (health > 0)
        {
            health -= damage;
            healthBar.UpdateEnemyHealthBar(health, maxHealth);

            if (health <= 0)
            {
                OnDeath?.Invoke();
            }
        }
    }
}