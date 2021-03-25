namespace GameDevHQ.Scripts
{
    public class DecoyTower : AbstractTower
    {
        protected override void OnEnable()
        {
            TowerManager.onTowerDecoyPlacementPreview += ChangeDecoyAttackRadiusToRed;
            TowerManager.onTowerSpotPreview += ChangeDecoyAttackRadiusToGreen;
        }

        protected override void OnDisable()
        {
            TowerManager.onTowerDecoyPlacementPreview -= ChangeDecoyAttackRadiusToRed;
            TowerManager.onTowerSpotPreview -= ChangeDecoyAttackRadiusToGreen;
        }

        private void ChangeDecoyAttackRadiusToGreen()
        {
            _attackRadiusMeshRenderer.material.color = _attackRadiusEnabledColor;
        }
        
        private void ChangeDecoyAttackRadiusToRed()
        {
            _attackRadiusMeshRenderer.material.color = _attackRadiusDisabledColor;
        }

        protected override void VerifyTowerInfo()
        {
            // Decoy's don't need to set any tower info since they are never passed around.
        }
    }
}