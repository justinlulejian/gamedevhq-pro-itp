using System;
using UnityEngine;
using UnityEngine.AI;

namespace GameDevHQ.Scripts
{
    public class Enemy : MonoBehaviour
    {
        [field: SerializeField]
        [field: Tooltip("The end destination the enemy will attempt to navigate to.")]
        [field: Header("Navigation components")]
        public GameObject NavTarget { get; set;}

        private NavMeshAgent _navMeshAgent;

        [SerializeField] [Header("Health Settings")]
        private int health = 100;

        [Header("Currency Settings")]
        [SerializeField] [Tooltip("The currency value provided when the enemy is killed by player.")]
        private int warFundValue = 100;
       
        private void Start()
        {
            _navMeshAgent = GetComponent<NavMeshAgent>();
            
            if (_navMeshAgent == null)
            {
                Debug.LogError($"Nav mesh agent is null on enemy: {this.gameObject.name}");
            }
            if (NavTarget == null)
            {
                Debug.LogError($"Nav target for enemy: {this.gameObject.name} is null.");
            }
            
            _navMeshAgent.SetDestination(NavTarget.transform.position);
        }
        
        public void Damage(int damageValue)
        {
            Mathf.Min(0, health -= damageValue);

            if (health <= 0)
            {
                Kill();
            }
        }
        
        private void Kill()
        {
            Destroy(this.gameObject);
        }
    }
}
