namespace GameDevHQ.Scripts
{
    public class DecoyTower : AbstractTower
    {
        protected override void OnEnable()
        {
            TowerManager.onDecoyEnabled += TurnOnAttackRadius;
            TowerManager.onTowerPreview += TurnOffAttackRadius;
        }

        protected override void OnDisable()
        {
            TowerManager.onDecoyEnabled -= TurnOnAttackRadius;
            TowerManager.onTowerPreview -= TurnOffAttackRadius;
        }

        protected override void VerifyTowerInfo()
        {
            // Decoy's don't need to set any tower info since they are never passed around.
        }
    }
}