using UnityEngine;

/// <summary>
/// Makes the 
/// </summary>
public static class NoiseFilterFactory
{

    public static INoiseFilter CreateNoiseFilter(NoiseSettings settings)
    {
        switch (settings.filterType) 
        {
            case NoiseSettings.FilterType.Simple:
                return new SimpleNoiseFilter(settings.simpleNoiseSettings);
            case NoiseSettings.FilterType.Rigid:
                return new RidgidNoiseFilter(settings.rigidNoiseSettings);
        }
        return null;
    }
}
