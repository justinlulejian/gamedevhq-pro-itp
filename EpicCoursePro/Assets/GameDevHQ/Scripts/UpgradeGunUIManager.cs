using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Remoting.Messaging;
using GameDevHQ.Scripts;
using UnityEngine;
using UnityEngine.UI;

public class UpgradeGunUIManager : MonoBehaviour
{
    [SerializeField] 
    private Text _upgradeCostText;
    [SerializeField] 
    private Button _confirmButton;
    [SerializeField] 
    private Button _cancelButton;
    [SerializeField] 
    private GameObject _upgradedTowerPrefab;
    [SerializeField]
    private GameObject _towerToUpgradePrefab;
    private Tower _towerToUpgrade;
    
    [SerializeField] 
    private int _upgradeCost = 100;
    private Image _image; // For swapping of gun upgrade sprite for different towers.

    private List<Text> _texts = new List<Text>();
    private List<Button> _buttons = new List<Button>();
    private List<Image> _images = new List<Image>();
    private TowerSpot _towerSpotUpgrade;

    public static event Action onUpgradeUIActivated; 
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
        _image = GetComponent<Image>();
        _texts = GetComponentsInChildren<Text>().ToList();
        _buttons.Add(_confirmButton);
        _buttons.Add(_cancelButton);
        _images.Add(_image);
        _images.AddRange(GetComponentsInChildren<Image>());
        _towerToUpgrade = _towerToUpgradePrefab.GetComponent<Tower>();

        if (_image == null)
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
        if (_confirmButton == null)
        {
            Debug.LogError($"Upgrade UI doesn't have access to confirm button.");
        }
        if (_cancelButton == null)
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
        _towerSpotUpgrade = null;
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

        // if (towerSpot.IsUpgraded)
        // {
        //     TurnOffUI();
        //     DivertToTowerDismantleUI(towerSpot);
        //     return;
        // }
        onUpgradeUIActivated?.Invoke();
        _towerSpotUpgrade = towerSpot;
        TurnOnUI();
    }

    // private void DivertToTowerDismantleUI(TowerSpot towerSpot)
    // {
    //     DismantleTowerUIManager.Instance.PresentDismantleUI(towerSpot);
    // }
    
    public void TryBuyUpgrade()
    {
        if (_towerSpotUpgrade == null) 
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
            onPlayerCanPlaceUpgrade?.Invoke(_towerSpotUpgrade, _upgradedTowerPrefab);
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
