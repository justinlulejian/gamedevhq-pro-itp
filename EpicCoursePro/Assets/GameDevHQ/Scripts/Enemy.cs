using UnityEngine;
using UnityEngine.AI;

namespace GameDevHQ.Scripts
{
    public class Enemy : MonoBehaviour
    {
        [SerializeField] [Tooltip("The end destination the enemy will attempt to navigate to.")][Header("Navigation components")]
        private GameObject _navTarget;
        private NavMeshAgent _navMeshAgent;

        [SerializeField] [Header("Health Settings")]
        private int _health = 100;

        [Header("Currency Settings")]
        [SerializeField] [Tooltip("The currency value provided when the enemy is killed by player.")]
        private int warFundValue = 100;
       
        void Start()
        {
            _navMeshAgent = GetComponent<NavMeshAgent>();
            
            if (_navMeshAgent == null)
            {
                Debug.LogError($"Nav mesh agent is null on enemy: {this.gameObject.name}");
            }
            
            _navMeshAgent.SetDestination(_navTarget.transform.position);
        }
        
        public void Damage(int damageValue)
        {
            Mathf.Min(0, _health -= damageValue);

            if (_health <= 0)
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
