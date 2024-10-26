using UnityEngine;

public class ParticleLine : MonoBehaviour
{
    public ParticleSystem pSystem;  // Particle system reference
    public Transform startPoint;     // Starting point of the particles
    public Transform endPoint;       // End point of the particles
    [SerializeField] float killRadius;
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
        CheckParticlesForProximity();
    }

    void CheckParticlesForProximity()
{
    ParticleSystem.Particle[] particles = new ParticleSystem.Particle[pSystem.particleCount];
    int numParticlesAlive = pSystem.GetParticles(particles);

    // Get the particle system's transform for local-to-world space conversion
    Transform particleSystemTransform = pSystem.transform;

    // Loop through each particle and check if it’s within the killRadius of the endPoint
    for (int i = 0; i < numParticlesAlive; i++)
    {
        // Convert the particle's local position to world space
        Vector3 particleWorldPos = particleSystemTransform.TransformPoint(particles[i].position);

        // Check the distance between the particle's world position and the end point
        float distanceToEndPoint = Vector3.Distance(particleWorldPos, endPoint.position);


        // If the particle is within the kill radius, set its lifetime to 0
        if (distanceToEndPoint < killRadius)
        {
            particles[i].remainingLifetime = 0;  // Make the particle die
        }
        
    }

    // Update the particle system with the modified particles
    pSystem.SetParticles(particles, numParticlesAlive);
}

}
