using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class AgentTracker : MonoBehaviour
{
    public Transform target; // Target to move towards
    private NavMeshAgent agent;
    [SerializeField] private LayerMask groundLayer; // Layer mask for ground

    void Start()
    {
        agent = GetComponent<NavMeshAgent>();
        agent.updateRotation = false; // Disable automatic rotation
    }

    void Update()
    {
        if (target)
        {
            // Calculate the direction to the target
            Vector3 direction = target.position - transform.position;
            agent.SetDestination(target.position);
        }
    }
}
