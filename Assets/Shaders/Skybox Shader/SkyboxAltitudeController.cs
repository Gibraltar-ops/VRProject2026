using UnityEngine;

// SÄTT DEN HÄR PÅ VÅR GAME MANAGER

public class SkyboxAltitudeController : MonoBehaviour
{
    [Header("Dependencies")]
    [SerializeField] private Material skyboxMaterial;

    [Header("Parameters")]
    [SerializeField] private float maxHeight = 10000f; 
    
    [Range(0f, 0.5f)]
    [SerializeField] private float maxDip = 0.15f; 

    private void Update()
    {
       
        if (skyboxMaterial == null || GameManager.instance == null) return;

      
        float currentHeight = GameManager.instance.Altitude;
        
    
        float heightRatio = Mathf.Clamp01(currentHeight / maxHeight);

    
        float currentDip = heightRatio * maxDip;
        skyboxMaterial.SetFloat("_HorizonDip", currentDip);
    }

    private void OnDisable()
    {
        if (skyboxMaterial != null)
        {
            skyboxMaterial.SetFloat("_HorizonDip", 0f);
        }
    }
}