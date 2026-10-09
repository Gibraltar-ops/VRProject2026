using UnityEngine;

public class SpeedGauge : MonoBehaviour
{
    [SerializeField] private Transform needle;
    [SerializeField] private float needleSpeed = .1f;

    private int freefallBalloons;
    private int maxSpeedBalloons;
    private int playerBalloons;
    private int hoverThreshHold;

    private Quaternion maxSpeedRotation;
    private Quaternion neutralSpeedRotation;
    private Quaternion freefallRotation;

    private Quaternion targetRotation;

    void Start()
    {
        freefallBalloons = GameManager.instance.freefallThreshold;
        maxSpeedBalloons = GameManager.instance.balloonsForVelocityCap;
        hoverThreshHold = GameManager.instance.hoverThreshold;

        freefallRotation = Quaternion.Euler(-90, -40, 0);
        maxSpeedRotation = Quaternion.Euler(-90, 40, 0);
        neutralSpeedRotation = Quaternion.Euler(-90, 0, 0);
    }

    void Update()
    {
        playerBalloons = GameManager.instance.ActivePlayerBalloons;

        HandleRotation();
    }

    private void HandleRotation()
    {
        if (playerBalloons <= freefallBalloons)
        {
            targetRotation = freefallRotation;
        }
        else if (playerBalloons <= hoverThreshHold)
        {
            float t = Mathf.InverseLerp(
                freefallBalloons,
                hoverThreshHold,
                playerBalloons
            );

            targetRotation = Quaternion.Lerp(
                freefallRotation,
                neutralSpeedRotation,
                t
            );
        }
        else if (playerBalloons <= maxSpeedBalloons)
        {
            float t = Mathf.InverseLerp(
                hoverThreshHold,
                maxSpeedBalloons,
                playerBalloons
            );

            targetRotation = Quaternion.Lerp(
                neutralSpeedRotation,
                maxSpeedRotation,
                t
            );
        }
        else
        {
            targetRotation = maxSpeedRotation;
        }

        needle.localRotation = Quaternion.Slerp(
            needle.localRotation,
            targetRotation,
            needleSpeed * Time.deltaTime
        );
    }
}
