using NUnit.Framework;
using TMPro;
using UnityEngine;
using UnityEngine.InputSystem.XR.Haptics;

public class GameManager : MonoBehaviour
{
    public static GameManager instance = null;

    [Header("Systems")]

    public BalloonTree PlayerBaloonTree;

    public int ActivePlayerBalloons
    {
        get{return PlayerBaloonTree.ActiveBalloons;}
    }

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
    public bool isRunning;
    
    public bool IsGameOver { get; private set; }

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
            if (velocity <= fatalImpactVelocity)
            {
                TriggerGameOver();
            }
            else if (velocity < 0f)
            {
                // under fatal threshold - mjuk landning 
                VerticalVelocity = 0f;
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

    private void PlayImpactEffects()
    {
        if (crashSound != null && audioSource != null)
        {
            audioSource.PlayOneShot(crashSound);
        }
        
        // Instansiera partiklar om prefab finns
        if (crashParticlePrefab != null)
        {
            // Vi instansierar partiklarna i världens nollpunkt.
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
