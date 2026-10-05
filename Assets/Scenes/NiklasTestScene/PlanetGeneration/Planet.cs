using UnityEngine;
using UnityEngine.Rendering;

/// <summary>
/// https://www.youtube.com/watch?v=uY9PAcNMu8s
/// </summary>
public class Planet : MonoBehaviour
{
    [Range(2,256), Header("mesh resolution")]
    public int resolution = 10;
    public Material material;
    public bool autoUpdate = true;

    public ShapeSettings shapeSettings;
    public ColorSettings colorSettings;

    [HideInInspector]
    public bool shapeSettingsFoldout; // must store a value for if the custom editor is active or not
    [HideInInspector]
    public bool colorSettingsFoldout;

    ShapeGenerator shapeGenerator; // does the vertex manipulation functions

    [SerializeField, HideInInspector]
    MeshFilter[] meshFilters;
    TerrainFace[] terrainFaces;

    // for making it work in the editor - The editor script is handling this now PlanetEditor
    //private void OnValidate()
    //{
    //    GeneratePlanet();
    //}

    void Initialize()
    {
        shapeGenerator = new ShapeGenerator(shapeSettings);

        if(meshFilters == null || meshFilters.Length == 0)
        {
            meshFilters = new MeshFilter[6];
        }
        terrainFaces = new TerrainFace[6];

        Vector3[] directions =
        {
            Vector3.up,
            Vector3.down,
            Vector3.left,
            Vector3.right,
            Vector3.forward,
            Vector3.back
        };

        for (int i = 0; i < 6; i++) 
        {
            if (meshFilters[i] == null)
            {
                GameObject meshObj = new GameObject("mesh");
                meshObj.transform.parent = transform;
                meshObj.transform.localPosition = Vector3.zero;
                //meshObj.AddComponent<MeshRenderer>().sharedMaterial = new Material(Shader.Find("Standard"));
                meshObj.AddComponent<MeshRenderer>().sharedMaterial = material;

                meshFilters[i] = meshObj.AddComponent<MeshFilter>();
                meshFilters[i].sharedMesh = new Mesh();
            }


            terrainFaces[i] = new TerrainFace(shapeGenerator , meshFilters[i].sharedMesh,resolution, directions[i]);

        }
    }

    public void GeneratePlanet()
    {
        Initialize();
        GenerateMesh();
        GenerateColors();
    }

    public void OnShapeSettingsUpdated()
    {
        if (autoUpdate)
        {
            Initialize();
            GenerateMesh();
        }
    }

    public void OnColorSettingsUpdated()
    {
        if (autoUpdate)
        {
            Initialize();
            GenerateColors();
        }
    }


    void GenerateMesh()
    {
        foreach (TerrainFace face in terrainFaces)
        {
            face.ConstructMesh();
        }
    }


    void GenerateColors()
    {
        foreach(MeshFilter m in meshFilters)
        {
            m.GetComponent<MeshRenderer>().sharedMaterial.color = colorSettings.planetColor;
        }
    }

}
