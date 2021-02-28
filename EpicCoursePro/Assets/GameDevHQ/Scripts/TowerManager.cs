﻿using System;
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
    // The decoy tower following the user's cursor, if requested.
    private DecoyTower _currentDecoyTower;
    private GameObject _instantiatedPreviewTowerOnSpot;

    [SerializeField]
    private List<GameObject> _towerPrefabs;

    private List<AbstractTower> _towerObjs = new List<AbstractTower>();

    public static event Action<bool> onTowerPlacementModeStatusChange;
    public static event Action onTowerPreview;
    public static event Action onTowerPlaced;
    public static event Action onDecoyEnabled;
    
    private bool _towerPlacementModeActivated;

    public bool IsTowerPlacementModeActivated()
    {
        return _towerPlacementModeActivated;
    }
    
    private void OnEnable()
    {
        TowerSpot.onUserMouseEnterTowerSpot += ActivateTowerPlacementPreview;
        TowerSpot.onUserMouseExitTowerSpot += DeactivateTowerPlacementPreview;
        TowerSpot.onUserMouseDownTowerSpot += PlaceTower;
    }

    private void OnDisable()
    {
        TowerSpot.onUserMouseEnterTowerSpot -= ActivateTowerPlacementPreview;
        TowerSpot.onUserMouseExitTowerSpot -= DeactivateTowerPlacementPreview;
        TowerSpot.onUserMouseDownTowerSpot -= PlaceTower;
    }
    
    protected override void Awake()
    {
        base.Awake();
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
            Ray rayOrigin = PlayerCamera.Instance.GetPlayerCamera()
                .ScreenPointToRay(Input.mousePosition);
            RaycastHit rayHitInfo;
            if (Physics.Raycast(rayOrigin, out rayHitInfo))
            {
                _currentDecoyTower.transform.position = rayHitInfo.point;
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
    
    private void ActivateTowerPlacementPreview(TowerSpot towerSpot)
    {
        {
            _currentDecoyTower.gameObject.SetActive(false);
            AbstractTower towerToPlace = _towerObjs.First(
                t => t.TowerType == _currentDecoyTower.TowerType);
            Transform towerSpotTransform = towerSpot.transform;
            _instantiatedPreviewTowerOnSpot = Instantiate(
                towerToPlace.gameObject, towerSpotTransform.position, Quaternion.identity,
                towerSpotTransform);
            onTowerPreview.Invoke();
        }
    }

    private void DeactivateTowerPlacementPreview()
    {
        if (_towerPlacementModeActivated && _currentDecoyTower != null)
        {
            EnableDecoy();
            Destroy(_instantiatedPreviewTowerOnSpot);
        }
    }
    
    private void PlaceTower(TowerSpot towerSpot)
    {
        AbstractTower towerToPlace = _instantiatedPreviewTowerOnSpot.GetComponent<AbstractTower>();
        GameManager gM = GameManager.Instance;
        if (gM.PlayerCanPurchaseItem(towerToPlace.WarFundValue))
        {
            gM.PurchaseItem(towerToPlace.WarFundValue);
            towerSpot.PlaceTower(_instantiatedPreviewTowerOnSpot);
            _instantiatedPreviewTowerOnSpot = null;
            EnableDecoy();
            onTowerPlaced.Invoke();
        }

        if (gM.GetWarFunds() == 0)
        {
            DeactivateTowerPlacementMode();
        }
    }

    private void EnableDecoy()
    {
        if (_towerPlacementModeActivated)
        {
            _currentDecoyTower.gameObject.SetActive(true);
            onDecoyEnabled.Invoke();
        }
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
        onDecoyEnabled.Invoke();
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
        DeactivateTowerPlacementPreview();
        onTowerPlacementModeStatusChange.Invoke(true);
        SetCurrentDecoyTower(towerType);
    }

    private void DeactivateTowerPlacementMode()
    {
        DeactivateTowerPlacementPreview();
        ResetCurrentDecoyTower();
        _towerPlacementModeActivated = false;
        onTowerPlacementModeStatusChange.Invoke(false);
    }
    
}
