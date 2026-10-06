using JetBrains.Annotations;
using UnityEditor.ShaderGraph.Internal;
using UnityEngine;

/// <summary>
/// This script does the terrain vertex manipulations
/// </summary>
public class ShapeGenerator
{
    ShapeSettings settings;
    INoiseFilter[] noiseFilters;
    public MinMax elevationMinMax;

    public void UpdateSettings(ShapeSettings settings)
    {
        this.settings = settings;
        this.noiseFilters = new INoiseFilter[settings.noiseLayers.Length]; // setting up the arry of inoise filters

        for (int i = 0; i < noiseFilters.Length; i++)
        {
            // Use the noise filter factory to get the right kind of noise filter for the current layer
            this.noiseFilters[i] = NoiseFilterFactory.CreateNoiseFilter(settings.noiseLayers[i].noiseSettings); 
        }
        elevationMinMax = new MinMax();
    }

    public Vector3 CalculatePointOnPlanet(Vector3 pointOnUnitSphere)
    {
        //float elevation = noiseFilter.Evaluate(pointOnUnitSphere);
        float firstLayerValue = 0f; // used as a mask for the layers that have it enabled
        float elevation = 0;

        if (noiseFilters.Length > 0) 
        {
            firstLayerValue = noiseFilters[0].Evaluate(pointOnUnitSphere);
            if (settings.noiseLayers[0].enabled) // if first layer is enabled store the elevation 
            {
                elevation += firstLayerValue;
            }
        }

        // loop all layers over layer one - that can be used as a mask
        for (int i = 1; i < noiseFilters.Length; i++)
        {
            if (settings.noiseLayers[i].enabled) // only if the noise is enabled
            {
                float mask = (settings.noiseLayers[i].useFirstLayerAsMask ? firstLayerValue : 1); // use value or no mask 1
                // possible to add a mask threshold or somthing here
                elevation += noiseFilters[i].Evaluate(pointOnUnitSphere) * mask; // multiply the mask value 
            }
        }

        elevation = settings.planetRadius * (1 + elevation); // Calculate final elevation
        elevationMinMax.AddValue(elevation); // for keeping track of the max and min height

        return pointOnUnitSphere * elevation;
    }
}
