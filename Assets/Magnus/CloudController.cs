using UnityEngine;

public class CloudController : MonoBehaviour
{
    [Header("Dependencies")] [Tooltip("Partikelsystem på ballong här")] [SerializeField]
    private ParticleSystem cloudParticles;

    [Header("Cloud config")] [Tooltip("Avstånd i Y-led från det att molnen påverkar spelaren")] [SerializeField]
    private float cloudThickness = 100f;

    [Header("Particle control")] [Tooltip("Partiklar per sekund när vi är i molnen")] [SerializeField]
    private float maxEmissionRate = 50f;
    
    [Tooltip("X-axeln: 0 (Mitt i molnet) till 1 (Utanför molnet). Y-axeln: Emission multiplier (0 till 1).")] [SerializeField] 
    private AnimationCurve emissionCurve = AnimationCurve.Linear(0f, 1f, 1f, 0f);

    private ParticleSystem.EmissionModule emissionModule;

    private void Start()
    {
        if (cloudParticles != null)
        {
            emissionModule = cloudParticles.emission;
            
        }
        else
        {
            Debug.LogError("Referens till partikelsystem saknas ");
        }
    }

    private void FixedUpdate()
    {
        if (GameManager.instance == null) return;
        
        float currentAltitude = GameManager.instance.Altitude;
        float cloudBaseAltitude = GameManager.instance.CloudBaseAltitude;
        
        float newYPosition = currentAltitude + cloudThickness;
    }
}