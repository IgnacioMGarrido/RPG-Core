namespace RPG.Characters
{
    public class SelfHealBehaviour : AbilityBehaviour
    {

        public override void Use(HealthSystem hs)
        {
            HealTarget(hs);
            PlayParticleEffect();
            PlayAbilitySound();
        }
        private void HealTarget(HealthSystem hs)
        {
            hs.Heal((config as SelfHealConfig).GetHealAmount());
        }
    }
}
