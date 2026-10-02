using UnityEngine;

[RequireComponent(typeof(AudioSource))]
public class WindAudioManager : MonoBehaviour
{
    [Header("Dependencies")] [SerializeField]
    private GameManager gameManager;

    private AudioSource windAudioSource;
    
    [Header("Settings")]
    [Tooltip("Max hastighet = ljud 100% volym")]
    [SerializeField] private float maxFallSpeed = 30f;
    [Tooltip("Högre siffra = snabbare anpassning i ljudvolymsökning/minskning")]
    [SerializeField] private float volumeLerpSpeed = 5f;
    [Tooltip("Maxvolym för vindljud")]
    [Range(0f, 1f)]
    [SerializeField] private float maxVolume = 1f;
    [Tooltip("1 = linjär logaritmiskt kurva. Högre än 1 = ljudet ökar långsammare i början.")]
    [SerializeField] private float logarithmicFactor = 2f;

    void Awake()
    {
        windAudioSource = GetComponent<AudioSource>();
        
        if (gameManager == null)
        {
            gameManager = GameManager.instance;
        }
        
        windAudioSource.loop = true;
        windAudioSource.volume = 0f;
        
        if (!windAudioSource.isPlaying)
        {
            windAudioSource.Play();
        }
    }

    void Update()
    {
        if (gameManager == null) return;
        
        if (gameManager.Altitude <= 0.01f)
        {
            windAudioSource.volume = 0f;
            return; 
        }

        float currentVelocity = gameManager.VerticalVelocity;
        float targetVolume = 0f;

        if (currentVelocity < 0f)
        {
            float normalizedSpeed = Mathf.Clamp01(Mathf.Abs(currentVelocity) / maxFallSpeed);
            targetVolume = Mathf.Pow(normalizedSpeed, logarithmicFactor) * maxVolume;
        }
        
        windAudioSource.volume = Mathf.Lerp(windAudioSource.volume, targetVolume, volumeLerpSpeed * Time.deltaTime);
    }
}
