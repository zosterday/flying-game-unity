using UnityEngine;

public class MeshData
{
    public Vector3[] Vertices { get; set; }

    public Vector2[] Uvs { get; set; }

    private int[] triangles;

    private int triangleIndex;

    public MeshData(int meshWidth, int meshHeight)
    {
        Vertices = new Vector3[meshWidth * meshHeight];
        Uvs = new Vector2[meshWidth * meshHeight];
        triangles = new int[(meshWidth - 1) * (meshHeight - 1) * 6];
    }

    public void AddTriangle(int a, int b, int c)
    {
        triangles[triangleIndex] = a;
        triangles[triangleIndex + 1] = b;
        triangles[triangleIndex + 2] = c;
        triangleIndex += 3;
    }

    public Mesh CreateMesh()
    {
        var mesh = new Mesh();
        mesh.vertices = Vertices;
        mesh.triangles = triangles;
        mesh.uv = Uvs;
        mesh.RecalculateNormals();

        return mesh;
    }
}