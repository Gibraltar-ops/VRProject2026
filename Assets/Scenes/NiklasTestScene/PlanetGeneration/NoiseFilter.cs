using UnityEngine;

public class NoiseFilter : MonoBehaviour
{
    NoiseSettings settings;
    Noise noise = new Noise();

    public NoiseFilter(NoiseSettings settings)
    {  
        this.settings = settings; 
    }

    public float Evaluate(Vector3 point)
    {
        
        //float noiseValue =  (noise.Evaluate(point * settings.roughness + settings.center) + 1) * 0.5f; // make range 0 - 1f
        float noiseValue = 0;
        float frequency = settings.baseRoughness;
        float amplitude = 1f;

        for (int i = 0;  i < settings.numberOfLayers; i++)
        {
            float v = noise.Evaluate(point * frequency + settings.center);
            noiseValue += (v + 1) * 0.5f * amplitude;
            frequency *= settings.roughness; // when roughness > 1 - the frequency will increase each layer
            amplitude *= settings.persistance; // if percistance < 1 it will decrease each layer
        }

        noiseValue = Mathf.Max(0, noiseValue - settings.minValue); // make a cuttof where its no new noise
        return noiseValue * settings.strenght;
    }
}
