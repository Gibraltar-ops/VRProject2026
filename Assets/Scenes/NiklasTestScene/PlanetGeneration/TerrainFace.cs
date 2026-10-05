using UnityEngine;


/// <summary>
/// This class is called from planet to generate one of the cube planes
/// Used for the sphere
/// </summary>
public class TerrainFace
{
    ShapeGenerator shapeGenerator;
    Mesh mesh;
    int resolution;
    Vector3 localUp;
    Vector3 axisA;
    Vector3 axisB;

    public TerrainFace(ShapeGenerator shapeGenerator, Mesh mesh, int resolution, Vector3 localUp)
    {
        this.shapeGenerator = shapeGenerator;
        this.mesh = mesh;
        this.resolution = resolution;
        this.localUp = localUp;

        // Createing the coordinate sysstem for the plane
        axisA = new Vector3(localUp.y, localUp.z, localUp.x); // switching the vector around to create a vector pointing out from the side of the up direction
        axisB = Vector3.Cross(localUp, axisA); // perpendicular direction from the other axisis
    }

    public void ConstructMesh()
    {
        Vector3[] vertices = new Vector3[resolution * resolution]; // will be the resolution squared
        // triangles stores the vertices index points that make up the triangel face
        int[] triangles = new int[(resolution - 1) * (resolution - 1) * 2 * 3]; // calculate the total nr of triangles needed

        int triIndex = 0;

        for (int y =0 ; y < resolution; y++)
        {
            for (int x = 0 ; x < resolution; x++)
            {
                int i = x + y * resolution; // calculate curent verex index
                Vector2 precent = new Vector2(x,y) / (resolution - 1); // calculate how long along the lenght of the cube the points sould be
                // make a unit cube from the points (size 2x2x2 meters in this case) 
                Vector3 pointOnUnitCube = 
                    localUp + 
                    (precent.x - 0.5f) * 2 * axisA + 
                    (precent.y - 0.5f) * 2 * axisB;
                Vector3 pointOnUnitSphere = pointOnUnitCube.normalized;

                //vertices[i] = pointOnUnitCube; // make cube 
                //vertices[i] = pointOnUnitSphere; // make cube sphere
                vertices[i] = shapeGenerator.CalculatePointOnPlanet(pointOnUnitSphere);


                // we cannot create triangles at the right and bottom edge of our mesh
                if (x != resolution - 1 && y != resolution -1) 
                {
                    triangles[triIndex] = i; // starting triangle index - upper left
                    triangles[triIndex + 1] = i + resolution + 1; // next vertex right down - bottom right
                    triangles[triIndex + 2] = i + resolution; // next vertex down - left down

                    triangles[triIndex + 3] = i; // getting the first index - upper left
                    triangles[triIndex + 4] = i + 1; // next vert to the right - right
                    triangles[triIndex + 5] = i + resolution + 1; // next vert down and right - bottom right

                    triIndex += 6; // we added 6 vertices
                }
                
            }
        }
        mesh.Clear(); // reset the mesh
        mesh.vertices = vertices;
        mesh.triangles = triangles;
        mesh.RecalculateNormals();
        mesh.RecalculateBounds();
    }

}
