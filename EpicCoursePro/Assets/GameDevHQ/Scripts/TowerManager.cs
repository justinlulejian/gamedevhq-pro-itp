using System;
using System.Collections.Generic;
using System.Linq;
using GameDevHQ.Scripts;
using UnityEngine;
using UnityEngine.UIElements;

public class TowerManager : MonoSingleton<TowerManager>
{
    [SerializeField] 
    private GameObject _decoyTowerContainer;

    [SerializeField] 
    private List<GameObject> _decoyTowerPrefabs;
    private HashSet<DecoyTower> _instantiatedDecoyTowers = new HashSet<DecoyTower>();
    [SerializeField] 
    private GameObject _towerSpotContainer;
    private List<TowerSpot> _allTowerSpots = new List<TowerSpot>();

    // The decoy tower following the user's cursor, if requested.
    private DecoyTower _currentDecoyTower;
    private GameObject _previewDecoyOnTowerSpot;

    [SerializeField]
    private List<GameObject> _towerPrefabs;

    private List<AbstractTower> _towerObjs = new List<AbstractTower>();

    private Camera _playerCamera;
    public static event Action<bool> onTowerPlacementModeStatusChange;
    public static event Action onTowerSpotPreview;
    public static event Action onTowerPlaced;
    public static event Action onTowerDecoyPlacementPreview;
    
    private bool _towerPlacementModeActivated;

    #if UNITY_EDITOR
    // Debug functionality
    public List<TowerSpot> GetAllTowerSpots()
    {
        return _allTowerSpots;
    }
    #endif

    public bool IsTowerPlacementModeActivated()
    {
        return _towerPlacementModeActivated;
    }
    
    private void OnEnable()
    {
        TowerSpot.onUserPreviewTowerOnSpotIntent += ActivateTowerOnPlacementPreview;
        TowerSpot.onUserNoLongerPlaceTowerOnSpotPreviewIntent += DeactivateTowerOnPlacementPreview;
        TowerSpot.onUserPlaceTowerIntent += PlaceTower;
    }

    private void OnDisable()
    {
        TowerSpot.onUserPreviewTowerOnSpotIntent -= ActivateTowerOnPlacementPreview;
        TowerSpot.onUserNoLongerPlaceTowerOnSpotPreviewIntent -= DeactivateTowerOnPlacementPreview;
        TowerSpot.onUserPlaceTowerIntent -= PlaceTower;
    }
    
    protected override void Awake()
    {
        base.Awake();

        _allTowerSpots = _towerSpotContainer.GetComponentsInChildren<TowerSpot>().ToList();
        _playerCamera = PlayerCamera.Instance.GetPlayerCamera();
        
        if (_allTowerSpots.Count == 0)
        {
            Debug.LogError("No tower spots found from Tower Manager.");
        }
        if (_decoyTowerContainer == null)
        {
            Debug.LogError("Decoy tower container is null in Tower Manager.");
        }
        
        if (_towerPrefabs.Count < 1)
        {
            Debug.LogError("No tower prefabs specified to Tower Manager.");
        }
        
        foreach (GameObject decoyTowerPrefab in _decoyTowerPrefabs)
        {
            GameObject spawnedDecoyTowerType =
                Instantiate(decoyTowerPrefab, _decoyTowerContainer.transform);
            spawnedDecoyTowerType.transform.rotation = TowerSpot.TowerFacingEnemiesRotation;
            spawnedDecoyTowerType.SetActive(false);
            DecoyTower spawnedDecoyTower = spawnedDecoyTowerType.GetComponent<DecoyTower>();
            _instantiatedDecoyTowers.Add(spawnedDecoyTower);
        }

        foreach (GameObject towerPrefab in _towerPrefabs)
        {
            _towerObjs.Add(towerPrefab.GetComponent<AbstractTower>());
        }
    }

    private void FixedUpdate()
    {
        // TODO: Switch this to be done by decoy tower itself or will that not work?
        if (_currentDecoyTower)
        {
            Ray rayOrigin = _playerCamera.ScreenPointToRay(Input.mousePosition);
            RaycastHit[] raycastHits = new RaycastHit[1];
            if (Physics.RaycastNonAlloc(rayOrigin, raycastHits) > 0)
            {
                _currentDecoyTower.transform.position = raycastHits[0].point;
            }
        }
    }
    
    private void Update()
    {
        if (_towerPlacementModeActivated && Input.GetMouseButtonDown((int)MouseButton.RightMouse))
        {
            DeactivateTowerPlacementMode();
        }
    }
    
    private void ActivateTowerOnPlacementPreview(TowerSpot towerSpot)
    {
        {
            Transform towerSpotTransform = towerSpot.transform;
            _currentDecoyTower.transform.SetPositionAndRotation(
                towerSpotTransform.position, TowerSpot.TowerFacingEnemiesRotation);
            onTowerSpotPreview?.Invoke();
        }
    }

    private void DeactivateTowerOnPlacementPreview()
    {
        // This name doesn't make sense but it's to ensure we turn decoy radius material color
        // back to red when we go off the tower spot.
        onTowerDecoyPlacementPreview?.Invoke();
    }

    public void PlaceTowerOfTypeOnSpot(GameObject towerGameObject, TowerSpot towerSpot)
    {
        GameObject towerObjectToPlace = PoolManager.Instance.RequestObjOfType(towerGameObject);
        towerObjectToPlace.SetActive(true);
        towerSpot.PlaceTower(towerObjectToPlace);
        onTowerPlaced?.Invoke();
    }
    
    private void PlaceTower(TowerSpot towerSpot)
    {
        AbstractTower towerToPlace = _towerObjs.First(
            t => t.TowerType == _currentDecoyTower.TowerType);
        GameManager gM = GameManager.Instance;
        if (gM.PlayerCanPurchaseItem(towerToPlace.TowerInfo.WarFundsValue))
        {
            gM.PurchaseItem(towerToPlace.TowerInfo.WarFundsValue);
            PlaceTowerOfTypeOnSpot(towerToPlace.gameObject, towerSpot);
        }

        // If user is now out of money then deactivate the mode too.
        if (gM.GetWarFunds() == 0)
        {
            DeactivateTowerPlacementMode();
        }
    }
    
    public void PlayerUpgradeTowerOnSpot(TowerSpot towerSpot, GameObject upgradedTowerPrefab)
    {
        // TODO: Convert to pooling/generic method for all upgrade/non-upgrade towers.
        GameObject spawnedUpgradedTower = Instantiate(upgradedTowerPrefab);
        if (spawnedUpgradedTower == null)
        {
            Debug.LogError("Upgraded tower for tower spot " +
                           $"{towerSpot.GetInstanceID().ToString()} was null when attempting to " +
                           "be placed by Tower Manager.");
        }
        towerSpot.PlaceTower(spawnedUpgradedTower, upgrade:true);
        
    }

    private void SetCurrentDecoyTower(int towerType)
    {
        if (_currentDecoyTower && _currentDecoyTower.TowerType != towerType)
        {
            ResetCurrentDecoyTower();
        }
        DecoyTower decoyTower = _instantiatedDecoyTowers.First(
            dt => dt.TowerType == towerType);
        _currentDecoyTower = decoyTower;
        decoyTower.gameObject.SetActive(true);
        onTowerDecoyPlacementPreview?.Invoke();
    }

    private void ResetCurrentDecoyTower()
    {
        if (_currentDecoyTower)
        {
            _currentDecoyTower.gameObject.SetActive(false);
            _currentDecoyTower = null;
        }
    }
    
    public void StartTowerPlacementModeOfType(int towerType)
    {
        // TODO: Switching between tower placement types requires mouse to move out of spot and
        // return before it'll display preview.
        _towerPlacementModeActivated = true;
        DeactivateTowerOnPlacementPreview();
        onTowerPlacementModeStatusChange?.Invoke(true);
        SetCurrentDecoyTower(towerType);
    }

    public void DeactivateTowerPlacementMode()
    {
        DeactivateTowerOnPlacementPreview();
        ResetCurrentDecoyTower();
        _towerPlacementModeActivated = false;
        onTowerPlacementModeStatusChange?.Invoke(false);
    }
    
}
