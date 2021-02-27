using System;
using UnityEngine;

namespace GameDevHQ.Scripts
{
    public class Tower : AbstractTower
    {
        
        protected virtual void OnEnable()
        {
            TowerManager.onTowerPreview += TurnOnAttackRadius;
            TowerManager.onTowerPlaced += TurnOffAttackRadius;
            TowerManager.onTowerPlacementModeStatusChange += ChangeAttachRadiusVisible;
        }

        protected virtual void OnDisable()
        {
            TowerManager.onTowerPreview -= TurnOnAttackRadius;
            TowerManager.onTowerPlaced -= TurnOffAttackRadius;
            TowerManager.onTowerPlacementModeStatusChange -= ChangeAttachRadiusVisible;
        }
        
        protected void Awake()
        {
            if (AttackRadiusObj == null)
            {
                Debug.LogError($"Attack radius was null on tower {this.name}.");
            }
            
            _attackRadiusMeshRenderer = AttackRadiusObj.GetComponent<MeshRenderer>();
            
            if (_attackRadiusMeshRenderer == null)
            {
                Debug.LogError($"Attack radius mesh renderer was null on tower {this.name}.");
            }
        }

        protected void TurnOnAttackRadius()
        {
            _attackRadiusMeshRenderer.enabled = true;
        }
        
        protected void TurnOffAttackRadius()
        {
            _attackRadiusMeshRenderer.enabled = false;
        }
        
        protected void ChangeAttachRadiusVisible(bool visible)
        {
            _attackRadiusMeshRenderer.enabled = visible;
        }
    }
}