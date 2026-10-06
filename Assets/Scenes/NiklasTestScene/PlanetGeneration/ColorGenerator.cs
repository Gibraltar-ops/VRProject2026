using UnityEngine;

public class ColorGenerator
{
    ColorSettings settings;
    Texture2D texture;
    const int textureResolution = 64;

    // Not a constructor as we dont want it to update all the time
    public void UpdateSettings(ColorSettings settings)
    {
        this.settings = settings;
        if(texture == null)
        {
            texture = new Texture2D(textureResolution, 1);
        }
    }

    public void UpdateElevation(MinMax elevationMinMax)
    {
        settings.planetMaterial.SetVector("_elevationMinMax", new Vector4(elevationMinMax.Min, elevationMinMax.Max, 0f, 0f));
    }

    public void UpdateColors()
    {
        Color[] colors = new Color[textureResolution];
        
        for (int i = 0; i < textureResolution; i++)
        {
            colors[i] = settings.gradient.Evaluate( (float)i / (float)(textureResolution -1f) ); // sample the gradient from the color settings
        }
        texture.SetPixels(colors);
        texture.Apply();
        settings.planetMaterial.SetTexture("_texture", texture); // set the texture in the planet shader to be the gradient created here
    }
}
