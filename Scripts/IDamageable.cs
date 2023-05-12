using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface IDamageable // Interface for objects to take damage when bullets hit them
{
    void TakeDamage(int damage);
}