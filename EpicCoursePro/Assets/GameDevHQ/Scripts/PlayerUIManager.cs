using System.Collections.Generic;
using System.Linq;
using GameDevHQ.Scripts;
using UnityEngine;
using UnityEngine.Rendering;

public class PlayerUIManager : MonoSingleton<PlayerUIManager>
{
    [SerializeField] 
    private GameObject _warFundsUIObject;
    private WarFundsUIManager _warFundsUI;
    [SerializeField] 
    private List<GameObject> _upgradeTowerUIObjects = new List<GameObject>();
    private List<UpgradeGunUIManager> _upgradeGunUIs;
    [SerializeField] 
    private GameObject _dismantleUIObject;
    private DismantleTowerUIManager _dismantleUI;

    private List<GameObject> _interactableUIObjects = new List<GameObject>();

    public enum UIStates
    {
        UpgradeGatlingTowerUIState,
        UpgradeMissileTowerUIState,
        DismantleTowerUIState,
    }

    private Dictionary<UIStates, AbstractTowerUIManager> _uiStateToUIMap =
        new Dictionary<UIStates, AbstractTowerUIManager>();
    
    private void OnEnable()
    {
        TowerSpot.onUserTowerInteractIntent += EnableTowerUI;
    }

    private void OnDisable()
    {
        TowerSpot.onUserTowerInteractIntent -= EnableTowerUI;
    }

    protected override void Awake()
    {
        base.Awake();
        _warFundsUI = _warFundsUIObject.GetComponent<WarFundsUIManager>();
        foreach (var upgradeTowerUIObject in _upgradeTowerUIObjects)
        {
            UpgradeGunUIManager towerUpgradeUI = (
                upgradeTowerUIObject.GetComponent<UpgradeGunUIManager>());
            // TODO: Change this to be dynamic check for different tower types.
            if (towerUpgradeUI.TowerToUpgrade.TowerType == 1)
            {
                _uiStateToUIMap[UIStates.UpgradeGatlingTowerUIState] = towerUpgradeUI;
            } else if (towerUpgradeUI.TowerToUpgrade.TowerType == 2)
            {
                _uiStateToUIMap[UIStates.UpgradeMissileTowerUIState] = towerUpgradeUI;
            }
            _interactableUIObjects.Add(upgradeTowerUIObject);
        }
        _dismantleUI = _dismantleUIObject.GetComponent<DismantleTowerUIManager>();
        _uiStateToUIMap[UIStates.DismantleTowerUIState] = _dismantleUI;

        if (_warFundsUI == null) 
        {
            Debug.LogError("UI Manager does not have reference to war funds UI behavior " +
                           "script.");
        }
        if (_upgradeTowerUIObjects.Count < 1) 
        {
            Debug.LogError("UI Manager does not have reference to upgrade UI behavior " +
                           "scripts.");
        }
        if (_dismantleUI == null) 
        {
            Debug.LogError("UI Manager does not have reference to dismantle tower UI " +
                           "behavior script.");
        }
    }

    private void EnableTowerUI(UIStates uiState, TowerSpot towerSpot)
    {
        if (_interactableUIObjects.Any(ui => ui.activeSelf))
        {
            // UIs should not override one another. They should be dismissed by player.
            return;
        }
        
        // TODO(checkpoint): How will I communicate the tower spot to the 
        AbstractTowerUIManager uiManager = _uiStateToUIMap[uiState];
        uiManager.Spot = towerSpot;
        uiManager.gameObject.SetActive(true);
    }
    
    private void DisableTowerUI(UIStates uiState)
    {
        
        _uiStateToUIMap[uiState].gameObject.SetActive(false);
    }
    
    private void DisableAllUI()
    {
        _interactableUIObjects.ForEach(ui => ui.gameObject.SetActive(false));
    }

}
