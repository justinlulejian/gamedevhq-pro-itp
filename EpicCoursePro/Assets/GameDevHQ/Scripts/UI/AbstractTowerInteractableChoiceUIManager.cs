using System;
using UnityEngine;
using UnityEngine.UI;

namespace GameDevHQ.Scripts
{
    public abstract class AbstractTowerInteractableChoiceUIManager : MonoBehaviour
    {
        protected TowerSpot _towerSpot;
        [SerializeField] protected Button _confirmButton;
        [SerializeField] protected Button _cancelButton;
        [SerializeField] protected Text _warFundsValueText;
        [SerializeField] protected Image _choiceUIImage;

        protected TowerInfo _towerToUpgradeInfo;

        public bool ReadyToDisplay { get; protected set; }

        protected virtual void Awake()
        {
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
            if (_warFundsValueText == null)
            {
                Debug.LogError($"{this.name} ID: {GetInstanceID().ToString()} UI doesn't " +
                               $"have access to war funds text UI object.");
            }
            if (_choiceUIImage == null)
            {
                Debug.LogError($"{this.name} ID: {GetInstanceID().ToString()} UI doesn't " +
                               $"have access to UI Image for sprite setting.");
            }

        }

        protected abstract void SetChoiceUISprite();

        public virtual void DisplayUI()
        {
            if (!ReadyToDisplay)
            {
                Debug.LogError($"Upgrade UI manager must be ReadyToDisplay() before display.");
                throw new NotImplementedException();
            }
            
            SetChoiceUISprite();
        }

        public abstract void ConfirmChoice();

        private void OnDisable()
        {
            _towerSpot = null;
            _towerToUpgradeInfo = new TowerInfo();
            _warFundsValueText.text = "";
            _choiceUIImage.sprite = null;
            ReadyToDisplay = false;
        }

        public virtual void Initialize(TowerSpot ts, TowerInfo towerInfo)
        {
            _towerSpot = ts;
            _towerToUpgradeInfo = towerInfo;
            ReadyToDisplay = true;
        }

        public void DismissUI()
        {
            TurnOffUI();
        }

        protected void TurnOffUI()
        {
            gameObject.SetActive(false);
        }
    }
}