using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface IDamageable
{
    public int GetCurrentHealth();
    public int GetMaxHealth();
    public int GetInstanceID();
    public Transform GetTransform();
    public void DealDamage(int damage);

    public event Action<HealthUpdateParams> OnHealthUpdated;

    public struct HealthUpdateParams
    {
        public int PreviousHealth;
        public int CurrentHealth;
    }
}