using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace RPG.Characters
{
    [CreateAssetMenu(menuName = ("RPG/Special Ability/Area of Effect"))]
    public class AoEConfig : AbilityConfig
    {
        [Header("AoE Specific ")]
        [SerializeField] float radius = 5f;
        [SerializeField] float damageToEachTarget = 15f;

        public override AbilityBehaviour GetBehaviourComponent(GameObject objectToAttachTo)
        {
            return objectToAttachTo.AddComponent<AoEBehaviour>();
        }
        public float GetRadius()
        {
            return radius;
        }
        public void SetRadiusModifier(float abilityRadiusModifier)
        {
            this.radius *= abilityRadiusModifier;
        }
        public float GetDamageToEachTarget()
        {
            return damageToEachTarget;
        }
    }
}
