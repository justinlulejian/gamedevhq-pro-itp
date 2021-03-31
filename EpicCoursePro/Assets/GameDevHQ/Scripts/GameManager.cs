using System;
using GameDevHQ.Scripts.UI;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace GameDevHQ.Scripts
{
    public class GameManager : MonoSingleton<GameManager>
    {
        [SerializeField]
        private int _playerWarFunds = 500;
        [SerializeField] 
        private int _playerMaximumWarFunds = 100000;
        private float _currentTimeScale;
        private float _fixedDeltaTime;
       

        [SerializeField]
        private int _currentPlayerLives;
        [SerializeField] 
        private int _startingPlayerLives;
        [SerializeField] 
        private int _highLivesAmountMin = 8;
        [SerializeField] 
        private int _mediumLivesAmountMin = 5;

        // TODO: Have this incremented automatically as part of commit/build process.
        [SerializeField] 
        private float _versionNumber = 0.1f;

        public static event Action<int> onWarFundsChange;
        
        #if UNITY_EDITOR
        // Debug functionality
        public bool SkipIntroAckAndCountdown;
        public bool UnlimitedWarFunds;

        public void DrainWarFunds()
        {
            _playerWarFunds = 0;
        }

        public void ChangeTimeSpeedIncrement(float increment)
        {
            Time.timeScale = Mathf.Clamp(Time.timeScale + increment, 0.0f, 5.0f);
            Time.fixedDeltaTime = _fixedDeltaTime * Time.timeScale;
        }

        #endif
        
        private void OnEnable()
        {
            Enemy.onEnemyKilledByPlayer += AddWarFundsForEnemy;
            EnemyNavEnd.onEnemyCollision += PlayerLosesLifeForEnemy;
            SceneManager.sceneLoaded += OnSceneLoaded;
            WaveManager.onWavesComplete += PlayerWonLevel;

        }

        private void OnDisable()
        {
            Enemy.onEnemyKilledByPlayer -= AddWarFundsForEnemy;
            EnemyNavEnd.onEnemyCollision -= PlayerLosesLifeForEnemy;
            SceneManager.sceneLoaded -= OnSceneLoaded;
            WaveManager.onWavesComplete -= PlayerWonLevel;
        }

        protected override void Awake()
        {
            base.Awake();
            _fixedDeltaTime = Time.fixedDeltaTime;
            _currentPlayerLives = _startingPlayerLives;
        }

        private void Update()
        {
            #if UNITY_EDITOR
                if (Input.GetKey(KeyCode.H))
                {
                    Time.timeScale = Mathf.Clamp(Time.timeScale - .01f, 0f, 1.0f);
                } else if (Input.GetKey(KeyCode.J))
                {
                    Time.timeScale += .1f;
                } else if (Input.GetKey(KeyCode.K))
                {
                    Time.timeScale = 1.0f;
                }
                _currentTimeScale = Time.timeScale;
            #endif
        }

        private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
        {
            if (scene.name.Equals("Start_Level") && scene.isLoaded)
            {
                StartGame();
            }
        }

        private void StartGame()
        {
            PlayerUIManager.Instance.UpdatePlayerLives(_currentPlayerLives);
            SetUIColorOnLivesAmount();
            PlayerUIManager.Instance.UpdateVersionNumber(_versionNumber);
            #if UNITY_EDITOR
                if (!SkipIntroAckAndCountdown)
                {
                    PlayerUIManager.Instance.PresentStartUI(); 
                    return;
                }
            #endif
            StartWaves();
        }

        public void StartCountdownFinished()
        {
            PlayerUIManager.Instance.TurnOffIntroUI();
            StartWaves();
        }

        private void StartWaves()
        {
            SpawnManager.Instance.RequestNextWave();
        }

        public int GetWarFunds()
        {
            #if UNITY_EDITOR
                if (UnlimitedWarFunds)
                {
                    // TODO: make this update in the UI too.
                    return 999999;
                }
            #endif
            return _playerWarFunds;
        }

        private void SubtractWarFunds(int funds)
        {
            _playerWarFunds = Mathf.Clamp(_playerWarFunds - funds, 0, _playerMaximumWarFunds);
            onWarFundsChange?.Invoke(_playerWarFunds);
        }

        public void AddWarFunds(int funds)
        {
            _playerWarFunds += funds;
            onWarFundsChange?.Invoke(_playerWarFunds);
        }

        public bool PlayerCanPurchaseItem(int purchaseCost)
        {
            return GetWarFunds() >= purchaseCost;
        }

        public void PurchaseItem(int purchaseCost)
        {
            SubtractWarFunds(purchaseCost); 
        }

        private void AddWarFundsForEnemy(Enemy enemy)
        {
            AddWarFunds(enemy.WarFundValue);
        }


        #region Lives

        private void PlayerLosesLifeForEnemy(GameObject enemy)
        {
            // In the future we may want to reduce lives differently for different enemies.
            _currentPlayerLives = Mathf.Clamp(
                _currentPlayerLives - 1, 0, _startingPlayerLives);
            PlayerUIManager.Instance.UpdatePlayerLives(_currentPlayerLives);
            SetUIColorOnLivesAmount();
            if (_currentPlayerLives == 0)
            {
                PlayerDied();
            }
        }

        private void SetUIColorOnLivesAmount()
        {
            if (_currentPlayerLives > _highLivesAmountMin)
            {
                PlayerUIManager.Instance.SetHighHealthColor();
            }
            else if (_currentPlayerLives > _mediumLivesAmountMin)
            {
                PlayerUIManager.Instance.SetMediumHealthColor();
            }
            else
            {
                PlayerUIManager.Instance.SetLowHealthColor();
            }
        }
        #endregion
        
        #region LevelControl

        private void PlayerDied()
        {
            EnableDisableLevel(false);
            PlayerUIManager.Instance.PresentPlayerDiedUI();
        }
        
        private void PlayerWonLevel()
        {
            EnableDisableLevel(false);
            PlayerUIManager.Instance.PresentPlayerWonLevelUI();
        }

        public AsyncOperation PlayerRequestRestartLevel()
        {
            EnableDisableLevel(true);
            var loadSceneAsync = SceneManager.LoadSceneAsync("GameDevHQ/Scenes/Start_Level");
            PlayerUIManager.Instance.ResetRestartClicked();
            ResetGameSpeed();
            PlayerUIManager.Instance.ResetPlaySpeedUI();
            return loadSceneAsync;
        }
        

        private void EnableDisableLevel(bool enable)
        {
            PlayerUIManager.Instance.EnableDisableTowerPlacementUI(enable);
            SpawnManager.Instance.SpawningEnabled = enable;
        }

        #endregion
        
        #region Control Game Speed

        public void PauseGameSpeed()
        {
            Time.timeScale = 0.0f;
            Time.fixedDeltaTime = _fixedDeltaTime * Time.timeScale;
        }

        public void DoubleGameSpeed()
        {
            Time.timeScale = 2.0f;
            Time.fixedDeltaTime = _fixedDeltaTime * Time.timeScale;
        }

        public void ResetGameSpeed()
        {
            Time.timeScale = 1.0f;
            Time.fixedDeltaTime = _fixedDeltaTime * Time.timeScale;
        }

        #endregion
    }
}
