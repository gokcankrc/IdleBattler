using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Runtime.CompilerServices;
using UnityEditor.Experimental.GraphView;
using UnityEngine;
using Debug = UnityEngine.Debug;

public class BattleGround : Singleton<BattleGround>
{
    private Character _activeCharacter;
    
    private List<Character> _enemySideCharacters = new List<Character>();
    private List<Character> _allySideCharacters  = new List<Character>();
    private List<int> _enemyPositions = new List<int>();
    private List<int> _allyPositions  = new List<int>();
    // private List<Character> _noneSideCharacters = new List<Character>();
    
    // easier way to get all characters from both sides. this is probably not useful, but anyways.
    private List<Character> EveryCharacter
    {
        get
        {
            List<Character> everyCharacters = new List<Character>();
            everyCharacters.AddRange(_allySideCharacters);
            everyCharacters.AddRange(_enemySideCharacters);
            return everyCharacters;
        }
    }

    private void Start()
    {
        GetTargets(TargetTypes.error123213, "right");
    }

    private static readonly System.Random Rnd = new System.Random();

    public void RemoveCharacterFromBg(Character characterToRemove)
    {
        if (characterToRemove.bgPosition > 0)
        {
            _enemySideCharacters.Remove(characterToRemove);
            _enemyPositions.Remove(characterToRemove.bgPosition);
            if (_enemyPositions.Count == 0) FinishBattle();
        }
        else if (characterToRemove.bgPosition < 0)
        {
            _allySideCharacters.Remove(characterToRemove);
            _allyPositions.Remove(characterToRemove.bgPosition);
            if (_enemyPositions.Count == 0) FinishBattle();
        }
        else throw new Exception();
    }

    public void AddCharacterToBg(Character characterToAdd)
    {
        if (characterToAdd.bgPosition > 0)
        {
            _enemySideCharacters.Add(characterToAdd);
            _enemyPositions.Add(characterToAdd.bgPosition);
        }
        else if (characterToAdd.bgPosition < 0)
        {
            _allySideCharacters.Add(characterToAdd);
            _allyPositions.Add(characterToAdd.bgPosition);
        }
        else throw new Exception();
    }

    private void FinishBattle()
    {
        return;
        throw new NotImplementedException();
    }

    private void Awake()
    {
        // Lets gather all the active characters into their separate lists.
        foreach (var character in GetComponentsInChildren<Character>())
        {
            AddCharacterToBg(character);
        }
        // Sort all by battleground position
        _enemySideCharacters.Sort((x, y) => x.bgPosition.CompareTo(y.bgPosition));
        _allySideCharacters.Sort( (x, y) => x.bgPosition.CompareTo(y.bgPosition));
        _enemyPositions = _enemySideCharacters.Select(c =>  c.bgPosition).ToList();
        _allyPositions  = _allySideCharacters.Select( c =>  c.bgPosition).ToList();
    }

    public Character[] GetTargets(TargetTypes targetType, string side)
    {
        List<Character> eligibleCharacters = new List<Character>(); 
        // List<int> eligibleCharacterPositions = new List<int>();
        
        // Check if we have any sides empty. If empty, the battle ends.
        if (_enemyPositions.Count == 0 || _allyPositions.Count == 0)
        {
            FinishBattle();
            // if empty array is returned, they do not attack but the CD kicks in anyway. it's like attacking when no enemy is around.
            return new Character[0];
        }

        
        // if side is +1 instead of -1, we switch the target. this is much easier to implement for short term.
        // only accepts "right" or "left"
        if (side == "right")
        {
            TargetTypes SwitchTargetSide(TargetTypes targetToSwitch)
            {
                switch (targetToSwitch)
                {
                    case TargetTypes.EnemyFrontArea:
                        targetToSwitch = TargetTypes.AllyFrontArea;
                        break;
                    case TargetTypes.EnemyFrontRandom:
                        targetToSwitch = TargetTypes.AllyFrontRandom;
                        break;
                    case TargetTypes.EnemyBack:
                        targetToSwitch = TargetTypes.AllyBack;
                        break;
                    case TargetTypes.EnemyBackRandom:
                        targetToSwitch = TargetTypes.AllyBackRandom;
                        break;
                    case TargetTypes.EnemyAll:
                        targetToSwitch = TargetTypes.AllyAll;
                        break;
                    case TargetTypes.EnemyAllRandom:
                        targetToSwitch = TargetTypes.AllyAllRandom;
                        break;
                }

                return targetToSwitch;
            }
            targetType = SwitchTargetSide(targetType);
        }
        else if (side != "left" && side != "none") throw new Exception("side is: " + side);

            int minPos, maxPos;
        switch (targetType)
        {
            case TargetTypes.Self:
                // ITS YOU RETARD
                // Also i don't have a way of telling you that you are you.
                throw new Exception();
            case TargetTypes.EnemyFrontArea:
            case TargetTypes.EnemyFrontRandom:
                // add all enemies to eligibles if they are positioned in the front
                minPos = _enemyPositions.Min();
                eligibleCharacters.AddRange(_enemySideCharacters.Where(character => character.bgPosition == minPos));
                break;
            case TargetTypes.EnemyBack:
            case TargetTypes.EnemyBackRandom:
                maxPos = _enemyPositions.Max();
                eligibleCharacters.AddRange(_enemySideCharacters.Where(character => character.bgPosition == maxPos));
                break;
            case TargetTypes.EnemyAll:
            case TargetTypes.EnemyAllRandom:
                eligibleCharacters.AddRange(_enemySideCharacters);
                break;
            case TargetTypes.AllyFrontArea:
            case TargetTypes.AllyFrontRandom:
                // add all allies to eligibles if they are positioned in the front, which is the highest number for allies
                minPos = _allyPositions.Max();
                eligibleCharacters.AddRange(_allySideCharacters.Where(character => character.bgPosition == minPos));
                break;
            case TargetTypes.AllyBack:
            case TargetTypes.AllyBackRandom:
                maxPos = _allyPositions.Max();
                eligibleCharacters.AddRange(_allySideCharacters.Where(character => character.bgPosition == maxPos));
                break;
            case TargetTypes.AllyAll:
            case TargetTypes.AllyAllRandom:
                eligibleCharacters.AddRange(_allySideCharacters);
                break;
            default:
                // If code reaches here, there is an unhandled target in the code above.
                throw new ArgumentOutOfRangeException(nameof(targetType), targetType, null);
                Debug.Assert(true);
        }
        
        // if target is random, randomly pick one from the list.
        var listOfRandomTargetTypes = new[]
        {
            TargetTypes.AllyFrontRandom, TargetTypes.AllyBackRandom, TargetTypes.AllyAllRandom,
            TargetTypes.EnemyFrontRandom, TargetTypes.EnemyBackRandom, TargetTypes.EnemyAllRandom
        };
        if (listOfRandomTargetTypes.Contains(targetType))
        {
            var randomCharacter = eligibleCharacters[Rnd.Next(eligibleCharacters.Count)];
            eligibleCharacters.Clear();
            eligibleCharacters.Add(randomCharacter);
        }

        return eligibleCharacters.ToArray();
    }


}
