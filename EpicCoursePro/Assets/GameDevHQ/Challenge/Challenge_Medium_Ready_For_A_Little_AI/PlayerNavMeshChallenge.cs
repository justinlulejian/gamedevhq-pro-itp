using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class PlayerNavMeshChallenge : MonoBehaviour
{
    [SerializeField]
    private GameObject _target;
    private NavMeshAgent _navMeshAgent;
    // Start is called before the first frame update
    void Start()
    {
        _navMeshAgent = GetComponent<NavMeshAgent>();
        _navMeshAgent.SetDestination(_target.transform.position);
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
