using System.Collections.Generic;
using UnityEngine;
using Debug = UnityEngine.Debug;


namespace GameDevHQ.Scripts
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
        private List<Transform> _gunBarrels = new List<Transform>(); //Reference to hold the gun barrel
        [SerializeField] 
        private List<GameObject> _muzzleFlashes = new List<GameObject>(); //reference to the muzzle flash effect to play when firing
        [SerializeField] 
        private List<ParticleSystem> _bulletCasings = new List<ParticleSystem>(); //reference to the bullet casing effect to play when firing
        [SerializeField] 
        private AudioClip _fireSound; //Reference to the audio clip

        private AudioSource _audioSource; //reference to the audio source component
        
        void Start()
        {
            if (_gunBarrels.Count < 1 )
            {
                Debug.LogError($"Tower {name} id: {this.GetInstanceID().ToString()} is " +
                               $"missing it's gun barrel transform(s).");
            }
            if (_muzzleFlashes.Count < 1 )
            {
                Debug.LogError($"Tower {name} id: {this.GetInstanceID().ToString()} is " +
                               $"missing it's muzzle flash object(s).");
            }
            if (_bulletCasings.Count < 1 )
            {
                Debug.LogError($"Tower {name} id: {this.GetInstanceID().ToString()} is " +
                               $"missing it's bullet case particle system object(s).");
            }
            if (_fireSound == null)
            {
                Debug.LogError($"Tower {name} id: {this.GetInstanceID().ToString()} is " +
                               $"missing it's firing sound.");
            }

            //setting the initial state of the muzzle flash effect to off
            foreach (var muzzleFlash in _muzzleFlashes)
            {
                muzzleFlash.SetActive(false);
            }
            _audioSource = GetComponent<AudioSource>(); // assign the Audio Source to the reference variable
            _audioSource.playOnAwake = false; //disabling play on awake
            _audioSource.loop = true; //making sure our sound effect loops
            _audioSource.clip = _fireSound; //assign the clip to play
        }

        protected override void Update()
        {
            base.Update();
            // TODO: Does this need to be called in update or could it be another less updated
            // method like a coroutine?
            if (_targetedEnemy == null && !_resettingRotation)
            {
                StopAttacking();
                StartCoroutine(ResetRotation());
                _resettingRotation = true;
            }
            if (Time.time > _canFire && _targetedEnemy != null)
            {
                _targetedEnemy.PlayerDamageEnemy(_damageValue);
                _canFire = Time.time + _damageRate; //
            }
            if (_targetedEnemy != null) 
            {
                RotateBarrel();
                foreach (var bulletCasing in _bulletCasings)
                {
                    bulletCasing.Emit(1); //Emit the bullet casing particle effect 
                }
               
            }
            
            if (_targetedEnemy != null && !_firingAtEnemy)
            {
                foreach (var muzzleFlash in _muzzleFlashes)
                {
                    muzzleFlash.SetActive(true); //enable muzzle effect particle effect
                }
                _audioSource.Play(); //play audio clip attached to audio source
                _firingAtEnemy = true;  // Only initiate this one per firing instance.
            }
            
            
        }

        // Method to rotate gun barrel 
        void RotateBarrel() 
        {
            // TODO: Make this a slower spin up effect with lerp?
            //rotate the gun barrel along the "forward" (z) axis at 500 meters per second
            foreach (var gunBarrel in _gunBarrels)
            {
                gunBarrel.transform.Rotate(Vector3.forward * (Time.deltaTime * -500.0f));
            }
        }

        protected override void StopAttacking()
        {
            _targetedEnemy = null;
            _firingAtEnemy = false;
            foreach (var muzzleFlash in _muzzleFlashes)
            {
                muzzleFlash.SetActive(false);
            }
            _audioSource.Stop();
            ResetFiringState();
        }

        protected override void ResetFiringState()
        {
            _firingAtEnemy = false;
            foreach (var muzzleFlash in _muzzleFlashes)
            {
                muzzleFlash.SetActive(false);
            }
        }
    }
}
