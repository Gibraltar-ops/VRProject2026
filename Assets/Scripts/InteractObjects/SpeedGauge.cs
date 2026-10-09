using UnityEngine;

public class SpeedGauge : MonoBehaviour
{
    [SerializeField] private Transform needle;
    [SerializeField] float needleSpeed;

    private int freefallBalloons;
    private int maxSpeedBalloons;
    private int playerBalloons;
    private int hoverThreshHold;

    private Quaternion maxSpeedRotation;
    private Quaternion neutralSpeed;
    private Quaternion freefallRotation;
    public Quaternion targetRotation;

    void Start()
    {
        freefallBalloons = GameManager.instance.freefallThreshold;
        maxSpeedBalloons = GameManager.instance.balloonsForVelocityCap;
        hoverThreshHold = GameManager.instance.hoverThreshold;

        playerBalloons = GameManager.instance.ActivePlayerBalloons;

        freefallRotation = Quaternion.Euler(-90, -40, 0);
        maxSpeedRotation = Quaternion.Euler(-90, 40, 0);
        neutralSpeed = Quaternion.Euler(-90, 0, 0);
    }

    void Update()
    {
        playerBalloons = GameManager.instance.ActivePlayerBalloons;
        HandleRotation();
    }

    private void HandleRotation()
    {
        float t;
        switch (playerBalloons)
        {
            case <= 3:
                targetRotation = freefallRotation;
                break;

            case <= 10:
                t = Mathf.InverseLerp(freefallBalloons, hoverThreshHold, playerBalloons);
                targetRotation = Quaternion.Lerp(freefallRotation, neutralSpeed, t);
                break;

            case < 20:
                t = Mathf.InverseLerp(hoverThreshHold, maxSpeedBalloons, playerBalloons);
                targetRotation = Quaternion.Lerp(neutralSpeed, maxSpeedRotation, t);
                break;

            case >= 20:
                targetRotation = maxSpeedRotation;
                break;
        }

        needle.localRotation = Quaternion.Slerp(needle.localRotation, targetRotation, needleSpeed*Time.deltaTime);
    }
}
