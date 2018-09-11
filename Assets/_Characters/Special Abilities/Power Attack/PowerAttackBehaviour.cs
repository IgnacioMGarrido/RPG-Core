namespace RPG.Characters
{
    public class PowerAttackBehaviour : AbilityBehaviour
    {
        public override void Use(HealthSystem hs)
        {
            DealDamage(hs);
            PlayParticleEffect();
            PlayAbilitySound();
        }
        private void DealDamage(HealthSystem hs)
        {
            hs.TakeDamage((config as PowerAttackConfig).GetExtraDamage());
        }
    }
}
