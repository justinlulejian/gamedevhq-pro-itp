using UnityEngine;
using UnityEngine.AI;

namespace GameDevHQ.Scripts
{
    public class Enemy : MonoBehaviour
    {
        [SerializeField] 
        private GameObject _navTarget;

        [SerializeField] 
        private int _health = 100;

        private NavMeshAgent _navMeshAgent;
        void Start()
        {
            _navMeshAgent = GetComponent<NavMeshAgent>();
            _navMeshAgent.SetDestination(_navTarget.transform.position);

        }

        public void Damage(int damageValue)
        {
            Mathf.Min(0, _health -= damageValue);
        }
    }
}
