using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using GameDevHQ.Scripts;
using UnityEditor.Rendering;
using UnityEngine;
using Debug = UnityEngine.Debug;


namespace GameDevHQ.FileBase.Gatling_Gun
{
    /// <summary>
    /// This script will allow you to view the presentation of the Turret and use it within your project.
    /// Please feel free to extend this script however you'd like. To access this script from another script
    /// (Script Communication using GetComponent) -- You must include the namespace (using statements) at the top. 
    /// "using GameDevHQ.FileBase.Gatling_Gun" without the quotes. 
    /// 
    /// For more, visit GameDevHQ.com
    /// 
    /// @authors
    /// Al Heck
    /// Jonathan Weinberger
    /// </summary>

    [RequireComponent(typeof(AudioSource))] //Require Audio Source component
    public class Gatling_Gun : Tower
    {
        [SerializeField] 
        private GameObject _gunBarrelObj;
        private Transform _gunBarrel; //Reference to hold the gun barrel
        public GameObject Muzzle_Flash; //reference to the muzzle flash effect to play when firing
        public ParticleSystem bulletCasings; //reference to the bullet casing effect to play when firing
        public AudioClip fireSound; //Reference to the audio clip

        private AudioSource _audioSource; //reference to the audio source component
        private bool _startWeaponNoise = true;
        
        // Use this for initialization
        void Start()
        {
            _gunBarrel = _gunBarrelObj.GetComponent<Transform>(); //assigning the transform of the gun barrel to the variable

            if (_gunBarrel == null)
            {
                Debug.LogError($"Tower {name} id: {this.GetInstanceID().ToString()} is " +
                               $"missing it's gun barrel transform.");
            }
            
            Muzzle_Flash.SetActive(false); //setting the initial state of the muzzle flash effect to off
            _audioSource = GetComponent<AudioSource>(); // assign the Audio Source to the reference variable
            
            if (_gunBarrel == null)
            {
                Debug.LogError($"Tower {name} id: {this.GetInstanceID().ToString()} is " +
                               $"missing it's audio source.");
            }
            _audioSource.playOnAwake = false; //disabling play on awake
            _audioSource.loop = true; //making sure our sound effect loops
            _audioSource.clip = fireSound; //assign the clip to play

            StartCoroutine(FiringRoutine());
        }

        private IEnumerator FiringRoutine()
        {
            yield return new WaitUntil(() => _currentTargetedEnemy != null);
            Enemy originalEnemyTarget = _currentTargetedEnemy;
            Muzzle_Flash.SetActive(true); //enable muzzle effect particle effect
            _audioSource.Play();
            bulletCasings.Play();
            while (true)
            {
                // Switching targets.
                if (_currentTargetedEnemy != originalEnemyTarget)
                {
                    break;
                }
                SpinBarrel();
                RotateTowardsTarget();
                yield return null;
            }

            // Reset firing state to idle.
            if (_currentTargetedEnemy == null)
            {
                Muzzle_Flash.SetActive(false);
                _audioSource.Stop();
                bulletCasings.Stop();
            }
            // TODO: Could this be refactored to be a continuously running coroutine?
            StartCoroutine(FiringRoutine());
        }

        // protected override void Update()
        // {
        //     base.Update();
        //     // TODO: Does this need to be called in update or could it be another less updated
        //     // method like a coroutine?
        //     if (Time.time > _canFire && _currentTargetedEnemy && !_currentTargetedEnemy.IsDead)
        //     {
        //         _currentTargetedEnemy.PlayerDamageEnemy(_damageValue);
        //         _canFire = Time.time + _damageRate; //
        //     }
        //     if (_currentTargetedEnemy != null) 
        //     {
        //         SpinBarrel();
        //         bulletCasings.Emit(1); //Emit the bullet casing particle effect 
        //     }
        //     
        //     if (_currentTargetedEnemy && !_firingAtEnemy)
        //     {
        //         Muzzle_Flash.SetActive(true); //enable muzzle effect particle effect
        //         _audioSource.Play(); //play audio clip attached to audio source
        //         _firingAtEnemy = true;  // Only initiate this one per firing instance.
        //     }
        // }

        // Method to rotate gun barrel 
        void SpinBarrel() 
        {
            // TODO: Make this a slower spin up effect with lerp?
            //rotate the gun barrel along the "forward" (z) axis at 500 meters per second
            _gunBarrel.transform.Rotate(Vector3.forward * (Time.deltaTime * -500.0f));

        }

        protected override void StopAttacking()
        {
            _currentTargetedEnemy = null;
            _firingAtEnemy = false;
            Muzzle_Flash.SetActive(false);
            _audioSource.Stop();
        }
    }
}
