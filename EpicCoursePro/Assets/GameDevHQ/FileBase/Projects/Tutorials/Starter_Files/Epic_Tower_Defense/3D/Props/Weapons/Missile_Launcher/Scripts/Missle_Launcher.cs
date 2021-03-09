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

        protected override void Update()
        {
            base.Update();
            // TODO: This is duplicated with Gatling impl for now, but missile collision damage
            // feature will remove this.
            if (Time.time > _canFire && _targetedEnemy && !_targetedEnemy.IsDead)
            {
                _targetedEnemy.PlayerDamageEnemy(_damageValue);
                _canFire = Time.time + _damageRate;
            }

            if (_targetedEnemy && !_launched)
            {
                StartCoroutine(FireRocketsRoutine());
                _launched = true;
            }
        }

        private void FireRocket(int i)
        {
            // TODO: Pool rockets
            // GameObject rocket = Instantiate(_missilePrefab) as GameObject; //instantiate a rocket
            GameObject rocket = PoolManager.Instance.RequestObjOfType(_missilePrefab);
            rocket.SetActive(true);
            
            Transform rocketOriginalParent = rocket.transform.parent;
            rocket.transform.parent = _misslePositions[i].transform; //set the rockets parent to the missile launch position 
            rocket.transform.localPosition = Vector3.zero; //set the rocket position values to zero
            rocket.transform.localEulerAngles = new Vector3(-90, 0, 0); //set the rotation values to be properly aligned with the rockets forward direction
            rocket.transform.parent = rocketOriginalParent; //set the rocket parent to null
            Debug.Log($"Missile {this.GetInstanceID().ToString()}: Rocket position: {rocket.transform.position.ToString()} localposition:" +
                      $" {rocket.transform.localPosition.ToString()}");
            Debug.Log($"Missile {this.GetInstanceID().ToString()}: Rocket rotation: {rocket.transform.rotation.ToString()} localrotation:" +
                      $" {rocket.transform.localRotation.ToString()}");

            Missle.Missle missile =
                rocket.GetComponent<GameDevHQ.FileBase.Missle_Launcher.Missle.Missle>();
            missile.AssignMissleRules(
                _launchSpeed, _power, _fuseDelay, _destroyTime); //assign missile properties 
            StartCoroutine(missile.Start());

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
                // If we switch enemies mid-routine then stop firing, reload, and then start
                // shooting again. TODO: this'll need to change once it's not called in update anymore?
                if (_targetedEnemy == null)
                {
                    break;
                }
                FireRocket(i);
                yield return new WaitForSeconds(_fireDelay); //wait for the firedelay
            }

            // Reset/reload rockets after firing.
            for (int i = 0; i < _misslePositions.Length; i++) //itterate through missle positions
            {
                yield return new WaitForSeconds(_reloadTime); //wait for reload time
                _misslePositions[i].SetActive(true); //enable fake rocket to show ready to fire
            }

            _launched = false; //set launch bool to false
        }


        protected override void StartFiringAtEnemy(Enemy enemy)
        {
        }

        protected override void StopAttacking()
        {
            _targetedEnemy = null;
        }

        protected override void ResetFiringState()
        {
            _firingAtEnemy = false;
        }
    }
}

