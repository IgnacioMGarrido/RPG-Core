namespace RPG.Characters
{
    public class SelfHealBehaviour : AbilityBehaviour
    {

        Player player;
        private void Start()
        {
            player = GetComponent<Player>();
        }
        public override void Use(AbilityUseParams abilityUseParams)
        {
            HealTarget(abilityUseParams);
            PlayParticleEffect();
            PlayAbilitySound();
        }
        private void HealTarget(AbilityUseParams abilityUseParams)
        {
            var playerHealth = player.GetComponent<HealthSystem>();
            playerHealth.Heal((config as SelfHealConfig).GetHealAmount());
        }
    }
}
