using UnityEngine;

[CreateAssetMenu(fileName = "color_settings", menuName = "planet/createColorSettings")]
public class ColorSettings : ScriptableObject
{
    public Gradient gradient;
    public Material planetMaterial;

    public class BiomeColorSettings
    {

    }
}
