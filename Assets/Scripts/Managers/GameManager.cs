using UnityEngine;

public class GameManager : MonoBehaviour
{
    public static GameManager instance = null;

    [Header("Systems")]

    [Header("Altitude")]
    public float Altitude{get; private set;}
    public float VerticalVelocity { get; private set; }
    
    public float CloudBaseAltitude{get;} = 1200f;
    public float WinAltitude {get; private set; } = 15000f;

    [Header("Runtime debug: antal ballonger")] 
    [SerializeField] private int activeBalloons = 1;
    
    [Header("Balans/Thresholds")]
    [SerializeField] private float liftMultiplier = 2.0f;
    [SerializeField] private float gravityMultiplier = 5.0f;
    [Tooltip("Hur många ballonger som behövs för att hovra")]
    [SerializeField] private int hoverThreshold = 10;
    [Tooltip("Hur få ballonger som behövs för att hamna i fritt fall")]
    [SerializeField] private int freefallThreshold = 3;
    [Tooltip("Hur mycket acceleration/lift multiplier påverkas per uppblåst ballong")]
    [SerializeField] private float acceleration = 2.0f;
    
    [Header("Fritt fall - justeringar")]
    [SerializeField] private float maxFreeFallSpeed = 30.0f;
    [Tooltip("Accelerering för fritt fall - 9.8 = jordens tyngdkraft")]
    [SerializeField] private float freefallAcceleration = 9.8f;


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

    void FixedUpdate()
    {
        HandleAltitudeChange();
    }

    private void HandleAltitudeChange()
    {
        float targetVelocity = 0f;

        if (activeBalloons > hoverThreshold)
        {
            int extraBalloons = activeBalloons - hoverThreshold;
            targetVelocity = extraBalloons * liftMultiplier;
        }
        
        else if (activeBalloons <= freefallThreshold)
        {
            targetVelocity = -maxFreeFallSpeed;
        }
        else if (activeBalloons < hoverThreshold)
        {
            int missingBalloons = hoverThreshold - activeBalloons;
            targetVelocity = -(missingBalloons * (gravityMultiplier / (hoverThreshold - freefallThreshold))); 
        }

        float currentAcceleration = (activeBalloons <= freefallThreshold) ? freefallAcceleration : acceleration;
        
        VerticalVelocity = Mathf.MoveTowards(VerticalVelocity, targetVelocity, currentAcceleration * Time.fixedDeltaTime);

        Altitude += VerticalVelocity * Time.fixedDeltaTime;
        
        if (Altitude < 0f)
        {
            Altitude = 0f;
            if (VerticalVelocity < 0f)
            {
                VerticalVelocity = 0f;
            }
        }
    }

    public void AddBalloon()
    {
        activeBalloons++;
    }
    
    public void RemoveBalloon(int amount)
    {
        activeBalloons -= amount;

        if (activeBalloons < 0)
        {
            activeBalloons = 0;
        }
    }

}
