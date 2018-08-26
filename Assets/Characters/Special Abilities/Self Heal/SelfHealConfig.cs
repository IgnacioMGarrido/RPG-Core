using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace RPG.Characters
{
    [CreateAssetMenu(menuName = ("RPG/Special Ability/Self heal"))]
    public class SelfHealConfig : SpecialAbility
    {
        [Header("Power Attack Specific ")]
        [SerializeField]float healAmount = 10f;

        public override void AttachComponentTo(GameObject gameObjectToAttachTo)
        {

            var behaviourComponent = gameObjectToAttachTo.AddComponent<SelfHealBehaviour>();
            behaviourComponent.SetConfig(this);
            behaviour = behaviourComponent;
        }

        public float GetHealAmount()
        {
            return healAmount;
        }
    }
}
