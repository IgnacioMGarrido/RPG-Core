using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using RPG.Characters;
namespace RPG.Characters {
    [ExecuteInEditMode]
    public class WeaponPickupPoint : MonoBehaviour {

        [SerializeField] Weapon weaponConfig;
        [SerializeField] AudioClip weapongPickUpSoundEffect;
        bool hasPlayed = false;

        private void Update()
        {
            if (!Application.isPlaying)
            {
                DestroyChildren();
                InstantiateWeapon();
            }
        }

        private void DestroyChildren()
        {
            foreach (Transform child in transform)
            {
                DestroyImmediate(child.gameObject);
            }
        }

        private void InstantiateWeapon()
        {
            var weapon = weaponConfig.WeaponPrefab;
            weapon.transform.position = Vector3.zero;
            Instantiate(weapon, gameObject.transform);
        }

        private void OnTriggerEnter(Collider other)
        {
            if (other.gameObject.tag == "Player" && hasPlayed == false)
            {
                hasPlayed = true;
                Player player = FindObjectOfType<Player>();
                player.PutWeaponInHand(weaponConfig);
                var audioSource = player.GetComponent<AudioSource>();
                if (audioSource.isPlaying == false)
                { 
                    audioSource.PlayOneShot(weapongPickUpSoundEffect);
                }
                Destroy(this.gameObject);
            }
        }
    }
}
