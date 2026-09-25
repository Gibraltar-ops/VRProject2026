using UnityEngine;

public class GameManager : MonoBehaviour
{
    public static GameManager instance = null;

    [Header("Systems")]

    [Header("Altitude")]
    public float Altitude{get; private set;}
    public float CloudBaseAltitude{get;} = 1200f;

    public float WinAltitude {get;} = 15000f;

    private float liftMultiplier = 2.0f;
    private float gravityMultiplier = 5.0f;


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
        HandleAltitudeChange();
    }

    private void HandleAltitudeChange()
    {
        
    }

}
