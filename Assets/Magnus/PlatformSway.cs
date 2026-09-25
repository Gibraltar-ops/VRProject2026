using UnityEngine;

/// <summary>
///  Script för att skapa illusionen av att ballongkorgen svajjar
/// </summary>

public class PlatformSway : MonoBehaviour
{
  [Header("Referenser")] 
  [SerializeField] private PlatformDynamics platform;
  
  [Header("Sway inställningar")] 
  [SerializeField] private float maxSwayAngleX = 2.0f; // Max rotation fram/bak (grader)
  [SerializeField] private float maxSwayAngleZ = 1.5f; // Max rotation i sidled (grader 
  [SerializeField] private float swaySpeed = 0.5f; // Hur snabbt korgen svajjar
  
  [SerializeField] private float swayFadeDistance = 5.0f; // Hur högt behöver vi vara innan maximalt svajj?

  // perlin noise för att skapa random offset i svajjet 
  private float noiseOffsetX;
  private float noiseOffsetZ;

  private void Start()
  {
    noiseOffsetX = Random.Range(0f, 1000f);
    noiseOffsetZ = Random.Range(1000f, 2000f);
  }
  private void Update()
  {
    if (platform == null) return;

    float currentAltitude = platform.GetAltitude();
    float altitudeSwayMultiplier = Mathf.Clamp01(currentAltitude / swayFadeDistance);
    
    float noiseX = Mathf.PerlinNoise(Time.time * swaySpeed + noiseOffsetX, 0f);
    float noiseZ = Mathf.PerlinNoise(0f, Time.time * swaySpeed + noiseOffsetZ);
    
    float normalizedNoiseX = (noiseX * 2f) - 1f;
    float normalizedNoiseZ = (noiseZ * 2f) - 1f;
    
    float currentSwayX = normalizedNoiseX * maxSwayAngleX * altitudeSwayMultiplier;
    float currentSwayZ = normalizedNoiseZ * maxSwayAngleZ * altitudeSwayMultiplier;

    transform.localRotation = Quaternion.Euler(currentSwayX, 0f, currentSwayZ);
  }
}
