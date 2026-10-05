using UnityEngine;

[CreateAssetMenu(fileName ="shape_settings",menuName ="planet/createShapeSettings")]
public class ShapeSettings : ScriptableObject
{
    public float planetRadius = 1f;
    public NoiseLayer[] noiseLayers;

    [System.Serializable]
    public class NoiseLayer
    {
        public bool enabled = true;
        public NoiseSettings noiseSettings;
    }
}
