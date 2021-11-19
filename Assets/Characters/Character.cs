using System;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using static Ability;
using Random = UnityEngine.Random;


public class Character : MonoBehaviour
{
    public enum CharacterState
    {
        Idle, Casting, Dead
    }
    public CharacterState state = CharacterState.Idle;

    // 1,2,3 are enemy positions, 1 being the front. -1,-2,-3 are ally positions, -1 being the front.
    public int bgPosition; // -1,-2,-3 etc. for player side and 1,2,3 for enemy side

    [NonSerialized] public Stats Stats;
    [SerializeField] private HealthBar myHealthBar;
    [SerializeField] private SpriteRenderer myCharacterSprite;
    private Passive _passive;
    private Attack _attack;
    private Skill _skill;
    private Ultimate _ultimate;

    private void Awake()
    {
        Stats = GetComponent<Stats>();
        _passive = GetComponent<Passive>();
        _attack = GetComponent<Attack>();
        _skill = GetComponent<Skill>();
        _ultimate = GetComponent<Ultimate>();
        
    }

    private void Start()
    {
        if (bgPosition > 0) myCharacterSprite.transform.localScale = new Vector3(-1, 1, 1);
    }

    public void TakeDamage(float attackValue, AttackTypes attackType)
    {
        // Defense calculations

        attackValue = attackValue * (100 / (100 + Stats.defense));
        Stats.health -= attackValue;
        
        // update health bar
        myHealthBar.UpdateHealthBarSize(Stats.HealthFraction);
        
        // todo; popups
        // steal the locksmith's popups
    }

    public void TakeHeal(float healValue)
    {
        // update Stats
        Stats.health += healValue;
        if (Stats.health > Stats.maxHealth)
        {
            Stats.health = Stats.maxHealth;
        }
        
        // update health bar
        myHealthBar.UpdateHealthBarSize(Stats.HealthFraction);
    }

    public Ability[] CastableAbilities
    {
        get
        {
            // ORDER MATTERS
            Ability[] abilities = {_ultimate, _skill, _attack};
            return abilities;
        }
    }
    
    private void AbilityCooldownsTick()
    {
        // Time passses for cooldowns.
        foreach (var ability in CastableAbilities)
        {
            if (ability == null) continue;
            if (ability.cooldown >= 0)
            {
                ability.cooldown -= 1 * Time.deltaTime;
            }
        }
    }

    private void AbilityCastAnyReady()
    {
        // Abilities are "Execute"'d if their CD is ready.
        foreach (var ability in CastableAbilities)
        {
            if (ability == null) continue;
            if (ability.cooldown <= 0)
            {
                IdleToCasting(ability);
                
                // Return because we don't want to cast two things at once
                return;
            }
        }
    }

    private void IdleToCasting(Ability ability)  // Aka. "ExecuteAbility"
    {
        // Switches state from idle to casting.
        // Happens if a skill is executed.
        
        // set cd
        ability.cooldown = ability.cooldownMax;
        
        // If casttime is less than 0, ability is cast in an instant.
        if (ability.castTime < 0) return;
        
        // switch to casting loadbar, sort of. like windup time or whatever.
        state = CharacterState.Casting;
        _currentCastingAbility = ability;
        _castingMax = ability.castTime;
        _castingRemainingSec = _castingMax;
    }
    
    private void Die()
    {
        state = CharacterState.Dead;
        
        // remove from bg
        BattleGround.I.RemoveCharacterFromBg(this);
        myCharacterSprite.color = Color.red;
    }

    private void Revive()
    {
        state = CharacterState.Idle;
        
        BattleGround.I.AddCharacterToBg(this);
        myCharacterSprite.color = Color.white;
    }
    
    // todo; method variables that stay there? I could just send the castingMax and method would do things in itself
    [SerializeField] private Ability _currentCastingAbility;
    [SerializeField] private float _castingRemainingSec;
    [SerializeField] private float _castingMax;  // To draw GUI
    

    // Update is called once per frame
    void FixedUpdate()
    {

        void IdleUpdate()
        {
            AbilityCooldownsTick();
            if (Stats.health < 0)
            {
                Die();
                return;
            }
            AbilityCastAnyReady();
        }
        
        void CastingUpdate()
        {
            AbilityCooldownsTick();

            _castingRemainingSec -= Time.fixedDeltaTime;
            if (_castingRemainingSec <= 0)
            {
                // ability cast has finished, tell ability to actually cast itself. Which is the method provided by ability.
                _currentCastingAbility.FinishCast();
                
                state = CharacterState.Idle;
            }
            
            // todo; casting loadbar or whatever. also names what we are doing.
            

            if (Stats.health < 0)
            {
                Die();
                return;
            }

        }

        void DeadUpdate()
        {
            AbilityCooldownsTick();
        }
        
        // State Machine
        switch (state)
        {
            case CharacterState.Idle:
                IdleUpdate();
                break;
            case CharacterState.Casting:
                CastingUpdate();
                break;
            case CharacterState.Dead:
                DeadUpdate();
                break;
        }
    }


}
