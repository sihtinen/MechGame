using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface IDamageable
{
    public int GetInstanceID();
    public int DealDamage(int damage);
}