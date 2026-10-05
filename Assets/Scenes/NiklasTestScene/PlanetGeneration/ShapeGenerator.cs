using UnityEngine;

/// <summary>
/// This script does the terrain vertex manipulations
/// </summary>
public class ShapeGenerator
{
    ShapeSettings settings;
    NoiseFilter[] noiseFilters;

    public ShapeGenerator(ShapeSettings settings)
    {
        this.settings = settings;
        this.noiseFilters = new NoiseFilter[settings.noiseLayers.Length];

        for (int i = 0; i < noiseFilters.Length; i++)
        {
            this.noiseFilters[i] = new NoiseFilter(settings.noiseLayers[i].noiseSettings);
        }
    }

    public Vector3 CalculatePointOnPlanet(Vector3 pointOnUnitSphere)
    {
        //float elevation = noiseFilter.Evaluate(pointOnUnitSphere);
        float elevation = 0;
        for (int i = 0; i < noiseFilters.Length; i++)
        {
            if (settings.noiseLayers[i].enabled)
            {
                elevation += noiseFilters[i].Evaluate(pointOnUnitSphere);
            }
        }


        return pointOnUnitSphere * settings.planetRadius * (1 + elevation);
    }
}
