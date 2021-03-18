namespace GameDevHQ.Scripts.UI
{
    public class DismantleTowerUIManager : AbstractTowerInteractableChoiceUIManager
    {
        protected override void SetChoiceUISprite()
        {
            _choiceUIImage.sprite = _towerToUpgradeInfo.DismantleSprite;
        }

        public override void DisplayUI()
        {
            base.DisplayUI();
            _warFundsValueText.text = $"{_towerToUpgradeInfo.DismantleValue.ToString()}";
        }

        public override void ConfirmChoice()
        {
            DismantleTower();
        }

        public void DismantleTower()
        {
            GameManager.Instance.AddWarFunds(
                _towerSpot.GetTowerPlacedOnSpot().TowerInfo.WarFundsValue);
            _towerSpot.DismantleTower();
            TurnOffUI();
        }
    }
}
