using UnityEngine;

public class CloudController : MonoBehaviour
{
    [Header("Dependencies")] [Tooltip("Partikelsystem på ballong här")] [SerializeField]
    private ParticleSystem cloudParticles;

    [Header("Cloud config")] [Tooltip("Avstånd i Y-led från det att molnen påverkar spelaren")] [SerializeField]
    private float cloudThickness = 100.0f;

    [Header("Particle control")] [Tooltip("Partiklar per sekund när vi är i molnen")] [SerializeField]
    private float maxEmissionRate = 50f;
    
    [Tooltip("X-axeln: 0 (Mitt i moln) till 1 (Utanför moln). Y: Emission multiplier (0 - 1).")] [SerializeField] 
    private AnimationCurve emissionCurve = AnimationCurve.Linear(0f, 1f, 1f, 0f);

    private ParticleSystem.EmissionModule emissionModule;
    private ParticleSystem.VelocityOverLifetimeModule velocityModule;

    private void Start()
    {
        if (cloudParticles != null)
        {
            emissionModule = cloudParticles.emission;
            velocityModule = cloudParticles.velocityOverLifetime;
            
            velocityModule.enabled = true;
            
        }
        else
        {
            Debug.LogError("Saknar referens till partikelsystem ");
        }
    }

    private void FixedUpdate()
    {
        if (GameManager.instance == null) return;
        
        float currentAltitude = GameManager.instance.Altitude;
        float cloudBaseAltitude = GameManager.instance.CloudBaseAltitude;
        float currentVerticalVelocity = GameManager.instance.VerticalVelocity;

        float newYPosition = cloudBaseAltitude - currentAltitude;
        transform.position = new Vector3(transform.position.x, newYPosition, transform.position.z);

        UpdateParticleEmission(newYPosition);
        
        UpdateParticleVelocity(currentVerticalVelocity);
    }

    private void UpdateParticleEmission(float cloudYPosition)
    {
        if (cloudParticles == null) return;
        
        // totalt avstånd från origo till molnets centrum
        float distanceToCloudCenter = Mathf.Abs(cloudYPosition);

        float normalizedDistance = distanceToCloudCenter / (cloudThickness * 0.5f);
        normalizedDistance = Mathf.Clamp01(normalizedDistance);

        float evaluatedMultiplier = emissionCurve.Evaluate(normalizedDistance);

        emissionModule.rateOverTime = maxEmissionRate * evaluatedMultiplier;
    }
    
    private void UpdateParticleVelocity(float verticalVelocity)
    {
        if (cloudParticles == null) return;
        
        float invertedVelocity = -verticalVelocity;
        velocityModule.y = new ParticleSystem.MinMaxCurve(invertedVelocity);
    }
}