namespace RPG.Characters
{
    public class PowerAttackBehaviour : AbilityBehaviour
    {
        public override void Use(AbilityUseParams abilityUseParams)
        {
            DealDamage(abilityUseParams);
            PlayParticleEffect();
            PlayAbilitySound();
        }
        private void DealDamage(AbilityUseParams abilityUseParams)
        {
            abilityUseParams.target.TakeDamage(abilityUseParams.baseDamage + (config as PowerAttackConfig).GetExtraDamage());
        }
    }
}
