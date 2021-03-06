﻿using System;
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

        private void Update()
        {
            // TODO: This is duplicated with Gatling impl for now, but missile collision damage
            // feature will remove this.
            if (Time.time > _canFire && _targetedEnemy && !_targetedEnemy.IsDead)
            {
                _targetedEnemy.PlayerDamageEnemy(_damageValue);
                _canFire = Time.time + _damageRate;
            }
           
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
        
        IEnumerator FireRocketsRoutine()
        {
            // TODO: When we detect collisions, should this be more dynamic so we're more certain
            // the rocket will collide? Otherwise it'll always miss sometimes (if rot is large).
            // This also has the problem where a rocket will fire late after enemy exits and rocket
            // will fire later.
            // Wait a moment for the turret to get some rotation towards target before firing.
            yield return new WaitForSeconds(.5f);
            for (int i = 0; i < _misslePositions.Length; i++) //for loop to iterate through each missle position
            {
                Debug.Log($"Tower {name} firing at target {_targetedEnemy.name}");
                FireRocket(i);
                yield return new WaitForSeconds(_fireDelay); //wait for the firedelay
            }

            // Reset rockets.
            for (int i = 0; i < _misslePositions.Length; i++) //itterate through missle positions
            {
                yield return new WaitForSeconds(_reloadTime); //wait for reload time
                _misslePositions[i].SetActive(true); //enable fake rocket to show ready to fire
            }

            _launched = false; //set launch bool to false
        }

        private void AnimateFiring()
        {
            if (!_targetedEnemy || _launched) return;
            _launched = true; //set the launch bool to true
            StartCoroutine(FireRocketsRoutine()); //start a coroutine that fires the rockets.
        }
        

        protected override void StartFiringAtEnemy(Enemy enemy)
        {
            AnimateFiring();
        }

        protected override void StopAttacking()
        {
            throw new NotImplementedException();
        }

        protected override void ResetFiringState()
        {
            _firingAtEnemy = false;
        }
    }
}

