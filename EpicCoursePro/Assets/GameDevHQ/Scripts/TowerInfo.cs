using UnityEngine;

namespace GameDevHQ.Scripts
{
    [System.Serializable]
    public struct TowerInfo
    {
        public int WarFundsValue;
        public int DismantleValue;
        public Sprite UpgradeSprite;
        public Sprite DismantleSprite;
        public GameObject UpgradedTowerPrefab;
        public bool IsUpgradeAble => UpgradedTowerPrefab != null;
        // TODO: Also do Armory UI button sprite?
    }
}