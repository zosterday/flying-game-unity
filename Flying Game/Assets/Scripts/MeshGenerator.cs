using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;


public static class MeshGenerator
{
    public static MeshData GenerateTerrainMesh(float[,] heightMap)
    {
        var width = heightMap.GetLength(0);
        var height = heightMap.GetLength(1);
        var topLeftX = (width - 1) * (-0.5f);
        var topLeftZ = (height - 1) * (0.5f);

        var meshData = new MeshData(width, height);
        var vertexIndex = 0;

        for (var y = 0; y < height; y++)
        {
            for (var x = 0; x < width; x++)
            {
                meshData.Vertices[vertexIndex] = new Vector3(topLeftX + x, heightMap[x, y], topLeftZ - y);
                meshData.Uvs[vertexIndex] = new Vector2(x / (float)width, y / (float)height);

                if (x < width - 1 && y < height - 1)
                {
                    meshData.AddTriangle(vertexIndex, vertexIndex + width + 1, vertexIndex + width);
                    meshData.AddTriangle(vertexIndex + width + 1, vertexIndex, vertexIndex + 1);
                }

                vertexIndex++;
            }
        }

        return meshData;
    }
}

public class MeshData
{
    public Vector3[] Vertices { get; set; }

    public int[] Triangles { get; set; }

    public Vector2[] Uvs { get; set; }

    private int triangleIndex;

    public MeshData(int meshWidth, int meshHeight)
    {
        Vertices = new Vector3[meshWidth * meshHeight];
        Uvs = new Vector2[meshWidth * meshHeight];
        Triangles = new int[(meshWidth - 1) * (meshHeight - 1) * 6];
    }

    public void AddTriangle(int a, int b, int c)
    {
        Triangles[triangleIndex] = a;
        Triangles[triangleIndex + 1] = b;
        Triangles[triangleIndex + 2] = c;
        triangleIndex += 3;
    }

    public Mesh CreateMesh()
    {
        var mesh = new Mesh();
        mesh.vertices = Vertices;
        mesh.triangles = Triangles;
        mesh.uv = Uvs;
        mesh.RecalculateNormals();

        return mesh;
    }
}



























//[RequireComponent(typeof(MeshFilter))]
//public class MeshGenerator : MonoBehaviour
//{
    //private const float NoiseMultiplier = 0.3f;

    //[SerializeField]
    //private int xSize;

    //[SerializeField]
    //private int zSize;

    //[SerializeField]
    //private GameObject player;

    //private Mesh mesh;

    //private int[] triangles;
    //private Vector3[] vertices;

    //// Start is called before the first frame update
    //void Start()
    //{
    //    mesh = new();

    //    GetComponent<MeshFilter>().mesh = mesh;

    //    GenerateMesh();
    //    UpdateMesh();

    //}


    //private void GenerateMesh()
    //{
    //    vertices = new Vector3[(xSize + 1) * (zSize + 1)];

    //    for (int i = 0, z = 0; z <= zSize; z++)
    //    {
    //        for (int x = 0; x <= xSize; x++)
    //        {
    //            var y = Mathf.PerlinNoise(x * NoiseMultiplier, z * NoiseMultiplier) * 2f;
    //            vertices[i] = new Vector3(x, y, z);
    //            i++;
    //        }
    //    }

    //    triangles = new int[xSize * zSize * 6];

    //    int vert = 0;
    //    int tris = 0;
    //    for (int z = 0; z < zSize; z++)
    //    {
    //        for (int x = 0; x < xSize; x++)
    //        {
    //            triangles[tris + 0] = vert + 0;
    //            triangles[tris + 1] = vert + xSize + 1;
    //            triangles[tris + 2] = vert + 1;
    //            triangles[tris + 3] = vert + 1;
    //            triangles[tris + 4] = vert + xSize + 1;
    //            triangles[tris + 5] = vert + xSize + 2;

    //            vert++;
    //            tris += 6;
    //        }
    //        vert++;
    //    }

    //}

    //private void UpdateMesh()
    //{
    //    mesh.Clear();

    //    mesh.vertices = vertices;
    //    mesh.triangles = triangles;

    //    mesh.RecalculateNormals();
    //}




    //private const float NoiseMultiplier = 0.3f;

    //private const float BlockSize = 1f;

    //[SerializeField]
    //private int xSize;

    //[SerializeField]
    //private int zSize;

    //[SerializeField]
    //private GameObject player;

    //private Mesh mesh;

    //private int[] triangles;
    //private Vector3[] vertices;

    //private Vector3 startPosition;

    //private int xPlayerLocation => (int)Mathf.Floor(player.transform.position.x);
    //private int zPlayerLocation => (int)Mathf.Floor(player.transform.position.z);

    //private int xPlayerMove => (int)(player.transform.position.x - startPosition.x);
    //private int zPlayerMove => (int)(player.transform.position.z - startPosition.z);


    //// Start is called before the first frame update
    //void Start()
    //{
    //    mesh = new();

    //    GetComponent<MeshFilter>().mesh = mesh;

    //    GenerateMesh();
    //    UpdateMesh();

    //}

    //private void Update()
    //{
    //    if (Mathf.Abs(xPlayerMove) >= BlockSize || Mathf.Abs(zPlayerMove) >= BlockSize)
    //    {

    //    }

    //}









    //private void GenerateMesh()
    //{
    //    vertices = new Vector3[(xSize + 1) * (zSize + 1)];

    //    for (int i = 0, z = 0; z <= zSize; z++)
    //    {
    //        for (int x = 0; x <= xSize; x++)
    //        {
    //            var y = Mathf.PerlinNoise(x * NoiseMultiplier, z * NoiseMultiplier) * 2f;
    //            vertices[i] = new Vector3(x, y, z);
    //            i++;
    //        }
    //    }

    //    triangles = new int[xSize * zSize * 6];

    //    int vert = 0;
    //    int tris = 0;
    //    for (int z = 0; z < zSize; z++)
    //    {
    //        for (int x = 0; x < xSize; x++)
    //        {
    //            triangles[tris + 0] = vert + 0;
    //            triangles[tris + 1] = vert + xSize + 1;
    //            triangles[tris + 2] = vert + 1;
    //            triangles[tris + 3] = vert + 1;
    //            triangles[tris + 4] = vert + xSize + 1;
    //            triangles[tris + 5] = vert + xSize + 2;

    //            vert++;
    //            tris += 6;
    //        }
    //        vert++;
    //    }

    //}

    //private void UpdateMesh()
    //{
    //    mesh.Clear();

    //    mesh.vertices = vertices;
    //    mesh.triangles = triangles;

    //    mesh.RecalculateNormals();
    //}
//}
