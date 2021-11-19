using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Stats : MonoBehaviour
{
    // think this of non-monobehaviour class. this was easier to set it up like this so i did like this.
    // I don't even methods like TakeDamage() here.
    
    // Defensive
    public float maxHealth = 1000;
    public float health = 1000;
    public float defense = 100; // could be seen as % increase to health
    public float physicalResistance;
    public float elementalResistance;

    public float HealthFraction => health / maxHealth;
    
    // Offensive
    public float baseAttack = 400;
    public float attackMult = 50; // % increase to baseAttack
    public float flatAttack = 100; // added after attackMult

    public float Attack => baseAttack * (100 + attackMult) / 100 + flatAttack;
}
