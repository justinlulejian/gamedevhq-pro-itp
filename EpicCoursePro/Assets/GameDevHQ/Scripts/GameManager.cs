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
    
        public static event Action<int> onWarFundsChange;
        
        
        private void OnEnable()
        {
            Enemy.onEnemyKilledByPlayer += AddWarFundsForEnemy;
            SceneManager.sceneLoaded += OnSceneLoaded;
        }

        private void OnDisable()
        {
            Enemy.onEnemyKilledByPlayer -= AddWarFundsForEnemy;
            SceneManager.sceneLoaded -= OnSceneLoaded;
        }

        protected override void Awake()
        {
            base.Awake();
            _fixedDeltaTime = Time.fixedDeltaTime;
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
