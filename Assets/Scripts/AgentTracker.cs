using System.Collections;
using UnityEngine;
using UnityEngine.AI;

public class AgentTracker : MonoBehaviour
{
    public Transform target;
    private NavMeshAgent agent;

    [SerializeField] private bool isFloored = false; // Determines if we should use ground position
    [SerializeField] private float duration = 5f; // Duration for tracking the target
    [SerializeField] private LayerMask groundLayer; // Layer mask for the ground
    
    [SerializeField] private Transform agentObj; // Layer mask for the ground

    void Start()
    {
        agent = GetComponent<NavMeshAgent>();
        agent.updateRotation = false; 
        StartCoroutine(TrackTarget()); // Start tracking the target
    }

    private IEnumerator TrackTarget()
    {
        float elapsed = 0f;

        // Track the target for the specified duration
        while (elapsed < duration || duration == 0)
        {
            if (target)
            {
                Vector3 targetPosition;

                if (isFloored)
                {
                    // Cast a ray downwards from the target's position to find the ground
                    RaycastHit hit;
                    if (Physics.Raycast(target.position, Vector3.down, out hit, Mathf.Infinity, groundLayer))
                    {
                        targetPosition = hit.point; // Use the point where the ray hits the ground
                    }
                    else
                    {
                        targetPosition = target.position; // Fallback to target's original position if no ground found
                    }
                }
                else
                {
                    targetPosition = target.position; // Use the target's position directly
                }
                
                if (agent.isOnNavMesh)
                { 
                    agent.SetDestination(targetPosition); // Keep moving toward the target (or the ground position)
                }

                // Flip the GameObject based on movement direction
                if (agent.velocity.x != 0)
                {
                    // Set rotation to 0 if moving right, or 180 if moving left
                    Vector3 rotation = agentObj.transform.eulerAngles;
                    rotation.y = agent.velocity.x > 0 ? 0 : 180;
                    agentObj.transform.eulerAngles = rotation;
                }
            }

            elapsed += Time.deltaTime;
            yield return null; // Wait for the next frame
        }
        
        Destroy(gameObject); // Destroy the agent after the duration
    }
}
