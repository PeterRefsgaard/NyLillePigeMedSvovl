using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CohesiveWindEffect : MonoBehaviour
{
    private ParticleSystem snowParticleSystem;
    private ParticleSystem.VelocityOverLifetimeModule velocityModule;

    public float maxXVelocity = 0.5f; // Maximum horizontal drift
    public float maxZVelocity = 0.3f; // Maximum depth drift
    public float windChangeInterval = 2f; // How often the wind changes direction

    private Vector3 currentWindDirection;

    void Start()
    {
        snowParticleSystem = GetComponent<ParticleSystem>();
        velocityModule = snowParticleSystem.velocityOverLifetime;

        // Initialize the wind direction
        SetNewWindDirection();

        // Change the wind direction periodically
        InvokeRepeating("SetNewWindDirection", 0f, windChangeInterval);
    }

    void SetNewWindDirection()
    {
        // Generate a new random wind direction (X and Z)
        currentWindDirection = new Vector3(
            Random.Range(-maxXVelocity, maxXVelocity), // X (horizontal)
            0, // Y is kept at 0 since we want snow to fall down
            Random.Range(-maxZVelocity, maxZVelocity)  // Z (depth)
        );

        // Apply the new wind direction to the entire particle system
        velocityModule.x = currentWindDirection.x;
        velocityModule.z = currentWindDirection.z;
    }
}

