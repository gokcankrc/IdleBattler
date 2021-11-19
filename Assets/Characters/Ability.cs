using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum TargetTypes
{
    Self, 
    EnemyFrontArea, EnemyFrontRandom, EnemyBack, EnemyBackRandom, EnemyAll, EnemyAllRandom,
    AllyFrontArea, AllyFrontRandom, AllyBack, AllyBackRandom, AllyAll, AllyAllRandom
}

public enum AttackTypes
{
    Physical, Pyro, Electro, Hydro, Cryo, Geo
}

public abstract class Ability : MonoBehaviour
{
    protected string sideStringCuzImLazy;
    // Abilities do NOT control themselves. They are controlled by Character. So the cooldown is set and iterated by character.
    public float cooldownMax;
    public float cooldown;
    public float castTime;
    public TargetTypes targetType;
    protected Character Host;

    private void Start()
    {
        Host = GetComponentInParent<Character>();

        // gotta do this do automate "right" and "left" stuff
        if (Host.bgPosition > 0)
        {
            sideStringCuzImLazy = "right";
        }
        else if (Host.bgPosition < 0)
        {
            sideStringCuzImLazy = "left";
        }
        else
        {
            sideStringCuzImLazy = "none";
        }
    }

    // Execute is called when abilities are cast. callback is
    // todo; what was callback gonna be?
    // public abstract Action Execute(Action callback);
    public abstract void FinishCast();

    protected static float CalculateAttackValue(Character host, float dmgScaling)
    {
        float attackValue = host.Stats.Attack;
        attackValue *= dmgScaling / 100;
        return attackValue;
    }

}

public abstract class Passive : Ability 
{
}

public abstract class Attack : Ability 
{
    // baseline for all attacks
    public AttackTypes attackType = AttackTypes.Physical;
    public float dmgAtkScaling = 80;

    public override void FinishCast()
    {
        // The ability finishes casting on Character side. Now effects apply.
        
        // Get the target
        var targets = BattleGround.I.GetTargets(targetType, sideStringCuzImLazy);
        if (targets.Length == 0) return;
        var target = targets[0];
        
        // Do the attack
        var attackValue = Host.Stats.Attack * (100 + dmgAtkScaling) / 100;
        target.TakeDamage(attackValue, attackType);
    }
}

public abstract class Skill : Ability 
{ 

}

public abstract class Ultimate : Ability 
{ 

}
    


