﻿using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace GameDevHQ.Scripts.UI
{
    public class PlayerUIManager : MonoSingleton<PlayerUIManager>
    {
        [Header("UI Components - Game Info and Manipulation")]
        [SerializeField] 
        private GameObject _warFundsUIObject;
        private WarFundsUIManager _warFundsUI;
        [SerializeField] 
        private GameObject _playbackSpeedUIObject;
        private PlaybackSpeedUIManager _playbackSpeedUI;
        [SerializeField] 
        private GameObject _restartUIObject;
        private RestartUIManager _restartUI;
        [SerializeField] 
        private GameObject _livesWaveUIObject;
        private LivesWaveUIManager _livesWaveUI;
        [SerializeField] 
        private GameObject _levelStatusUIObject;
        private LevelStatusUIManager _levelStatusUI;
        [Header("UI Components - Tower Manipulation")] 
        [SerializeField] 
        private GameObject _armoryUIObject;
        private ArmoryUIManager _armoryUI;
        [SerializeField] 
        private GameObject _upgradeTowerUIObject;
        private UpgradeTowerChoiceUIManager _upgradeTowerChoiceUI;
        [SerializeField]
        private GameObject _dismantleUIObject;
        private DismantleTowerUIManager _dismantleUI;

        private readonly List<MonoBehaviour> _userInterfaces =
            new List<MonoBehaviour>();
        
        private void OnEnable()
        {
            LevelStatusUIManager.onCountdownFinished += StartCountdownFinished;
            WaveManager.onWaveStart += UpdateWaveCountUI;
        }

        private void OnDisable()
        {
            LevelStatusUIManager.onCountdownFinished += StartCountdownFinished;
            WaveManager.onWaveStart -= UpdateWaveCountUI;
        }

        private void StartCountdownFinished()
        {
            GameManager.Instance.StartCountdownFinished();
            _levelStatusUIObject.SetActive(false);
        }

        protected override void Awake()
        {
            base.Awake();
            _warFundsUI = _warFundsUIObject.GetComponent<WarFundsUIManager>();
            _playbackSpeedUI = _playbackSpeedUIObject.GetComponent<PlaybackSpeedUIManager>();
            _restartUI = _restartUIObject.GetComponent<RestartUIManager>();
            _livesWaveUI = _livesWaveUIObject.GetComponent<LivesWaveUIManager>();
            _levelStatusUI = _levelStatusUIObject.GetComponent<LevelStatusUIManager>();
            _armoryUI = _armoryUIObject.GetComponent<ArmoryUIManager>();
            _upgradeTowerChoiceUI =
                _upgradeTowerUIObject.GetComponent<UpgradeTowerChoiceUIManager>();
            _dismantleUI = _dismantleUIObject.GetComponent<DismantleTowerUIManager>();
            _userInterfaces.AddRange(new MonoBehaviour[]
            {
                _warFundsUI, _playbackSpeedUI, _restartUI, _livesWaveUI, _levelStatusUI, _armoryUI,
                _upgradeTowerChoiceUI, _dismantleUI
            });

            List<MonoBehaviour> nullUIs =
                _userInterfaces.FindAll(ui => ui == null);
            if (nullUIs.Count > 0) 
            {
                Debug.LogError(
                    "UI Manager does not have reference all UI components. Missing:" +
                    $" {nullUIs}");
            }
        }

        // Tower upgrade and dismantle are mutually exclusive UIs.
        public void EnableTowerUpgradeUI(TowerSpot towerSpot, TowerInfo towerInfo)
        {
            TowerManager.Instance.DeactivateTowerPlacementMode();
            _dismantleUIObject.SetActive(false);
            _upgradeTowerUIObject.SetActive(true);
            _upgradeTowerChoiceUI.Initialize(towerSpot, towerInfo);
            if (!_upgradeTowerChoiceUI.ReadyToDisplay)
            {
                // TODO: Necessary? Or can active be false and script will still run when called?
                _upgradeTowerUIObject.SetActive(false);
                return;
            }
            _upgradeTowerChoiceUI.DisplayUI();
        }

        public void EnableTowerDismantleUI(TowerSpot towerSpot, TowerInfo towerInfo)
        {
            TowerManager.Instance.DeactivateTowerPlacementMode();
            _upgradeTowerUIObject.SetActive(false);
            _dismantleUIObject.SetActive(true);
            _dismantleUI.Initialize(towerSpot, towerInfo);
            if (!_dismantleUI.ReadyToDisplay)
            {
                // TODO: Necessary? Or can active be false and script will still run when called?
                _dismantleUIObject.SetActive(false);
                return;
            }
            _dismantleUI.DisplayUI();
        }

        public void UpgradeFailedAnim()
        {
            WarFundsUIManager.Instance.WarFundsOutAnim();
        }

        public void PresentStartUI()
        {
            _levelStatusUIObject.SetActive(true);
            _levelStatusUI.PresentStartUI();
        }

        #region Lives, Waves, and Version

        public void UpdatePlayerLives(int livesRemaining)
        {
            _livesWaveUI.UpdateLivesCount(livesRemaining);
        }

        private void UpdateWaveCountUI(int currentWaveNumber, int totalWavesNumber)
        {
            _livesWaveUI.UpdateWaveCount(currentWaveNumber, totalWavesNumber);
        }

        public void UpdateVersionNumber(float versionNumber)
        {
            _livesWaveUI.UpdateVersionNumber(versionNumber);
        }

        #endregion

       

        #region Control Speed State UI

        public void PauseClicked()
        {
            GameManager.Instance.PauseGameSpeed();
        }

        public void PlayClicked()
        {
            GameManager.Instance.ResetGameSpeed();
        }

        public void FastForwardClicked()
        {
            GameManager.Instance.DoubleGameSpeed();
        }
        
        #endregion
    }
}