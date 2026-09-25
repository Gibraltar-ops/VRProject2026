using UnityEngine;

public class PlatformDynamics : MonoBehaviour
{
    // Variabler för ballong, upp-/nerhastighet och tröskelvärden
    
    // Runtime data
    [Header("Felsökning i editorn")]
    [SerializeField] private int activeBalloons = 1;
    [SerializeField] private float currentAltitude = 0f;
    [SerializeField] private float currentVerticalVelocity = 0f;
    
    [Header("Balans och tröskelvärden")]
    [SerializeField] private float winAltitude = 15000f;
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
    
    private void FixedUpdate()
    {
        CalculateVelocity();
    }

    private void Update()
    {
        UpdateAltitude();
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

    private void CalculateVelocity()
    {
        float targetVelocity = 0f; // 0 = ballongen hovrar
        
        // Fler ballonger än threshold = Ballongen rör sig uppåt
        if (activeBalloons > hoverThreshold)
        {
            int extraBalloons = activeBalloons - hoverThreshold;

            targetVelocity = extraBalloons * liftMultiplier;
        }
        
        // Ballonger lika med eller färre än freefall och vi faller maximal hastighet
        else if (activeBalloons <= freefallThreshold)
        {
            targetVelocity = -maxFreeFallSpeed;
        }
        
        // Ballong under threshold men över freefall = långsam descent
        else if (activeBalloons < hoverThreshold)
        {
            int missingBalloons = hoverThreshold - activeBalloons;

            targetVelocity = -(missingBalloons * (gravityMultiplier / (hoverThreshold - freefallThreshold)));
        }

        float currentAcceleration = (activeBalloons <= freefallThreshold) ? freefallAcceleration : acceleration;

        // Mjuk acceleration med MoveTowards mot specifik velocity 
        currentVerticalVelocity = 
            Mathf.MoveTowards(currentVerticalVelocity, targetVelocity, currentAcceleration * Time.fixedDeltaTime); 

    }

    private void UpdateAltitude()
    {
        // Hastighet adderas över tid
        currentAltitude += currentVerticalVelocity * Time.fixedDeltaTime;

        // Stoppar oss från att åka under markplanet
        if (currentAltitude < 0f)
        {
            currentAltitude = 0f;

            // Nollställer hastigheten vid impact 
            if (currentVerticalVelocity < 0f)
            {
                currentVerticalVelocity = 0f;
            }
        }
    }

    // Getter för att leverera currentAltitude till markplanet
    public float GetAltitude()
    {
        return currentAltitude;
    }

    
    public float GetVerticalVelocity()
    {
        return currentVerticalVelocity;
    }
    
}