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
            direction.y = 0; // Ignore vertical movement

            // Only allow left/right movement (no backward movement)
            if (direction.z < 0)
            {
                direction.z = 0; // Prevent moving backward
            }

            // Move towards the target if it's within a certain range
            if (direction.magnitude > 0.1f)
            {
                // Set the destination of the NavMeshAgent
                agent.SetDestination(target.position);

                // Rotate the agent to face the target
                if (direction.x != 0)
                    transform.rotation = Quaternion.LookRotation(new Vector3(direction.x, 0, 0));
            }
        }
    }
}
