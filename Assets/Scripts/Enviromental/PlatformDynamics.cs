using UnityEngine;

public class PlatformDynamics : MonoBehaviour // Det här behöver ligga på vårt plattformsobjekt!! 
{
    public float GetAltitude()
    {
        return GameManager.instance.Altitude;
    }

    public float GetVerticalVelocity()
    {
        return GameManager.instance.VerticalVelocity;
    }
}