
using UnityEngine;

namespace GameDevHQ.Scripts.UI
{
    public class UpgradeTowerChoiceUIManager : AbstractTowerInteractableChoiceUIManager
    {
        public override void Initialize(TowerSpot ts, TowerInfo towerInfo)
        {
            base.Initialize(ts, towerInfo);
            if (!towerInfo.IsUpgradeAble)
            {
                // TODO: Replace with a UI prompt that states this that then fades away.
                Debug.Log(
                    $"Tower {ts.GetTowerPlacedOnSpot().name} cannot be upgraded.");
                ReadyToDisplay = false;
            }
        }

        protected override void SetChoiceUISprite()
        {
            _choiceUIImage.sprite = _towerToUpgradeInfo.UpgradeSprite;
        }

        public override void DisplayUI()
        {
            base.DisplayUI();
            _warFundsValueText.text = $"{_towerToUpgradeInfo.WarFundsValue.ToString()}";
            
        }

        public override void ConfirmChoice()
        {
            TryBuyUpgrade();
        }

        public void TryBuyUpgrade()
        {
            
            if (GameManager.Instance.PlayerCanPurchaseItem(_towerToUpgradeInfo.WarFundsValue))
            {
                GameManager.Instance.PurchaseItem(_towerToUpgradeInfo.WarFundsValue);
                TowerManager.Instance.PlayerUpgradeTowerOnSpot(_towerSpot,
                    _towerToUpgradeInfo.UpgradedTowerPrefab);
                TurnOffUI();
            }
            else
            {
                UpgradeFailedAnim();
            }
        }

        private void UpgradeFailedAnim()
        {
            PlayerUIManager.Instance.UpgradeFailedAnim();
        }
        
    }
}
