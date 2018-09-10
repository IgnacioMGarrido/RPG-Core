namespace RPG.Characters
{
    public class SelfHealBehaviour : AbilityBehaviour
    {

        public override void Use(AbilityUseParams abilityUseParams)
        {
            HealTarget(abilityUseParams);
            PlayParticleEffect();
            PlayAbilitySound();
        }
        private void HealTarget(AbilityUseParams abilityUseParams)
        {
            abilityUseParams.target.TakeHeal(-(config as SelfHealConfig).GetHealAmount());
        }
    }
}
