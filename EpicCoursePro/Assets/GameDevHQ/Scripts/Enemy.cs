using UnityEngine;
using UnityEngine.AI;

namespace GameDevHQ.Scripts
{
    public class Enemy : MonoBehaviour
    {
        [field: Header("Navigation and speed components")]
        [field: SerializeField]
        [field: Tooltip("The end destination the enemy will attempt to navigate to.")]
        public GameObject NavTarget { get; set;}

        [field: SerializeField]
        [field: Tooltip("The speed at which the enemy will approach the navigation target.")]
        protected float _navigationSpeed = 1.5f;

        private NavMeshAgent _navMeshAgent;

        [SerializeField] 
        [Header("Health Settings")]
        private int health = 100;

        [Header("Currency Settings")]
        [SerializeField] 
        [Tooltip("The currency value provided when the enemy is killed by player.")]
        private int warFundValue = 100;
       
        private void Start()
        {
            _navMeshAgent = GetComponent<NavMeshAgent>();
            NavTarget = SpawnManager.Instance.GetEnemySpawnEndPoint();
           
            if (_navMeshAgent == null)
            {
                Debug.LogError($"Nav mesh agent is null on enemy: {this.gameObject.name}");
            }
            if (NavTarget == null)
            {
                Debug.LogError($"Nav target for enemy: {this.gameObject.name} is null.");
            }
            
            _navMeshAgent.SetDestination(NavTarget.transform.position);
            _navMeshAgent.speed = _navigationSpeed;
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
