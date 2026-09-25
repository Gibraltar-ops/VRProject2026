using TMPro;
using UnityEngine;

public class DebugAltitude : MonoBehaviour
{

    public TextMeshProUGUI TMP_currBallonCount;
    public TextMeshProUGUI TMP_currAltitude;
    public TextMeshProUGUI TMP_currVelocity;
    


    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        TMP_currAltitude.text = "Altitude: " +  GameManager.instance.Altitude.ToString();
        TMP_currVelocity.text = "Velocity: " +  GameManager.instance.VerticalVelocity.ToString();
        TMP_currBallonCount.text = "Current Ballons: " + GameManager.instance.ActivePlayerBalloons.ToString();
    }
}
