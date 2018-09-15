using UnityEngine;
using RPG.CameraUI;

namespace RPG.Characters
{
    public class PlayerControl : MonoBehaviour
    {

        EnemyAI target;
        RPGCursor rpgCursor;

        CharacterStats characterStats;
        Character playerCharacter;
        SpecialAbilities playerAbilities;
        WeaponSystem weaponSystem;
        void Start()
        {
            playerCharacter = GetComponent<Character>();
            playerAbilities = GetComponent<SpecialAbilities>();
            characterStats = GetComponent<CharacterStats>();
            weaponSystem = GetComponent<WeaponSystem>();

            RegisterForMouseEvents();

        }

        private void RegisterForMouseEvents()
        {
            rpgCursor = Camera.main.GetComponent<RPGCursor>();
            rpgCursor.onMouseOverEnemy += OnMouseOverEnemy;
            rpgCursor.onMouseOverPotentiallyWalkable += OnMouseOverPotentiallyWalkable;
        }
        void OnMouseOverPotentiallyWalkable(Vector3 destination)
        {
            if (Input.GetMouseButton(0))
            {
                playerCharacter.SetDestination(destination);
            }
        }
        void OnMouseOverEnemy(EnemyAI enemyToSet)
        {

            this.target = enemyToSet;
            HealthSystem enemyCharacterHealth = this.target.GetComponent<HealthSystem>();

            if (Input.GetMouseButton(0) && playerCharacter.IsTargetInRange(enemyCharacterHealth.GetComponent<Character>()))
            {
                weaponSystem.AttackTarget(enemyCharacterHealth);
            }
            else if (Input.GetMouseButtonDown(1) && playerCharacter.IsTargetInRange(enemyCharacterHealth.GetComponent<Character>()))
            {
                playerAbilities.AttemptSpecialAbility(0, enemyCharacterHealth, characterStats.GetDamage());
            }
        }

        // Update is called once per frame
        void Update()
        {
            //var healthAsPercentage = GetComponent<HealthSystem>().HealthAsPercentage;
            if (playerCharacter.GetIsAlive())
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


    }
}