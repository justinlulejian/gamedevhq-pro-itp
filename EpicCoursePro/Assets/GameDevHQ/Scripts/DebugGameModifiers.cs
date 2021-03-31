using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEngine;

namespace GameDevHQ.Scripts
{
    // Functionality allowed for use with the custom debug editor window.
    public class DebugGameModifiers : MonoSingleton<DebugGameModifiers>
    {
        #if UNITY_EDITOR
        protected override void Awake() {
            // Keep script running across scene changes since it has to reset tower placement.
            // It's stateless for now so we should be okay.
            base.Awake();
            DontDestroyOnLoad(transform.gameObject);
        }
        
        public void UnlimitedWarFunds()
        {
            GameManager.Instance.UnlimitedWarFunds = true;
        }

        public void DrainFunds()
        {
            GameManager.Instance.DrainWarFunds();
        }

        public void ChangeGameSpeed(float increment)
        {
            GameManager.Instance.ChangeTimeSpeedIncrement(increment);
        }

        public void StartNeverEndingWave()
        {
            SpawnManager.Instance.StartNeverEndingWave();
        }
        
        public void StopCurrentWave()
        {
            WaveManager.Instance.StopCurrentWave();
        }
        
        public void SkipIntroOnPlay(bool enable)
        {
            GameManager.Instance.SkipIntroAckAndCountdown = enable;
        }

        // TODO: Expand this to make tower types customizable, right now it statically picks one.
        // Will require changing how upgrade towers are placed in TowerManager.
        public void PlaceTowersOnAllSpots()
        {
            List<GameObject> allTowerPrefabs = PoolManager.Instance.AllTowerPrefabs;
            GameObject gatlingGunPrefab = allTowerPrefabs.ElementAt(0);
            if (gatlingGunPrefab == null) Debug.LogError(
                "Couldn't find gatling gun tower prefab to place on all spots.");
            TowerManager.Instance.GetAllTowerSpots()
                .FindAll(spot => spot.IsAvailableForPlacement)
                .ForEach(
                    availableSpot => TowerManager.Instance.PlaceTowerOfTypeOnSpot(
                        gatlingGunPrefab, availableSpot));
        }

        

        // Reset the game but replace the same towers in their place when this occurred.
        public void ResetGameWithCurrentTowerPlacement()
        {
            Dictionary<int, GameObject> towerTypeToPrefabMap = new Dictionary<int, GameObject>();
            foreach (var towerPrefab in PoolManager.Instance.AllTowerPrefabs)
            {
                AbstractTower tower = towerPrefab.GetComponent<AbstractTower>();
                towerTypeToPrefabMap[tower.TowerType] = towerPrefab;
            }
            
            // Store the towers and where they were placed last time.
            Dictionary<string, GameObject> towerSpotBeforeTowerMapping =
                new Dictionary<string, GameObject>();
            List<TowerSpot> allPopulatedTowerSpots = TowerManager.Instance.GetAllTowerSpots()
                .FindAll(spot => spot.GetTowerPlacedOnSpot() != null);
            // Get the prefab for each tower and store that.
            allPopulatedTowerSpots.ForEach(
                spot => towerSpotBeforeTowerMapping[spot.name] = 
                    towerTypeToPrefabMap[
                        spot.GetTowerPlacedOnSpot().GetComponent<AbstractTower>().TowerType]);
            
            // Reset the game state.
            var sceneReload = GameManager.Instance.PlayerRequestRestartLevel();

            StartCoroutine(ReplacePreviousTowersRoutine(towerSpotBeforeTowerMapping, sceneReload));

        }

        private IEnumerator ReplacePreviousTowersRoutine(
            Dictionary<string, GameObject> towerSpotBeforeTowerMapping, AsyncOperation sceneReload)
        {
            yield return sceneReload;
            // Replace all the towers before the reset back on their spots.
            TowerManager.Instance.GetAllTowerSpots()
                .FindAll(spot => towerSpotBeforeTowerMapping.ContainsKey(spot.name))
                .ForEach(
                    spot => TowerManager.Instance.PlaceTowerOfTypeOnSpot(
                        towerSpotBeforeTowerMapping[spot.name], spot));
        }

        // Same as ResetGameWithCurrentTowerPlacement, but will restart the current wave running.
        public void ResetGameWithCurrentTowerPlacementAndWave()
        {
            // TODO: Impl
        }
        
        // End UNITY_EDITOR
        #endif
    }
}