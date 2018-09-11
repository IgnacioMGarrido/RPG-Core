using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;


namespace RPG.Characters
{
    public class SpecialAbilities : MonoBehaviour
    {
        const string ABILITY_ACTION_TRIGGER = "AbilityAction";
        const string DEFAULT_ABILITY = "DEFAULT ABILITY";

        [SerializeField] AbilityConfig[] abilities;
        [SerializeField] Image energyBar;
        [SerializeField] float maxEnergyPoints = 100f;
        [SerializeField] float regenPointsPerSecond = 10f;
        //TODO: Out of energy sound.

        float currentEnergyPoints;
        AudioSource audioSource;
        [Header("Animator")]
        [SerializeField] AnimatorOverrideController animatorOverrideController;
        Animator animator;

        private void Awake()
        {
            AttachAbilitiesToCharacter();
        }

        void Start()
        {
            energyBar = energyBar.GetComponent<Image>();
            audioSource = GetComponent<AudioSource>();
            currentEnergyPoints = maxEnergyPoints;
            animator = GetComponent<Animator>();
            UpdateEnergyBarImage();
        }
        private void Update()
        {
            if (currentEnergyPoints < maxEnergyPoints)
            {
                RegenEnergyPoints();
            }
        }
        private void AttachAbilitiesToCharacter()
        {
            foreach (AbilityConfig ability in abilities)
            {
                ability.AttachAbilityTo(gameObject);
            }
        }
        public void AttemptSpecialAbility(int abilityIndex, HealthSystem target, float amount)
        {
            if (IsEnergyAvailable(abilities[abilityIndex].GetEnergyCost()))
            {
                ConsumeEnergy(abilities[abilityIndex].GetEnergyCost());
                if (abilityIndex == 0)
                {
                    //float damageAmount = CalculateHitProbability(amount, target);
                    //TODO: remove AbilityUseParams.
                    abilities[abilityIndex].Use(target);
                }
                else
                {
                    abilities[abilityIndex].Use(target);

                }

                //Play Ability animation.
                if (abilities[abilityIndex].GetAbilityAnimation() != null)
                {
                    SetupAbilityAnimation(abilities[abilityIndex]);
                    animator.SetTrigger(ABILITY_ACTION_TRIGGER);
                }

            }
        }
        private void SetupAbilityAnimation(AbilityConfig ability)
        {
            animator.runtimeAnimatorController = animatorOverrideController;
            animatorOverrideController[DEFAULT_ABILITY] = ability.GetAbilityAnimation(); //remove const
        }

        public int GetNumOfAbilities()
        {
            return abilities.Length;
        }
        private void RegenEnergyPoints()
        {
            AddEnergyPoints();
            UpdateEnergyBarImage();
        }

        private void AddEnergyPoints()
        {
            var pointsToAdd = regenPointsPerSecond * Time.deltaTime;
            currentEnergyPoints = Mathf.Clamp(currentEnergyPoints + pointsToAdd, 0, maxEnergyPoints);
        }

        public bool IsEnergyAvailable(float amount) {
            return amount <= currentEnergyPoints;
        }

        public void ConsumeEnergy(float pointsPerHit)
        {
            float newEnergyPoints = currentEnergyPoints - pointsPerHit;
            currentEnergyPoints = Mathf.Clamp(newEnergyPoints, 0, maxEnergyPoints);
            UpdateEnergyBarImage();
        }

        private void UpdateEnergyBarImage()
        {
            energyBar.fillAmount = EnergyAsPercentage;//new Rect(xValue, 0f, 0.5f, 1f);
        }


        private float EnergyAsPercentage{ get { return currentEnergyPoints / (float)maxEnergyPoints; } }
    }
}
