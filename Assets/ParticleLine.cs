using UnityEngine;

public class ParticleLine : MonoBehaviour
{
    public ParticleSystem pSystem;  // Particle system reference
    public Transform startPoint;     // Starting point of the particles
    public Transform endPoint;       // End point of the particles
    public string targetTag = "Player"; // Tag to check for collisions

    private ParticleSystem.ShapeModule shapeModule;
    private ParticleSystem.MainModule mainModule;
    private ParticleSystem.CollisionModule collisionModule;

    void Start()
    {
        if (pSystem == null)
        {
            Debug.LogError("ParticleSystem is not assigned.");
            return;
        }

        // Get the ShapeModule for configuring the shape as an edge
        shapeModule = pSystem.shape;
        shapeModule.shapeType = ParticleSystemShapeType.SingleSidedEdge; // Set shape to "Edge"

        // Get the MainModule for configuring properties
        mainModule = pSystem.main;

        // Get the CollisionModule to enable collision with other objects
        collisionModule = pSystem.collision;
        collisionModule.enabled = true; // Enable collision
        collisionModule.collidesWith = LayerMask.GetMask("Default"); // Ensure this matches your target layer
    }

    void Update()
    {
        if (pSystem == null || startPoint == null || endPoint == null) return;

        // Calculate the direction from startPoint to endPoint
        Vector3 direction = endPoint.position - startPoint.position;

        // Set the particle system's position to the start point
        pSystem.transform.position = startPoint.position;

        // Set the rotation to face the end point
        pSystem.transform.rotation = Quaternion.LookRotation(direction, Vector3.up);

        // Ensure the particle system is playing
        if (!pSystem.isPlaying)
        {
            pSystem.Play();
        }

        // Check for overlapping particles with the target collider
        CheckParticleTriggers();
    }

    void CheckParticleTriggers()
    {
        ParticleSystem.Particle[] particles = new ParticleSystem.Particle[pSystem.particleCount];
        int numParticles = pSystem.GetParticles(particles);

        for (int i = 0; i < numParticles; i++)
        {
            // Create a sphere at the particle's position to check for overlaps
            Collider[] hitColliders = Physics.OverlapSphere(particles[i].position, 0.1f); // Adjust the radius as needed

            foreach (var hitCollider in hitColliders)
            {
                // Check if the colliding object has the specified tag
                if(hitCollider.transform.parent != null){
                    if (hitCollider.transform.parent.CompareTag(targetTag + "Player"))
                    {
                        // Set the particle's lifetime to 0 to make it "die"
                        particles[i].remainingLifetime = 0;
                        Debug.Log("Particle triggered on: " + hitCollider.name);
                    }
                }
                
            }
        }

        // Update the particle system with the modified particles
        pSystem.SetParticles(particles, numParticles);
    }
}
