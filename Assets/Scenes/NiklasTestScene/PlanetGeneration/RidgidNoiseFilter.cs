using UnityEngine;


/// <summary>
/// we can visualize the function using a sine wave
/// Using a absolute value on a sine gives us small hop arcs
/// Inverting that gives us muntain peaks insted
/// Adding a exponential to the sine gives ut sharper peeks
/// </summary>
public class RidgidNoiseFilter : INoiseFilter
{
    NoiseSettings.RigidNoiseSettings settings;
    Noise noise = new Noise();

    public RidgidNoiseFilter(NoiseSettings.RigidNoiseSettings settings)
    {
        this.settings = settings;
    }

    public float Evaluate(Vector3 point)
    {

        //float noiseValue =  (noise.Evaluate(point * settings.roughness + settings.center) + 1) * 0.5f; // make range 0 - 1f
        float noiseValue = 0;
        float frequency = settings.baseRoughness;
        float amplitude = 1f;
        float weight = 1f;

        for (int i = 0; i < settings.numberOfLayers; i++)
        {
            float v = noise.Evaluate(point * frequency + settings.center);
            // making it a rigid shape
            v = 1 - Mathf.Abs(v); // inverted absolue
            v *= v; // ^2 for sharper peeks
            // The weight has the effect of making more detail at the peaks than in the valleys
            v *= weight;
            weight = Mathf.Clamp01(v * settings.weightMultiplier);

            noiseValue += v * amplitude; // we dont need to recalculate to 0-1 range as we already got it
            frequency *= settings.roughness; // when roughness > 1 - the frequency will increase each layer
            amplitude *= settings.persistance; // if percistance < 1 it will decrease each layer
        }

        noiseValue = Mathf.Max(0, noiseValue - settings.minValue); // make a cuttof where its no new noise
        return noiseValue * settings.strenght;
    }
}
