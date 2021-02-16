using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class MechOne : MonoBehaviour
{
    [SerializeField] private GameObject _navTarget;
    
    private NavMeshAgent _navMeshAgent;
    
    // Start is called before the first frame update
    void Start()
    {
        _navMeshAgent = GetComponent<NavMeshAgent>();
        _navMeshAgent.SetDestination(_navTarget.transform.position);
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
