using System;
using GameDevHQ.Scripts;
using UnityEngine;

public class UpgradeGunUIManager : AbstractTowerUIManager
{
    [SerializeField]
    private GameObject _upgradedTowerPrefab;
    [SerializeField]
    private GameObject _towerToUpgradePrefab;
    public Tower TowerToUpgrade;
    
    [SerializeField] 
    private int _upgradeCost = 100;

    public static event Action onUpgradeUIActivated; 
    public static event Action<TowerSpot, GameObject> onPlayerCanPlaceUpgrade; 
    public static event Action onPlayerNotEnoughWarFundsForUpgrade;

    protected override void Awake()
    {
        base.Awake();
        TowerToUpgrade = _towerToUpgradePrefab.GetComponent<Tower>();

        if (_upgradedTowerPrefab == null)
        {
            Debug.LogError($"Upgrade UI doesn't have access to upgraded tower prefab.");
        }
        if (TowerToUpgrade == null)
        {
            Debug.LogError($"Upgrade UI doesn't have access to original tower information.");
        }
    }
    
    private void OnEnable()
    {
        PresentUI();
    }

    private void Start()
    {
        SetUpgradeCostAmount();
    }

    private void SetUpgradeCostAmount()
    {
        _warFundsValueText.text = $"{_upgradeCost.ToString()}";
    }

    protected override void PresentUI()
    {
        if (!(Spot.GetTowerPlacedOnSpot().TowerType == TowerToUpgrade.TowerType))
        {
            return;
        }
        onUpgradeUIActivated?.Invoke();
    }

    public void TryBuyUpgrade()
    {
        if (Spot == null) 
        {
            Debug.LogError(
                $"Upgrade UI for tower {_upgradedTowerPrefab.name} doesn't know which " +
                $"tower spot to upgrade.");
        }
        
        if (GameManager.Instance.PlayerCanPurchaseItem(_upgradeCost))
        {
            GameManager.Instance.PurchaseItem(_upgradeCost);
            onPlayerCanPlaceUpgrade?.Invoke(Spot, _upgradedTowerPrefab);
            TurnOffUI();
        }
        else
        {
            UpgradeFailedAnim();
        }
    }

    private void UpgradeFailedAnim()
    {
        onPlayerNotEnoughWarFundsForUpgrade?.Invoke();
    }
}
