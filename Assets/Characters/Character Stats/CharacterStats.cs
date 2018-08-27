using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace RPG.Characters
{
    public class CharacterStats : MonoBehaviour
    {
        private enum Operation
        {
            Addition,
            Substraction
        }
        const int LEVEL_BASE = 10;
        [SerializeField] int level = 1;
        // Attributes
        [SerializeField] int might = 10;
        [SerializeField] int constitution = 10;
        [SerializeField] int dexterity = 10;
        [SerializeField] int perception = 10;
        [SerializeField] int intellect = 10;
        [SerializeField] int resolve = 10;

        /*                         (Default)
         *                      |   Fighter   |   Barbarian   |   Cipher   |   Priest   |   Ranger   |       
            Base Deflection           25             15             20           20           20
            Base Fortitude            20             30             20           20           20
            Base reflex               20             20             15           20           20
            Base Will                 15             15             30           25           20
        */
        // --- Base Defenses ---
        const float baseDeflection = 25;
        const float baseFortitude = 20;
        const float basereflex = 20;
        const float baseWill = 15;
        //Increases
        const float defenseIncreasePerPoint = 2;

        // --- Base Action Stats ---
        const float baseAccuracy = 20;
        const float baseDamage = 10;
        const float baseHealing = 10;
        const float baseActionSpeed = 1;
        const float baseAoEIncrease = 0;
        const float baseDuration = 0;

        // --- Base Passive Stats ---
        const float baseHealth = 42;
        const float healthPerLevel = 12;

        [Header("Defenses")]
        // -- Defenses --
        [SerializeField] float deflection = 0;
        [SerializeField] float fortitude = 0;
        [SerializeField] float reflex = 0;
        [SerializeField] float will = 0;

        [Header("Action Stats")]
        // -- Action Stats -- 
        [SerializeField] float accuracy = 0;
        [SerializeField] float damage = 0;
        [SerializeField] float healing = 0;
        [SerializeField] float actionSpeed = 0;
        [SerializeField] float AoEIncrease = 0;
        [SerializeField] float duration = 0;

        [Header("Passive Stats")]
        // -- Passive Stats -- 
        [SerializeField] float health = 0;


        private void Awake()
        {
            CalculateDefenses();

            CalculatePassiveStats();

            CalculateActionStats();

        }

        private void CalculatePassiveStats()
        {
            health = baseHealth + healthPerLevel * level;
        }

        private void CalculateDefenses()
        {
            deflection = CalculateStatLinear(baseDeflection, resolve, 1);
            fortitude = CalculateStatLinear(baseFortitude, might, 2) + CalculateStatLinear(baseFortitude, constitution, 2);
            reflex = CalculateStatLinear(basereflex, dexterity, 2) + CalculateStatLinear(basereflex, perception, 2);
            will = CalculateStatLinear(baseWill, intellect, 2) + CalculateStatLinear(baseWill, resolve, 2);
        }
        private void CalculateActionStats()
        {
            accuracy = CalculateStatLinear(baseAccuracy, perception, 1);
            damage = CalculateStatPercentage(baseDamage, might, 0.03f);
            healing = CalculateStatPercentage(baseHealing, might, 0.03f);
            actionSpeed = CalculateStatPercentage(baseActionSpeed, dexterity, -0.03f);
            AoEIncrease = 0.1f * intellect;
            duration = 0.05f * intellect;
        }

        private float CalculateStatPercentage(float baseStat, float attribute, float increasePePointpercentage)
        {
            return baseStat + baseStat * (increasePePointpercentage * (attribute - LEVEL_BASE));
        }


        private float CalculateStatLinear(float baseStat, float attribute, float increasePePoint)
        {
            return baseStat + (attribute - LEVEL_BASE) * increasePePoint;
        }

        public float GetDamage()
        {
            return damage;
        }

        public float GetHealth()
        {
            return health;
        }

        public float GetAccuracy()
        {
            return accuracy;
        }
        public float GetDeflection() {
            return deflection;
        }

        public float GetHealing()
        {
            return healing;
        }
        public float GetActionSpeed()
        {
            return actionSpeed;
        }
        public void SetActionSpeed(float percentage)
        {
            actionSpeed += actionSpeed * percentage;
        }
        public void SetDamage(float percentage) {
            damage += baseDamage * percentage;
        }

    }
}