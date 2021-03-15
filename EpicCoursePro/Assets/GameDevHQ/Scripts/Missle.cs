using System;
using System.Collections;
using System.Collections.Generic;
using GameDevHQ.Scripts;
using UnityEngine;
using UnityEngine.Rendering;
using Random = UnityEngine.Random;

namespace GameDevHQ.FileBase.Missle_Launcher.Missle
{
    [RequireComponent(typeof(Rigidbody))] //require rigidbody
    [RequireComponent(typeof(AudioSource))] //require audiosource
    public class Missle : MonoBehaviour
    {
        [SerializeField]
        private List<ParticleSystem> _particleSystems; //reference to the particle system

        [SerializeField] 
        private CapsuleCollider _missileCollider;
        [SerializeField] 
        private MeshRenderer _meshRenderer;
        public int RocketDamageValue = 100;
        private bool _collidedWithEnemy;

        [SerializeField]
        private float _launchSpeed; //launch speed of the rocket
        [SerializeField]
        private float _power; //power of the rocket
        [SerializeField] //fuse delay of the rocket
        private float _fuseDelay;

        private Rigidbody _rigidbody; //reference to the rigidbody of the rocket
        private AudioSource _audioSource; //reference to the audiosource of the rocket

        private bool _firing;  // Whether rocket has been "armed"/launched to fire at enemy.
        private bool _launched = false; //bool for if the rocket has launched
        private float _initialLaunchTime = 2.0f; //initial launch time for the rocket
        private bool _thrust; //bool to enable the rocket thrusters

        private bool _fuseOut = false; //bool for if the rocket fuse
        private bool _trackRotation = false; //bool to track rotation of the rocket

        // public static event Action<Missle, GameObject> onMissileHitEnemy; 

        private void Awake()
        {
            _missileCollider = GetComponentInChildren<CapsuleCollider>();
            _meshRenderer = GetComponentInChildren<MeshRenderer>();
            if (_missileCollider == null)
            {
                Debug.LogError(
                    $"Collider on missile {this.GetInstanceID().ToString()} is " +
                    $"not present, enemy hit detection may break.");
            }
            if (_meshRenderer == null)
            {
                Debug.LogError(
                    $"Mesh renderer on missile {this.GetInstanceID().ToString()} is " +
                    $"not present, enemy hit detection may break.");
            }
        }


        // Use this for initialization
        IEnumerator Start()
        {
            _rigidbody = GetComponent<Rigidbody>(); //assign the rigidbody component 
            _audioSource = GetComponent<AudioSource>(); //assign the audiosource component
            _audioSource.pitch = Random.Range(0.7f, 1.9f); //randomize the pitch of the rocket audio
            _particleSystems.ForEach(p => p.Play()); //play the particles of the rocket
            _audioSource.Play(); //play the rocket sound

            yield return new WaitForSeconds(_fuseDelay); //wait for the fuse delay

            _initialLaunchTime = Time.time + 1.0f; //set the initial launch time
            _fuseOut = true; //set fuseOut to true
            _launched = true; //set the launch bool to true 
            _thrust = false; //set thrust bool to false

        }


        // Update is called once per frame
        void FixedUpdate()
        {
            if (_fuseOut == false) //check if fuseOut is false
                return;

            if (_launched == true) //check if launched is true
            {
                _firing = true;
                _rigidbody.AddForce(transform.forward * _launchSpeed); //add force to the rocket in the forward direction

                if (Time.time > _initialLaunchTime + _fuseDelay) //check if the initial launch + fuse delay has passed
                {
                    _launched = false; //launched bool goes false
                    _thrust = true; //thrust bool goes true
                }
            }

            if (_thrust == true) //if thrust is true
            {
                _rigidbody.useGravity = true; //enable gravity 
                _rigidbody.velocity = transform.forward * _power; //set velocity multiplied by the power variable
                _thrust = false; //set thrust bool to false
                _trackRotation = true; //track rotation bool set to true
            }
            
            // TODO: Turn this into homing towards a target.
            if (_trackRotation == true) //check track rotation bool
            {
                _rigidbody.rotation = Quaternion.LookRotation(_rigidbody.velocity); // adjust rotation of rocket based on velocity
                _rigidbody.AddForce(transform.forward * 100f); //add force to the rocket
            }
        }

        private void OnTriggerEnter(Collider other)
        {
            if (!_firing) return;
            if (!other.CompareTag("Enemy")) return;
            if (_collidedWithEnemy) return;
            _collidedWithEnemy = true;

            DamageEnemy(other.gameObject);
            StopMissileInteractions();
            
            // Convert to pooling
            Destroy(this.gameObject, 5f); //destroy the rocket after anim should finish playing. 
        }
        
        private void DamageEnemy(GameObject enemyObj)
        {
            Enemy enemy = enemyObj.GetComponent<Enemy>();
            enemy.PlayerDamageEnemy(RocketDamageValue);
        }
        
        private void StopMissileInteractions()
        {
            _particleSystems.ForEach(p => p.Stop());
            _audioSource.Stop();
            _meshRenderer.enabled = false;
            _missileCollider.enabled = false;
            _launched = false;
            _thrust = false;
            _trackRotation = false;
            _fuseOut = false;
            _firing = false;
        }

        /// <summary>
        /// This method is used to assign traits to our missle assigned from the launcher.
        /// </summary>
        public void AssignMissleRules(float launchSpeed, float power, float fuseDelay, float destroyTimer)
        {
            _launchSpeed = launchSpeed; //set the launch speed
            _power = power; //set the power
            _fuseDelay = fuseDelay; //set the fuse delay
            Destroy(this.gameObject, destroyTimer); //destroy the rocket after destroyTimer 
        }
    }
}

