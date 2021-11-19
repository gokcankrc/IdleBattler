namespace Characters.Razor
{
    public class RazorUltimate : Ultimate
    {
        public AttackTypes attackType;
        public float dmgAtkScaling;


        public override void FinishCast()
        {
            return;
            // The ability finishes casting on Character side. Now effects apply.
            
        }


    }
}
