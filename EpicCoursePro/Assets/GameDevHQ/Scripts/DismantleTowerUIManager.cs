using System;
using UnityEngine;

namespace GameDevHQ.Scripts
{
    // TODO: refactor this to have a shared AbstractUIManager class with UpgradeGunUIManager.
    public class DismantleTowerUIManager : AbstractTowerUIManager
    {
        public static event Action<TowerSpot> onDismantleTower;
        
        protected override void PresentUI()
        {
            _warFundsValueText.text = Spot.GetTowerPlacedOnSpot().WarFundValue.ToString();
        }

        public void DismantleTower()
        {
            Tower tower = Spot.GetTowerPlacedOnSpot();
            // TODO: create a new property that stores the dismantle value.
            GameManager.Instance.AddWarFunds(tower.WarFundValue);
            onDismantleTower?.Invoke(Spot);
            Spot = null;
            TurnOffUI();
        }
    }
}
