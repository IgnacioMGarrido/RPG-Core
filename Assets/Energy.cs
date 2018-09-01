using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;


namespace RPG.Characters
{
    public class Energy : MonoBehaviour
    {
        [SerializeField] Image energyBar;
        [SerializeField] float maxEnergyPoints = 100f;
        [SerializeField] float regenPointsPerSecond = 10f;

       // private int enemyLayer = 10;
        float currentEnergyPoints;
        // Use this for initialization
        void Start()
        {
            energyBar = energyBar.GetComponent<Image>();
            currentEnergyPoints = maxEnergyPoints;
            UpdateEnergyBarImage();
        }
        private void Update()
        {
            if (currentEnergyPoints < maxEnergyPoints)
            {
                RegenEnergyPoints();
            }
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

        private float EnergyAsPercentage
        {
            get
            {
                return currentEnergyPoints / (float)maxEnergyPoints;
            }

        }
    }
}
