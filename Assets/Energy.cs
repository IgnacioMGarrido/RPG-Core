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

        private int enemyLayer = 10;
        private CameraRaycaster cameraRaycaster;
        float currentEnergyPoints;
        bool isChanged = false;
        // Use this for initialization
        void Start()
        {
            energyBar = energyBar.GetComponent<RawImage>();
            print(energyBar.name);
            currentEnergyPoints = maxEnergyPoints;
            cameraRaycaster = Camera.main.GetComponent<CameraRaycaster>();
            cameraRaycaster.notifyRightMouseClickObservers += ProcessRightClick;
        }
        private void Update()
        {
           // if (isChanged) {
           // }
            isChanged = false;
        }
        void ProcessRightClick(RaycastHit raycastHit, int layerHit)
        {
            Debug.Log("Notified");
            isChanged = true;
            float newEnergyPoints = currentEnergyPoints - pointsPerHit;
            currentEnergyPoints = Mathf.Clamp(newEnergyPoints, 0, maxEnergyPoints);
            UpdateEnergyBar();


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
