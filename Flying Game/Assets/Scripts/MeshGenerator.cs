using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

[RequireComponent(typeof(MeshFilter))]
public class MeshGenerator : MonoBehaviour
{
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




    private const float NoiseMultiplier = 0.3f;

    private const float BlockSize = 1f;

    [SerializeField]
    private int xSize;

    [SerializeField]
    private int zSize;

    [SerializeField]
    private GameObject player;

    private Mesh mesh;

    private int[] triangles;
    private Vector3[] vertices;

    private Vector3 startPosition;

    private int xPlayerLocation => (int)Mathf.Floor(player.transform.position.x);
    private int zPlayerLocation => (int)Mathf.Floor(player.transform.position.z);

    private int xPlayerMove => (int)(player.transform.position.x - startPosition.x);
    private int zPlayerMove => (int)(player.transform.position.z - startPosition.z);


    // Start is called before the first frame update
    void Start()
    {
        mesh = new();

        GetComponent<MeshFilter>().mesh = mesh;

        GenerateMesh();
        UpdateMesh();

    }

    private void Update()
    {
        if (Mathf.Abs(xPlayerMove) >= BlockSize || Mathf.Abs(zPlayerMove) >= BlockSize)
        {

        }

    }


    private void GenerateMesh()
    {
        vertices = new Vector3[(xSize + 1) * (zSize + 1)];

        for (int i = 0, z = 0; z <= zSize; z++)
        {
            for (int x = 0; x <= xSize; x++)
            {
                var y = Mathf.PerlinNoise(x * NoiseMultiplier, z * NoiseMultiplier) * 2f;
                vertices[i] = new Vector3(x, y, z);
                i++;
            }
        }

        triangles = new int[xSize * zSize * 6];

        int vert = 0;
        int tris = 0;
        for (int z = 0; z < zSize; z++)
        {
            for (int x = 0; x < xSize; x++)
            {
                triangles[tris + 0] = vert + 0;
                triangles[tris + 1] = vert + xSize + 1;
                triangles[tris + 2] = vert + 1;
                triangles[tris + 3] = vert + 1;
                triangles[tris + 4] = vert + xSize + 1;
                triangles[tris + 5] = vert + xSize + 2;

                vert++;
                tris += 6;
            }
            vert++;
        }

    }

    private void UpdateMesh()
    {
        mesh.Clear();

        mesh.vertices = vertices;
        mesh.triangles = triangles;

        mesh.RecalculateNormals();
    }
}
