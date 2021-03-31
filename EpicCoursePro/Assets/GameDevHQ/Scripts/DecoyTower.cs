using UnityEngine;

namespace GameDevHQ.Scripts
{
    public class DecoyTower : AbstractTower
    {
        private Material _attackRadiusMaterial;

        private readonly Color _attackRadiusEnabledColor = new Color(
            71 / 255.0f, 255 / 255.0f, 83 / 255.0f, 134/ 255.0f);
        private readonly Color _attackRadiusDisabledColor = new Color(
            255 / 255.0f, 17 / 255.0f, 0 / 255.0f, 134/ 255.0f);
        
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

        protected override void Awake()
        {
            base.Awake();
            _attackRadiusMaterial = _attackRadiusMeshRenderer.material;

            if (_attackRadiusMaterial == null)
            {
                Debug.LogError(
                    $"Decoy tower {name},{this.GetInstanceID().ToString()} does not have" +
                    $" access to it's attack radius material color.");
            }
        }

        private void ChangeDecoyAttackRadiusToGreen()
        {
            _attackRadiusMaterial.color = _attackRadiusEnabledColor;
        }
        
        private void ChangeDecoyAttackRadiusToRed()
        {
            _attackRadiusMaterial.color = _attackRadiusDisabledColor;
        }

        protected override void VerifyTowerInfo()
        {
            // Decoy's don't need to set any tower info since they are never passed around.
            return;
        }
    }
}