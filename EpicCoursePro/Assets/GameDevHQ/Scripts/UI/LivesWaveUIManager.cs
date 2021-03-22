using System;
using UnityEngine;
using UnityEngine.UI;

namespace GameDevHQ.Scripts.UI
{
    public class LivesWaveUIManager : MonoBehaviour
    {
        [SerializeField] 
        private Text _livesCountText;
        [SerializeField] 
        private Text _waveCountText;
        [SerializeField] 
        private Text _versionNumberText;


        private void Awake()
        {
            if (_livesCountText == null)
            {
                Debug.LogError("Lives/Waves UI manager is missing the lives count text.");
            }
            if (_waveCountText == null)
            {
                Debug.LogError("Lives/Waves UI manager is missing the waves count text.");
            }
            if (_versionNumberText == null)
            {
                Debug.LogError("Lives/Waves UI manager is missing the version number text.");
            }
        }

        public void UpdateLivesCount(int livesRemaining)
        {
            _livesCountText.text = livesRemaining.ToString();
        }
        
        public void UpdateWaveCount(int wave, int totalWaves)
        {
            _waveCountText.text = $"{wave.ToString()}/ {totalWaves.ToString()}";
        }
        
        public void UpdateVersionNumber(float version)
        {
            _versionNumberText.text = $"v{version.ToString()}";
        }
    }
}
