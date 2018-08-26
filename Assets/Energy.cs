using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using RPG.CameraUI;

namespace RPG.Characters
{
    public class Energy : MonoBehaviour
    {
        [SerializeField] RawImage energyBar;
        [SerializeField] float maxEnergyPoints = 100f;
        [SerializeField] float pointsPerHit = 10;

       // private int enemyLayer = 10;
        private RPGCursor cameraRaycaster;
        float currentEnergyPoints;
        // Use this for initialization
        void Start()
        {
            energyBar = energyBar.GetComponent<RawImage>();
            currentEnergyPoints = maxEnergyPoints;
            cameraRaycaster = Camera.main.GetComponent<RPGCursor>();
            cameraRaycaster.onMouseOverEnemy+= OnMouseOverEnemy;
        }
        void ProcessRightClick(RaycastHit raycastHit, int layerHit)
        {


        }
        void OnMouseOverEnemy(Enemy enemy) {
            if (Input.GetMouseButtonDown(1))
            { 
                float newEnergyPoints = currentEnergyPoints - pointsPerHit;
                currentEnergyPoints = Mathf.Clamp(newEnergyPoints, 0, maxEnergyPoints);
                UpdateEnergyBar();
            }
        }
        private void UpdateEnergyBar()
        {
            float xValue = -(EnergyAsPercentage / 2f) - 0.5f;
            energyBar.uvRect = new Rect(xValue, 0f, 0.5f, 1f);
        }

        public float EnergyAsPercentage
        {
            get
            {
                return currentEnergyPoints / (float)maxEnergyPoints;
            }

        }
    }
}
