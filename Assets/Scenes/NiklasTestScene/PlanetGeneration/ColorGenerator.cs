using UnityEngine;

public class ColorGenerator
{
    ColorSettings settings;
    Texture2D texture;
    const int textureResolution = 64;
    INoiseFilter biomeNoiseFilter;

    // Not a constructor as we dont want it to update all the time
    public void UpdateSettings(ColorSettings settings)
    {
        this.settings = settings;

        // update if the number of biomes changes or texture is null
        if(texture == null || texture.height != settings.biomeColorSettings.biomes.Length)
        {
            // store land and ocean thats why *2 
            texture = new Texture2D(textureResolution * 2, settings.biomeColorSettings.biomes.Length, TextureFormat.RGBA32, false);
        }
        biomeNoiseFilter = NoiseFilterFactory.CreateNoiseFilter(this.settings.biomeColorSettings.noiseSettings);
    }

    public void UpdateElevation(MinMax elevationMinMax)
    {
        settings.planetMaterial.SetVector("_elevationMinMax", new Vector4(elevationMinMax.Min, elevationMinMax.Max, 0f, 0f));
    }

    public float BiomePrecentFromPoint(Vector3 pointOnUnitSphere)
    {
        float heightPrecent = (pointOnUnitSphere.y + 1) / 2f; // unit sphere goes from +1 to -1 normalise it to 0-1
        // disturb the height by noise
        heightPrecent += (biomeNoiseFilter.Evaluate(pointOnUnitSphere) - settings.biomeColorSettings.noiseOffset) * settings.biomeColorSettings.noiseStrenght;
        float biomeIndex = 0;
        int numBiomes = settings.biomeColorSettings.biomes.Length;

        float blendRange =settings.biomeColorSettings.blendAmount / 2f + 0.0001f; // small amount added to make the code work better if blendamount = 0

        for (int i = 0; i < numBiomes; i++)
        {
            float distance = heightPrecent - settings.biomeColorSettings.biomes[i].startHeight;
            float weight = Mathf.InverseLerp(-blendRange, blendRange, distance);
            biomeIndex *= (1 - weight); // weight 1 is completly this biome
            biomeIndex += i * weight;
        }
        // return a float representing the biome index as a float normalized to 0-1
        // make sure division cannot be by zero if there is just one biome
        return biomeIndex / (float) Mathf.Max( 1f ,(numBiomes - 1));
    }

    public void UpdateColors()
    {
        Color[] colors = new Color[texture.width * texture.height];
        
        int colorIndex = 0;
        foreach(var biome in settings.biomeColorSettings.biomes) // loop trough all biomes
        {
            for (int i = 0; i < textureResolution * 2; i++)
            {
                Color gradientColor;
                if (i < textureResolution)
                {
                    // Sample from - Ocen gradient
                    gradientColor = settings.oceanColorGradient.Evaluate((float)i / (float)(textureResolution - 1f)); // sample the gradient from the ocean gradient
                }
                else
                {
                    // Sample from - Biome gradient -- (i- textureResolution)
                    gradientColor = biome.gradient.Evaluate((float)(i - textureResolution) / (float)(textureResolution - 1f)); // sample the gradient from the biome gradient
                }
                Color tintcolor = biome.tint;
                colors[colorIndex] = gradientColor * (1 - biome.tintPrecent) + tintcolor * biome.tintPrecent; // set the new color 
                colorIndex++;
            }
        }

        // make the final texture, setting all the colors
        texture.SetPixels(colors);
        texture.Apply();
        settings.planetMaterial.SetTexture("_texture", texture); // set the texture in the planet shader to be the gradient created here
    }
}
