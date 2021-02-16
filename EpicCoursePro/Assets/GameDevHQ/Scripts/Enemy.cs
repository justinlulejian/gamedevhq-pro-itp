using UnityEngine;
using UnityEngine.AI;

namespace GameDevHQ.Scripts
{
    public class Enemy : MonoBehaviour
    {
        [SerializeField] 
        private GameObject _navTarget;

        private NavMeshAgent _navMeshAgent;
        // Start is called before the first frame update
        void Start()
        {
            _navMeshAgent = GetComponent<NavMeshAgent>();
            _navMeshAgent.SetDestination(_navTarget.transform.position);

        }
    
    }
}
