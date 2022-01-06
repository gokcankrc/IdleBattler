using System.Collections.Generic;

namespace Characters.Razor
{
    public class RazorSkill : Skill
    {
        public AttackTypes attackType = AttackTypes.Electro; // WTF GÖKCAN
        
        
        public float dmgAtkScaling = 150;

        // todo;
        // How would i save setup of this skill in a way that it won't fuckup if these change? If prefab fucks up?
        // I don't like when prefab holds valuable balance info
        // A dumb way is to have an excel. I'd rather hardcode tho.

        public override void FinishCast()
        {
            // The ability finishes casting on Character side. Now effects apply.
        
            // Get the target
            var targets = BattleGround.I.GetTargets(targetType, sideStringCuzImLazy);
            if (targets.Length == 0) return;
            var target = targets[0];
        
            // Do the attack
            var attackValue = Host.Stats.Attack * (100 + dmgAtkScaling) / 100;
            target.TakeDamage(attackValue, this.attackType);
        }
    }
}
