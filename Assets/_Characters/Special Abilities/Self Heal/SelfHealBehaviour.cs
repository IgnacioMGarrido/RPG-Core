namespace RPG.Characters
{
    public class SelfHealBehaviour : AbilityBehaviour
    {

        Player player;
        private void Start()
        {
            player = GetComponent<Player>();
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
