using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.Assertions;
using UnityEngine.SceneManagement;

using RPG.CameraUI;

namespace RPG.Characters
{
    public class PlayerControl : MonoBehaviour
    {

        Enemy target;
        RPGCursor rpgCursor;

        CharacterStats characterStats;
        Character playerCharacter;
        SpecialAbilities playerAbilities;
          
        void Start()
        {
            playerCharacter = GetComponent<Character>();
            playerAbilities = GetComponent<SpecialAbilities>();
            characterStats = GetComponent<CharacterStats>();

            NotifyListeners();

        }

        private void NotifyListeners()
        {
            rpgCursor = Camera.main.GetComponent<RPGCursor>();
            rpgCursor.onMouseOverPotentiallyWalkable += OnMouseOverPotentiallyWalkable;
            rpgCursor.onMouseOverEnemy += OnMouseOverEnemy;
        }

        // Update is called once per frame
        void Update()
        {
            var healthAsPercentage = GetComponent<HealthSystem>().HealthAsPercentage;
            if (healthAsPercentage > Mathf.Epsilon)
            {
                ScanForAbilityKey();
            }
        }

        //TODO: Move Input Manager to self class.
        private void ScanForAbilityKey()
        {
            //healing
            if (Input.GetKeyDown(KeyCode.Alpha1)) //Self Healing
            {
                playerAbilities.AttemptSpecialAbility(1, GetComponent<HealthSystem>(), characterStats.GetHealing()); 
            }

            if (Input.GetKeyDown(KeyCode.Alpha2))// AoEDamage
            {
                playerAbilities.AttemptSpecialAbility(2, GetComponent<HealthSystem>(), characterStats.GetDamage());

            }
        }

        void OnMouseOverPotentiallyWalkable(Vector3 destination)
        {
             if (Input.GetMouseButton(0))
             {
                 playerCharacter.SetDestination(destination);
             }
        }
        void OnMouseOverEnemy(Enemy enemyToSet) {

            this.target = enemyToSet;
            HealthSystem enemyCharacter = this.target.GetComponent<HealthSystem>();

            if (Input.GetMouseButton(0) && GetComponent<Character>().IsTargetInRange(enemyCharacter.GetComponent<Character>()))
            {
                GetComponent<Character>().AttackTarget(enemyCharacter);
            }
            else if (Input.GetMouseButtonDown(1) && GetComponent<Character>().IsTargetInRange(enemyCharacter.GetComponent<Character>()))
            {
                playerAbilities.AttemptSpecialAbility(0, this.target.GetComponent<HealthSystem>(), characterStats.GetDamage());
            }
        }

    }
}