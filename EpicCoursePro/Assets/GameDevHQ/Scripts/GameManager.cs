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
        private bool _skipIntoAckAndCountdown;

        [SerializeField]
        private int _currentPlayerLives;
        [SerializeField] 
        private int _startingPlayerLives;

        // TODO: Have this incremented automatically as part of commit/build process.
        [SerializeField] 
        private float _versionNumber = 0.1f;

        public static event Action<int> onWarFundsChange;
        
        
        private void OnEnable()
        {
            Enemy.onEnemyKilledByPlayer += AddWarFundsForEnemy;
            EnemyNavEnd.onEnemyCollision += PlayerLosesLifeForEnemy;
            SceneManager.sceneLoaded += OnSceneLoaded;
            
        }

        private void OnDisable()
        {
            Enemy.onEnemyKilledByPlayer -= AddWarFundsForEnemy;
            EnemyNavEnd.onEnemyCollision -= PlayerLosesLifeForEnemy;
            SceneManager.sceneLoaded -= OnSceneLoaded;
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
            PlayerUIManager.Instance.UpdateVersionNumber(_versionNumber);
            
            if (!_skipIntoAckAndCountdown)
            {
                PlayerUIManager.Instance.PresentStartUI(); 
                return;
            }
            StartWaves();
        }

        public void StartCountdownFinished()
        {
            StartWaves();
        }

        private void StartWaves()
        {
            SpawnManager.Instance.StartWaves();
        }

        public int GetWarFunds()
        {
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
            return _playerWarFunds >= purchaseCost;
        }

        public void PurchaseItem(int purchaseCost)
        {
            SubtractWarFunds(purchaseCost); 
        }

        private void AddWarFundsForEnemy(Enemy enemy)
        {
            AddWarFunds(enemy.WarFundValue);
        }


        private void PlayerLosesLifeForEnemy(GameObject enemy)
        {
            // In the future we may want to reduce lives differently for different enemies.
            _currentPlayerLives = Mathf.Clamp(
                _currentPlayerLives - 1, 0, _startingPlayerLives);
            PlayerUIManager.Instance.UpdatePlayerLives(_currentPlayerLives);
            if (_currentPlayerLives == 0)
            {
                PlayerDied();
            }
        }

        private void PlayerDied()
        {
            // TODO: Add game over logic here.
        }

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
