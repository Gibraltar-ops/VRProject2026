using UnityEngine;

public class Altitudemeter : MonoBehaviour
{
    [SerializeField] private Renderer waterRenderer;
    private Material waterMat;

    private float progress;




    void Start()
    {
        waterMat = waterRenderer.material;

        
    }

    void Update()
    {
        progress = GameManager.instance.Altitude / GameManager.instance.WinAltitude;

        SetFill(Mathf.Clamp01(progress));

    }

    private void SetFill(float value)
    {
        waterMat.SetFloat("_FillAmount", value);
    }

}
