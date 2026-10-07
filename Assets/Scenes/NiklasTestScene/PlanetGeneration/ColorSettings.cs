using UnityEngine;

[CreateAssetMenu(fileName = "color_settings", menuName = "planet/createColorSettings")]
public class ColorSettings : ScriptableObject
{
    public Material planetMaterial;
    public BiomeColorSettings biomeColorSettings;
    public Gradient oceanColorGradient;

    [System.Serializable]
    public class BiomeColorSettings
    {
        public Biome[] biomes;
        public NoiseSettings noiseSettings;
        public float noiseOffset;
        public float noiseStrenght;
        [Range(0f,1f)]
        public float blendAmount;

        [System.Serializable]
        public class Biome
        {
            public Gradient gradient;
            public Color tint;
            [Range(0f,1f)]
            public float startHeight;
            [Range(0f, 1f)]
            public float tintPrecent;
        }

    }
}
