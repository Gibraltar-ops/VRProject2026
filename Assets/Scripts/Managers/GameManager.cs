using System.Collections;
using NUnit.Framework;
using TMPro;
using UnityEngine;
using UnityEngine.InputSystem.XR.Haptics;
using UnityEngine.XR.Interaction.Toolkit.Inputs.Haptics;

public class GameManager : MonoBehaviour
{
    public static GameManager instance = null;

    [Header("Systems")]

    public BalloonTree PlayerBaloonTree;
    
    [Header("VR Camera reference")]
    [SerializeField] private Transform vrCameraTransform;

    public int ActivePlayerBalloons
    {
        get{return PlayerBaloonTree.ActiveBalloons;}
    }
    
    [Header("Haptics")]
    [Tooltip("Referens till Haptic Impulse Player på vänster hand")]
    public HapticImpulsePlayer leftHandHaptics;
    [Tooltip("Referens till Haptic Impulse Player på höger hand")]
    public HapticImpulsePlayer rightHandHaptics;
    [SerializeField] private float maxHapticVelocity = -30f;

    [Header("Altitude")]
    public float Altitude{get; private set;}
    public float VerticalVelocity { get; private set; }
    public float GroundAltitude { get; private set; }   
    public float CloudBaseAltitude{get;} = 700f;
    public float WinAltitude {get; private set; } = 2000f;
    
    [Header("Balans/Thresholds")]
    [SerializeField] private float liftMultiplier = 1.5f;
    [SerializeField] private float gravityMultiplier = 5.0f;

    [Tooltip("Hur många ballonger som behövs för att hovra")]
    [SerializeField] private int hoverThreshold = 10;

    [Tooltip("Hur få ballonger som behövs för att hamna i fritt fall")]
    [SerializeField] private int freefallThreshold = 3;
    

    [Tooltip("Hur mycket acceleration/lift multiplier påverkas per uppblåst ballong")]
    [SerializeField] private float acceleration = 1.25f;
    
    [Header("Fritt fall - justeringar")]
    [SerializeField] private float maxFreeFallSpeed = 40.0f;
    [Tooltip("Accelerering för fritt fall - 9.8 = jordens tyngdkraft")]
    [SerializeField] private float freefallAcceleration = 9.8f;
    
    [Header("Impact ")]
    [SerializeField] private float fatalImpactVelocity = -20f;
    [SerializeField] private AudioClip crashSound;
    [SerializeField] private AudioSource audioSource;
    [SerializeField] private ParticleSystem crashParticlePrefab;
    
    [Header("Win Sequence ")]
    [SerializeField] private AudioClip winSound;
    [SerializeField] private GameObject winUIPrefab;
    
    [SerializeField] private GameObject airplanePrefab;
    
    [SerializeField] private float winAirplaneFlyDuration = 6.0f; 
    [SerializeField] private float winAirplaneSpawnDistance = 600f;
    [SerializeField] private float winAirplaneHeightOffset = 30f;
    
    public bool isRunning;
    
    public bool IsGameOver { get; private set; }
    public bool IsGameWon { get; private set; }
    
    private bool bWinSequence = false;

    private float VelocityCap = 5;



    void Awake()
    {
        if(instance == null)
        {
            instance = this;
        }
        else
        {
            Destroy(gameObject);
        }
    }

    void Update()
    {
        if (IsGameOver) return;
        
        HandlePlayerAltitude(CalculateVelocity(ActivePlayerBalloons, VerticalVelocity, true, 0f));
        HandleFallHaptics();
        
        if (!bWinSequence)
        {
            HandleFallHaptics();
        }
    }

    public float CalculateVelocity(int personalBalloons, float currentVel, bool player, float aiDiff)
    {
        float targetVelocity = 0f;

        if (personalBalloons >= hoverThreshold)
        {
            int extraBalloons = personalBalloons - hoverThreshold;
            if (player)
            {
                targetVelocity = Mathf.InverseLerp(0, 10, extraBalloons) * VelocityCap;
            }
            else
            {
                if(aiDiff == 0)
                {
                    targetVelocity = Mathf.InverseLerp(0, 6, extraBalloons) * VelocityCap;
                }
                else
                {
                    targetVelocity = (Mathf.InverseLerp(0, 6, extraBalloons) * VelocityCap) * aiDiff; 
                }
            }
        }
        
        else if (personalBalloons <= freefallThreshold)
        {
            targetVelocity = -maxFreeFallSpeed;
        }
        else if (personalBalloons < hoverThreshold)
        {
            int missingBalloons = hoverThreshold - personalBalloons;
            targetVelocity = -(missingBalloons * (gravityMultiplier / (hoverThreshold - freefallThreshold))); 
        }

        float currentAcceleration = (personalBalloons <= freefallThreshold) ? freefallAcceleration : acceleration;
        
        return Mathf.MoveTowards(currentVel, targetVelocity, currentAcceleration * Time.deltaTime);

    }

    private void HandlePlayerAltitude(float velocity)
    {
        VerticalVelocity = velocity; 
        Altitude += velocity * Time.deltaTime;
        
        if (Altitude <= 0f)
        {
            Altitude = 0f;
            
            // Hastighet över fatal threshold - vi dör 
            if (velocity <= fatalImpactVelocity && !IsGameWon)
            {
                TriggerGameOver();
            }
            else if (velocity < 0f)
            {
                // under fatal threshold - mjuk landning 
                VerticalVelocity = 0f;
            }
        }

        if (Altitude >= WinAltitude && !IsGameWon)
        {
            TriggerGameWon();
        }
    }
    
    private void HandleFallHaptics()
    {
       
        if (VerticalVelocity <= 0f)
        {
        
            float hapticIntensity = Mathf.InverseLerp(0f, maxHapticVelocity, VerticalVelocity);

            if (leftHandHaptics != null)
            {
                leftHandHaptics.SendHapticImpulse(hapticIntensity, Time.deltaTime);
            }
            
            if (rightHandHaptics != null)
            {
                rightHandHaptics.SendHapticImpulse(hapticIntensity, Time.deltaTime);
            }
        }
    }

    private void TriggerGameOver()
    {
        IsGameOver = true;
        VerticalVelocity = 0f;

        PlayImpactEffects();
        
        // Här kan vi sätta in fadeout och haptiskt feedback 
        
     
    }
    
    [ContextMenu("Debug: Trigga Vinstsekvens")]
    private void TriggerGameWon()
    {
        IsGameWon = true;
        StartCoroutine(WinSequenceTimeline());
        
    }

    private IEnumerator WinSequenceTimeline()
    {
        bWinSequence = true;
        bool balloonsPopped = false;
        
        Vector3 flattenedForward = vrCameraTransform.forward;
        flattenedForward.y = 0; 

        if (flattenedForward.sqrMagnitude < 0.01f)
        {
            flattenedForward = vrCameraTransform.up; 
            flattenedForward.y = 0;
            if (flattenedForward.sqrMagnitude < 0.01f) flattenedForward = Vector3.forward;
        }
        flattenedForward.Normalize();

        Vector3 startPos = vrCameraTransform.position + (flattenedForward * winAirplaneSpawnDistance) + (Vector3.up * winAirplaneHeightOffset);
        Vector3 endPos = vrCameraTransform.position - (flattenedForward * winAirplaneSpawnDistance) + (Vector3.up * winAirplaneHeightOffset);
        
        GameObject airplaneInstance = null;
        if (airplanePrefab != null)
        {
            airplaneInstance = Instantiate(airplanePrefab, startPos, Quaternion.LookRotation(endPos - startPos));
        }

        if (winSound != null && audioSource != null)
        {
            audioSource.PlayOneShot(winSound);
        }
        
        float timer = 0f;
        while (timer < winAirplaneFlyDuration)
        {
            timer += Time.deltaTime;
            float progress = timer / winAirplaneFlyDuration;

            if (airplaneInstance != null)
            {
                airplaneInstance.transform.position = Vector3.Lerp(startPos, endPos, progress);

                float distanceToPlayer = Vector3.Distance(airplaneInstance.transform.position, vrCameraTransform.position);
                float intensity = 1.0f - Mathf.Clamp01(distanceToPlayer / winAirplaneSpawnDistance);
                
                if (leftHandHaptics != null) leftHandHaptics.SendHapticImpulse(intensity, Time.deltaTime);
                if (rightHandHaptics != null) rightHandHaptics.SendHapticImpulse(intensity, Time.deltaTime);
            }

            if (progress >= 0.5f && !balloonsPopped)
            {
                if (PlayerBaloonTree != null)
                {
                    PlayerBaloonTree.PopAllBalloons();
                }
                balloonsPopped = true;
            }

            yield return null; 
        }

        float uiWaitDelay = 5.0f;
        float postTimer = 0f;
        
        if (airplaneInstance != null)
        {
            Vector3 flightDirection = (endPos - startPos).normalized;
            float flightSpeed = Vector3.Distance(startPos, endPos) / winAirplaneFlyDuration;

            while (postTimer < uiWaitDelay)
            {
                postTimer += Time.deltaTime;
                if (airplaneInstance != null)
                {
                    airplaneInstance.transform.position += flightDirection * flightSpeed * Time.deltaTime;
                }
                yield return null;
            }
        }
        else
        {
            yield return new WaitForSeconds(uiWaitDelay);
        }

        // Cleanup efter utfasning
        if (airplaneInstance != null)
        {
            Destroy(airplaneInstance);
        }
        bWinSequence = false;

        // Spawna UI
        if (winUIPrefab != null && vrCameraTransform != null)
        {
            Vector3 uiPosition = vrCameraTransform.position + vrCameraTransform.forward * 1.5f;
            Quaternion uiRotation = Quaternion.LookRotation(uiPosition - vrCameraTransform.position);
            Instantiate(winUIPrefab, uiPosition, uiRotation, vrCameraTransform.parent);
        }
    }

    private void PlayImpactEffects()
    {
        if (crashSound != null && audioSource != null)
        {
            audioSource.PlayOneShot(crashSound);
        }
        
        if (crashParticlePrefab != null)
        {
            Instantiate(crashParticlePrefab, Vector3.zero, Quaternion.identity);
        }
    }
    
    public void UpdateGroundAltitude(float yPosition)
    {
        GroundAltitude = yPosition;
    }

    public void SetRunning(bool value)
    {
        isRunning = value;
    }
}
