using System.Collections.Generic;
using UnityEngine;

public class BalloonStringRenderer : MonoBehaviour
{
    private List<LineRenderer> balloonStrings = new();
    public List<Transform> AttachPoint;
    [SerializeField] private BalloonTree balloonTree;
    [SerializeField] private Material _lineMaterial;
    [SerializeField] private Gradient _colorGradient;

    void Awake()
    {
        balloonTree.OnInitialized += Startup;
    }

    private void Startup()
    {
        balloonTree.OnInitialized -= Startup;
        InitializeStrings(balloonTree._balloons);
    }

    public void InitializeStrings(List<BalloonInfo> balloons)
    {
        List<BalloonInfo> tiedBalloons = new();
        for (int i = 0; i < balloons.Count; i++)
        {
            int attachIndex = i % AttachPoint.Count;
            BalloonInfo closestBalloon = new();

            closestBalloon = balloonTree.GetClosestBallon(AttachPoint[attachIndex].position, tiedBalloons);
            tiedBalloons.Add(closestBalloon);
            Transform balloonAttachPoint = closestBalloon.transform.Find("AttachPoint").transform;
            LineRenderer newString = CreateString(balloonAttachPoint, AttachPoint[attachIndex]);
            balloonStrings.Add(newString);


        }

    }

    LineRenderer CreateString(Transform balloonTransform, Transform attachPosition)
    {
        LineRenderer balloonString = balloonTransform.gameObject.AddComponent<LineRenderer>();
        balloonString.positionCount = 2;
        balloonString.SetPosition(0, balloonTransform.position);
        balloonString.SetPosition(1, attachPosition.position);
        balloonString.material = _lineMaterial;
        balloonString.startWidth = 0.02f;
        balloonString.endWidth = 0.02f;

        balloonString.startColor = Color.rosyBrown;
        balloonString.endColor = Color.brown;
        return balloonString;
    }
    // Update is called once per frame
    void Update()
    {

    }
}
