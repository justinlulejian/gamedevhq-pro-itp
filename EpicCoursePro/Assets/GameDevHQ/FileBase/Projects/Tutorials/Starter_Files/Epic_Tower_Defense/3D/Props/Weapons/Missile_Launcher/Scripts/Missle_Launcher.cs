using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using GameDevHQ.FileBase.Missle_Launcher.Missle;
using GameDevHQ.Scripts;

namespace GameDevHQ.FileBase.Missle_Launcher
{
    public class Missle_Launcher : Tower
    {
        [SerializeField]
        private GameObject _missilePrefab; //holds the missle gameobject to clone
        [SerializeField]
        private GameObject[] _misslePositions; //array to hold the rocket positions on the turret
        [SerializeField]
        private float _fireDelay; //fire delay between rockets
        [SerializeField]
        private float _launchSpeed; //initial launch speed of the rocket
        [SerializeField]
        private float _power; //power to apply to the force of the rocket
        [SerializeField]
        private float _fuseDelay; //fuse delay before the rocket launches
        [SerializeField]
        private float _reloadTime; //time in between reloading the rockets
        [SerializeField]
        private float _destroyTime = 10.0f; //how long till the rockets get cleaned up
        private bool _launched; //bool to check if we launched the rockets
        
        // Coroutines
        private Coroutine _enemyTrackingCoroutine;
        private Coroutine _fireAndReloadCoroutine;
        private int _nextAvailableRocketMissilePosition;

        // protected override void Update()
        // {
        //     base.Update();
        //     // TODO: This is duplicated with Gatling impl for now, but missile collision damage
        //     // feature will remove this.
        //     if (Time.time > _canFire && _currentTargetedEnemy && !_currentTargetedEnemy.IsDead)
        //     {
        //         _currentTargetedEnemy.PlayerDamageEnemy(_damageValue);
        //         _canFire = Time.time + _damageRate;
        //     }
        //
        //     if (_currentTargetedEnemy && !_launched)
        //     {
        //         StartCoroutine(FireRocketsRoutine());
        //         _launched = true;
        //     }
        // }

        private void Start()
        {
            StartCoroutine(FiringRoutine());
        }

        private void FireRocket(int i)
        {
            // TODO: Pool rockets
            GameObject rocket = Instantiate(_missilePrefab) as GameObject; //instantiate a rocket

            rocket.transform.parent = _misslePositions[i].transform; //set the rockets parent to the missle launch position 
            rocket.transform.localPosition = Vector3.zero; //set the rocket position values to zero
            rocket.transform.localEulerAngles = new Vector3(-90, 0, 0); //set the rotation values to be properly aligned with the rockets forward direction
            rocket.transform.parent = null; //set the rocket parent to null

            rocket.GetComponent<GameDevHQ.FileBase.Missle_Launcher.Missle.Missle>().AssignMissleRules(
                _launchSpeed, _power, _fuseDelay, _destroyTime); //assign missle properties 

            _misslePositions[i].SetActive(false); //turn off the rocket sitting in the turret to make it look like it fired

        }

        private IEnumerator FireAndReloadRoutine()
        {
            while (true)
            {
                for (int i = 0; i < _misslePositions.Length; i++)
                {
                    FireRocket(i);
                    yield return new WaitForSeconds(_fireDelay);
                }
            
                // Reload rockets.
                yield return StartCoroutine(ReloadRockets());
            }
        }

        private IEnumerator ReloadRockets()
        {
            foreach (GameObject missile in _misslePositions)
            {
                yield return new WaitForSeconds(_reloadTime); //wait for reload time
                missile.SetActive(true); //enable fake rocket to show ready to fire
            }
        }

        private IEnumerator TrackEnemyRoutine()
        {
            while (_currentTargetedEnemy != null)
            {
                RotateTowardsTarget();
                yield return null;
            }
        }

        private IEnumerator FiringRoutine()
        {
            yield return new WaitUntil(() => _currentTargetedEnemy != null);
            Enemy originalEnemyTarget = _currentTargetedEnemy;

            this.StartCoroutine(TrackEnemyRoutine(), ref _enemyTrackingCoroutine);
            // TODO: Maybe do a raycast instead of static wait so that turret points at enemy
            // before firing. Won't be an issue once they work with homing.
            // Wait a moment for the turret to get some rotation towards target before firing.
            yield return new WaitForSeconds(.5f);
            this.StartCoroutine(FireAndReloadRoutine(), ref _fireAndReloadCoroutine);

            while (true)
            {
                if (_currentTargetedEnemy != originalEnemyTarget)
                {
                    this.TryStopCoroutine(ref _enemyTrackingCoroutine);
                    this.TryStopCoroutine(ref _fireAndReloadCoroutine);
                    yield return StartCoroutine(ReloadRockets());
                    // TODO: Could this be refactored to be a continuously running coroutine?
                    StartCoroutine(FiringRoutine());
                    break;
                }
                
                RotateTowardsTarget();
                yield return null;
            }
        }
        
        // IEnumerator FireRocketsRoutine()
        // {
        //     // TODO: Maybe do a raycast instead of static wait so that turret points at enemy
        //     // before firing. Won't be an issue once they work with homing.
        //     // Wait a moment for the turret to get some rotation towards target before firing.
        //     yield return new WaitForSeconds(.5f);
        //     for (int i = 0; i < _misslePositions.Length; i++) //for loop to iterate through each missle position
        //     {
        //         // If we switch enemies mid-routine then stop firing, reload, and then start
        //         // shooting again. TODO: this'll need to change once it's not called in update anymore?
        //         if (_currentTargetedEnemy == null)
        //         {
        //             break;
        //         }
        //         FireRocket(i);
        //         yield return new WaitForSeconds(_fireDelay); //wait for the firedelay
        //     }
        //
        //     // Reset/reload rockets after firing.
        //     for (int i = 0; i < _misslePositions.Length; i++) //itterate through missle positions
        //     {
        //         yield return new WaitForSeconds(_reloadTime); //wait for reload time
        //         _misslePositions[i].SetActive(true); //enable fake rocket to show ready to fire
        //     }
        //
        //     _launched = false; //set launch bool to false
        // }

        protected override void StopAttacking()
        {
            _currentTargetedEnemy = null;
        }

        // protected override void ResetFiringState()
        // {
        //     _firingAtEnemy = false;
        // }
    }
}

