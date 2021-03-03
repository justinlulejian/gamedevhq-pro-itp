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

    }
}