using UnityEngine;

public class CloudPlane : MonoBehaviour
{
    [SerializeField] private PlatformDynamics platform;

    private Renderer[] allRenderers;

    [SerializeField]private float maxRenderDistance = 900f;

    private float realCloudAltitude;
    private Vector3 initialScale;

    private void Start()
    {
        initialScale = transform.localScale;
        allRenderers = GetComponentsInChildren<Renderer>();

        realCloudAltitude = transform.position.y;
    }

    private void Update()
    {
        if (platform == null) return;

        float currentAltitude = platform.GetAltitude();

        float targetVirtualDistance = realCloudAltitude - currentAltitude;

        float absoluteDistance = Mathf.Abs(targetVirtualDistance);

        if (absoluteDistance > maxRenderDistance)
        {
            // FORCED PERSPECTIVE
            // mathf.sign returnerar 1 om positivt och -1 om negativt, låser värdet på maxrenderdistance
            float directionY = Mathf.Sign(targetVirtualDistance);
            transform.position = new Vector3(transform.position.x, maxRenderDistance * directionY, transform.position.z);

            // skalning baserad på absoluta avståndet
            float scaleRatio = maxRenderDistance / absoluteDistance;
            transform.localScale = initialScale * scaleRatio;
        }
        else
        {
            // Innanför vår maxrenderdistance = molnet rör på sig och visas i skala 1:1 
            transform.position = new Vector3(transform.position.x, targetVirtualDistance, transform.position.z);
            transform.localScale = initialScale;
        }
    }
}
