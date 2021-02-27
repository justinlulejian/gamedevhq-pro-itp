using System;
using System.Collections.Generic;
using UnityEngine;

namespace GameDevHQ.Scripts
{
    public class DecoyTower : Tower
    {
        // TODO: Set Decoy's to default to transparent shaders.

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