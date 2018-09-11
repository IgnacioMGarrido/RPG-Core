using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

namespace RPG.Characters
{
    public class HealthSystem : MonoBehaviour
    {
        const string DEATH_TRIGGER = "Death";

        [SerializeField] float maxHealthPoints = 100f;
        [SerializeField] float currenthealthPoints = 0;
        [SerializeField] Image healthBar;
        [SerializeField] AudioClip[] hitSounds;
        [SerializeField] AudioClip[] deathSounds;
        [SerializeField] AudioClip[] healSounds;



        Animator animator;
        AudioSource audioSource;
        CharacterMovement characterMovement;
        float deathBanishSeconds = 3;

        // Use this for initialization
        void Start()
        {
            animator = GetComponent<Animator>();
            audioSource = GetComponent<AudioSource>();
            characterMovement = GetComponent<CharacterMovement>();
            InitializeCharacterHealthStats();
        }
        public void InitializeCharacterHealthStats()
        {
            CharacterStats characterStats = GetComponent<CharacterStats>();
            if (characterStats != null)
            {
                maxHealthPoints = characterStats.GetHealth();
                currenthealthPoints = maxHealthPoints;
            }
        }
        // Update is called once per frame
        void Update()
        {
            UpdateHealthBar();
        }

        private void UpdateHealthBar()
        {
            if (healthBar)
            {
                healthBar.fillAmount = HealthAsPercentage;
            }
        }
        public void TakeDamage(float damage)
        {
            bool characterDies = (currenthealthPoints - damage) <= 0;
            currenthealthPoints = Mathf.Clamp(currenthealthPoints - damage, 0f, maxHealthPoints);
            var clip = hitSounds[(int)UnityEngine.Random.Range(0, hitSounds.Length)];
            audioSource.PlayOneShot(clip);
            if (characterDies)
            {
                StartCoroutine(KillCharacter());
            }
        }

        public void Heal(float healPoints)
        {
            currenthealthPoints = Mathf.Clamp(currenthealthPoints + healPoints, 0f, maxHealthPoints);
            var clip = healSounds[(int)UnityEngine.Random.Range(0, healSounds.Length)];
            audioSource.PlayOneShot(clip);
            
        }
        public float HealthAsPercentage{ get { return currenthealthPoints / (float)maxHealthPoints; } }



        IEnumerator KillCharacter()
        {
            StopAllCoroutines();
            characterMovement.Kill();
            animator.SetTrigger(DEATH_TRIGGER);

            var playerComponent = GetComponent<Player>();
            if (playerComponent && playerComponent.isActiveAndEnabled)//If it is the player.
            {
                audioSource.clip = deathSounds[(int)UnityEngine.Random.Range(0, deathSounds.Length)];
                audioSource.Play();
                float duration = audioSource.clip.length > animator.GetCurrentAnimatorClipInfo(0).Length ? audioSource.clip.length : animator.GetCurrentAnimatorClipInfo(0).Length;
                yield return new WaitForSecondsRealtime(duration + 2); //Animation Length;
                SceneManager.LoadScene(0);
            }
            else //Assume is enemy for now Reconsiderd other NPCs.  
            {
                Destroy(gameObject, deathBanishSeconds);
            }

        }
    }
}
