using UnityEngine;

[System.Serializable]
public class NoiseSettings
{
    public enum FilterType
    {
        Simple,
        Rigid
    };

    public FilterType filterType; // noise filter type - used in noise factory

    [ConditionalHide("filterType",0)] // show and hide in editor inspector based on filterType enum above! Custom property drawer and another script
    public SimpleNoiseSettings simpleNoiseSettings;
    [ConditionalHide("filterType", 1)]
    public RigidNoiseSettings rigidNoiseSettings;


    [System.Serializable]
    public class SimpleNoiseSettings
    {
        public float strenght = 1f;
        [Range(1,8)]
        public int numberOfLayers = 1;
        public float baseRoughness = 1f;
        public float roughness = 2f;
        public float persistance = 0.5f;
        public Vector3 center;
        public float minValue;
    }

    [System.Serializable]
    public class RigidNoiseSettings : SimpleNoiseSettings
    {
        // Inherits all values from simple noise settings
        public float weightMultiplier = 0.8f;
    }

}
