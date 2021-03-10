using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using GameDevHQ.Scripts;
using UnityEditor.Rendering;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.UI;

public class UpgradeGunUIManager : MonoBehaviour
{
    [SerializeField] 
    private Text _upgradeCostText;
    [SerializeField] 
    private Button _upgradeConfirmButton;
    [SerializeField] 
    private Button _upgradeCancelButton;
    [SerializeField] 
    private GameObject _upgradedTowerPrefab;
    [SerializeField]
    private GameObject _towerToUpgradePrefab;
    private Tower _towerToUpgrade;
    
    [SerializeField] 
    private int _upgradeCost = 100;
    private Image _upgradeImage; // For swapping of gun upgrade sprite for different towers.

    private List<Text> _texts = new List<Text>();
    private List<Button> _buttons = new List<Button>();
    private List<Image> _images = new List<Image>();
    private TowerSpot _currentTowerSpotToUpgrade;

    public static event Action<TowerSpot, GameObject> onPlayerCanPlaceUpgrade; 
    public static event Action onPlayerNotEnoughWarFundsForUpgrade; 
    
    private void OnEnable()
    {
        TowerSpot.onMouseDownUpgradeTowerSpot += PresentUpgradeUI;
    }

    private void OnDisable()
    {
        TowerSpot.onMouseDownUpgradeTowerSpot -= PresentUpgradeUI;
    }

    private void Awake()
    {
        _upgradeImage = GetComponent<Image>();
        _texts = GetComponentsInChildren<Text>().ToList();
        _buttons.Add(_upgradeConfirmButton);
        _buttons.Add(_upgradeCancelButton);
        _images.Add(_upgradeImage);
        _images.AddRange(GetComponentsInChildren<Image>());
        _towerToUpgrade = _towerToUpgradePrefab.GetComponent<Tower>();

        if (_upgradeImage == null)
        {
            Debug.LogError($"Upgrade UI doesn't have access to Image component.");
        }
        if (_texts.Count < 1)
        {
            Debug.LogError($"Upgrade UI doesn't have access to text child components.");
        }
        if (_buttons.Count < 1)
        {
            Debug.LogError($"Upgrade UI doesn't have access to button child components.");
        }
        if (_images.Count < 2)
        {
            Debug.LogError($"Upgrade UI doesn't have access to all image/button child" +
                           $" components.");
        }
        if (_upgradeConfirmButton == null)
        {
            Debug.LogError($"Upgrade UI doesn't have access to confirm button.");
        }
        if (_upgradeCancelButton == null)
        {
            Debug.LogError($"Upgrade UI doesn't have access to cancel button.");
        }
        if (_upgradedTowerPrefab == null)
        {
            Debug.LogError($"Upgrade UI doesn't have access to upgraded tower prefab.");
        }
        if (_towerToUpgrade == null)
        {
            Debug.LogError($"Upgrade UI doesn't have access to original tower information.");
        }
    }

    private void EnableDisableUI(bool onoff)
    {
        foreach (var text in _texts)
        {
            text.enabled = onoff;
        }
        foreach (var button in _buttons)
        {
            button.enabled = onoff;
        }
        foreach (var image in _images)
        {
            image.enabled = onoff;
        }
    }

    private void TurnOnUI()
    {
        EnableDisableUI(true);
    }

    public void TurnOffUI()
    {
        EnableDisableUI(false);
        _currentTowerSpotToUpgrade = null;
    }

    private void Start()
    {
        SetUpgradeCostAmount();
    }

    private void SetUpgradeCostAmount()
    {
        _upgradeCostText.text = $"{_upgradeCost.ToString()}";
    }

    private void PresentUpgradeUI(TowerSpot towerSpot)
    {
        if (!(towerSpot.GetTowerPlacedOnSpot().TowerType == _towerToUpgrade.TowerType))
        {
            return;
        }
        _currentTowerSpotToUpgrade = towerSpot;
        TurnOnUI();
    }
    
    public void TryBuyUpgrade()
    {
        if (_currentTowerSpotToUpgrade == null) 
        {
            Debug.LogError(
                $"Upgrade UI for tower {_upgradedTowerPrefab.name} doesn't know which " +
                $"tower spot to upgrade.");
        }
        
        if (GameManager.Instance.PlayerCanPurchaseItem(_upgradeCost))
        {
            GameManager.Instance.PurchaseItem(_upgradeCost);
            // Tower manager to replace the upgrade tower on the spot and destroy the original.
            // Call TM with prefab of the upgraded tower.
            onPlayerCanPlaceUpgrade?.Invoke(_currentTowerSpotToUpgrade, _upgradedTowerPrefab);
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
