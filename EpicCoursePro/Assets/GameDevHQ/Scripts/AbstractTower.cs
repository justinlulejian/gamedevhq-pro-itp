using UnityEngine;

namespace GameDevHQ.Scripts
{
    public abstract class AbstractTower : MonoBehaviour
    {
        // TODO: Fix the visibility of these to only what's necessary.
        public int TowerType;
        public int WarFundValue;
        public GameObject AttackRadiusObj;
        protected MeshRenderer _attackRadiusMeshRenderer;
    }
}