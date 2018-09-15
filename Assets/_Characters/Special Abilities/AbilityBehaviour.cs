using UnityEngine;

namespace RPG.Characters
{
    public abstract class AbilityBehaviour : MonoBehaviour
    {
        const float PARTICLE_CLEANUP_DELAY = 8f;


        protected AbilityConfig config;
        protected AnimationClip animClip;

        public void SetConfig(AbilityConfig configToSet)
        {
            config = configToSet;
            animClip = config.GetAbilityAnimation();
        }
        protected void PlayParticleEffect()
        {
            if (config.GetParticlePrefab() != null)
            {
                GameObject go = Instantiate(config.GetParticlePrefab(), transform);
                go.transform.parent = null; //Maybe parent to the "Caster" and put the simulation space of the particle to world space.
                Destroy(go, PARTICLE_CLEANUP_DELAY);

            }
        }
        public abstract void Use(HealthSystem hs);

        protected void PlayAbilitySound()
        {
            var abilitySound = config.GetRandomAbilityAudioClip(); 
            var audioSource = GetComponent<AudioSource>();
            audioSource.clip = abilitySound;
            audioSource.Play();
        }
    }
}
