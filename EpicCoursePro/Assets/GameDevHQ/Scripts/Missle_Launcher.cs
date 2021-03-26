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
        protected GameObject _missilePrefab; //holds the missle gameobject to clone
        [SerializeField]
        private GameObject[] _misslePositionsLeft; //array to hold the rocket positions on the turret
        [SerializeField]
        private GameObject[] _missilePositionsRight; //array to hold the rocket positions on the turret
        private int _missilePositionsLength;
        [SerializeField]
        protected float _fireDelay; //fire delay between rockets
        [SerializeField]
        protected float _launchSpeed; //initial launch speed of the rocket
        [SerializeField]
        protected float _power; //power to apply to the force of the rocket
        [SerializeField]
        protected float _fuseDelay; //fuse delay before the rocket launches
        [SerializeField]
        protected float _reloadTime; //time in between reloading the rockets
        [SerializeField]
        protected float _destroyTime = 10.0f; //how long till the rockets get cleaned up
        protected bool _launched; //bool to check if we launched the rockets
        
        // Cached yields
        private readonly WaitForSeconds _waitToStartFiring = new WaitForSeconds(.5f);
        private WaitForSeconds _waitForFireDelay;
        private WaitForSeconds _waitForReloadTime;

        protected override void Awake()
        {
            base.Awake();
            if (_misslePositionsLeft == null && _missilePositionsRight == null)
            {
                Debug.LogError($"Tower {name} id: {this.GetInstanceID().ToString()} has " +
                               "no missile position lengths so firing will not work.");
            }
            
            if (_misslePositionsLeft.Length > 0 && _missilePositionsRight.Length > 0 &&
                _misslePositionsLeft.Length != _missilePositionsRight.Length)
            {
                Debug.LogError($"Tower {name} id: {this.GetInstanceID().ToString()} has " +
                               "mismatching missile position lengths so firing might not work.");
            }
            _missilePositionsLength = _misslePositionsLeft.Length;
            _waitForFireDelay = new WaitForSeconds(_fireDelay);
            _waitForReloadTime = new WaitForSeconds(_reloadTime);
        }

        protected override void Update()
        {
            base.Update();
            if (_targetedEnemy == null && !_resettingRotation)
            {
                StopAttacking();
                StartCoroutine(ResetRotation());
                _resettingRotation = true;
            }
            if (_targetedEnemy && !_launched)
            {
                StartCoroutine(FireRocketsRoutine());
                _launched = true;
            }
        }

        private void FireRockets(int position)
        {
            if (_misslePositionsLeft.Length > 0)
            {
                FireRocket(_misslePositionsLeft, position);
            }
            if (_missilePositionsRight.Length > 0)
            {
                FireRocket(_missilePositionsRight, position);
            }
            
        }

        private void FireRocket(GameObject[] missilePositions, int position)
        {
            // TODO: Pool rockets
            GameObject rocket = Instantiate(_missilePrefab); //instantiate a rocket

            rocket.transform.parent = missilePositions[position].transform; //set the rockets parent to the missle launch position 
            rocket.transform.localPosition = Vector3.zero; //set the rocket position values to zero
            // TODO(hack): Base missile launcher has missiles offset 90, but dual does not, how can
            // I make them consistent?
            if (TowerType == 2) // 
            {
                rocket.transform.localEulerAngles = new Vector3(-90, 0, 0); //set the rotation values to be properly aligned with the rockets forward direction
            }

            if (TowerType == 4)
            {
                rocket.transform.localEulerAngles = new Vector3(0, 0, 0); //set the rotation values to be properly aligned with the rockets forward direction
            }
            rocket.transform.parent = null; //set the rocket parent to null

            rocket.GetComponent<GameDevHQ.FileBase.Missle_Launcher.Missle.Missle>().AssignMissleRules(
                _launchSpeed, _power, _fuseDelay, _destroyTime); //assign missle properties 

            missilePositions[position].SetActive(false); //turn off the rocket sitting in the turret to make it look like it fired
        }
        
        IEnumerator FireRocketsRoutine()
        {
            // TODO: When we detect collisions, should this be more dynamic so we're more certain
            // the rocket will collide? Otherwise it'll always miss sometimes (if rot is large).
            // This also has the problem where a rocket will fire late after enemy exits and rocket
            // will fire later.
            // Wait a moment for the turret to get some rotation towards target before firing.
            yield return _waitToStartFiring;
            for (int i = 0; i < _missilePositionsLength; i++) //for loop to iterate through each missle position
            {
                // If we switch enemies mid-routine then stop firing, reload, and then start
                // shooting again. TODO: this'll need to change once it's not called in update anymore?
                if (_targetedEnemy == null)
                {
                    break;
                }
                FireRockets(i);
                yield return _waitForFireDelay; //wait for the firedelay
            }

            // Reset/reload rockets after firing.
            for (int i = 0; i < _missilePositionsLength; i++) //itterate through missle positions
            {
                yield return _waitForReloadTime; //wait for reload time
                //enable fake rocket to show ready to fire
                if(_misslePositionsLeft.Length > 0) _misslePositionsLeft[i].SetActive(true);
                if(_missilePositionsRight.Length > 0) _missilePositionsRight[i].SetActive(true);
            }

            _launched = false; //set launch bool to false
        }

        protected override void StopAttacking()
        {
            _targetedEnemy = null;
        }

        protected override void ResetFiringState()
        {
            // TODO: Make this a gatling-only concept since ML doesn't seem to use it.
            // _firingAtEnemy = false;
        }
    }
}

