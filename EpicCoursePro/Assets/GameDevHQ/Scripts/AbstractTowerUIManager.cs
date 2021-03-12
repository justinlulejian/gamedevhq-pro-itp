using System;
using UnityEngine;
using UnityEngine.UI;

namespace GameDevHQ.Scripts
{
    public abstract class AbstractTowerUIManager : MonoBehaviour
    {
        public TowerSpot Spot { get; set; }
        [SerializeField] 
        protected Button _confirmButton;
        [SerializeField] 
        protected Button _cancelButton;
        [SerializeField]
        protected Text _warFundsValueText;
        
        protected virtual void Awake()
        {
            if (Spot == null)
            {
                Debug.LogError($"{this.name} ID: {GetInstanceID().ToString()} UI doesn't " +
                               $"have access to confirm button.");
            }
            if (_confirmButton == null)
            {
                Debug.LogError($"{this.name} ID: {GetInstanceID().ToString()} UI doesn't" +
                               $" " +
                               $"have access to confirm button.");
            }
            if (_cancelButton == null)
            {
                Debug.LogError($"{this.name} ID: {GetInstanceID().ToString()} UI doesn't " +
                               $"have access to cancel button.");
            }
        }

        protected abstract void PresentUI();
        
        private void OnEnable()
        {
            PresentUI();
        }

        private void OnDisable()
        {
            Spot = null;
        }
        
        
        protected void TurnOffUI()
        {
           gameObject.SetActive(false);
        }
        
    }
}