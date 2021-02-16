using UnityEngine;
using UnityEngine.AI;

public class MechTwo : MonoBehaviour
{
    [SerializeField] private GameObject _navTarget;
    
    private NavMeshAgent _navMeshAgent;
    
    void Start()
    {
        _navMeshAgent = GetComponent<NavMeshAgent>();
        _navMeshAgent.SetDestination(_navTarget.transform.position);
    }
}