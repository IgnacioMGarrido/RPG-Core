using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace RPG.Characters
{
    [CreateAssetMenu(menuName = ("RPG/Special Ability/Area of Effect"))]
    public class AoEConfig : SpecialAbility
    {
        [Header("Power Attack Specific ")]
        [SerializeField] float radius = 5f;
        [SerializeField] float damageToEachTarget = 15f;
        

        public override void AttachComponentTo(GameObject gameObjectToAttachTo)
        {

            var behaviourComponent = gameObjectToAttachTo.AddComponent<AoEBehaviour>();
            behaviourComponent.SetConfig(this);
            behaviour = behaviourComponent;
        }
        public float GetRadius() {
            return radius;
        }
        public void SetRadiusModifier(float abilityRadiusModifier) {
            this.radius *= abilityRadiusModifier;
        }
        public float GetDamageToEachTarget()
        {
            return damageToEachTarget;
        }
    }
}
