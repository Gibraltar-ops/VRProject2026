using TMPro;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    public static GameManager instance = null;

    [Header("Systems")]

    public BalloonTree PlayerBaloonTree;

    public int ActivePlayerBalloons{
        get{return PlayerBaloonTree.ActiveBalloons;}
    }

    [Header("Altitude")]
    public float Altitude{get; private set;}
    public float VerticalVelocity { get; private set; }
    
    public float CloudBaseAltitude{get;} = 900f;
    public float WinAltitude {get; private set; } = 3000f;
    
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
    [SerializeField] private float maxFreeFallSpeed = 30.0f;
    [Tooltip("Accelerering för fritt fall - 9.8 = jordens tyngdkraft")]
    [SerializeField] private float freefallAcceleration = 9.8f;

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
        HandlePlayerAltitude(CalculateVelocity(ActivePlayerBalloons, VerticalVelocity, true));
    }

    public float CalculateVelocity(int personalBalloons, float currentVel, bool player)
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
                targetVelocity = Mathf.InverseLerp(0, 6, extraBalloons) * VelocityCap; 
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
        
        if (Altitude < 0f)
        {
            Altitude = 0f;
            if (velocity < 0f)
            {
                velocity = 0f;
            }
        }
        
    }
}
