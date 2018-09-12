namespace RPG.Characters
{
    public class SelfHealBehaviour : AbilityBehaviour
    {

        PlayerControl player;
        private void Start()
        {
            player = GetComponent<PlayerControl>();
        }
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
