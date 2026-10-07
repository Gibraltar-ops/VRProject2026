using UnityEngine;

/// <summary>
/// Noise filter interface
/// Used by the NoiseFilterFactory for determinating the type of noise to use
/// </summary>
public interface INoiseFilter
{
    public float Evaluate(Vector3 point);

}
