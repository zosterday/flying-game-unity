using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MeshGenerator : MonoBehaviour
{
    [SerializeField]
    private int xSize;

    [SerializeField]
    private int zSize;

    private Mesh mesh;

    private int[] triangles;
    private Vector3[] vertices;

    // Start is called before the first frame update
    void Start()
    {
        mesh = new();

        GetComponent<MeshFilter>().mesh = mesh;

        GenerateMesh();
        UpdateMesh();

    }

    // Update is called once per frame
    void Update()
    {

    }

    private void GenerateMesh()
    {
        triangles = new int[xSize * zSize * 6];
        vertices = new Vector3[(xSize + 1) * (zSize + 1)];

        for (int i = 0, z = 0; z <= zSize; z++)
        {
            for (var x = 0; x <= xSize; x++)
            {
                vertices[i] = new Vector3(x, 0, z);
                i++;
            }
        }

        int tris = 0;
        int verts = 0;

        for (var z = 0; z < zSize; z++)
        {
            for (var x = 0; x < xSize; x++)
            {
                triangles[tris + 0] = verts + 0;
                triangles[tris + 1] = verts + zSize + 1;
                triangles[tris + 2] = verts + 1;

                triangles[tris + 3] = verts + 1;
                triangles[tris + 4] = verts + zSize + 1;
                triangles[tris + 5] = verts + zSize + 2;

                verts++;
                tris += 6;
            }
            verts++;
        }
    }

    private void UpdateMesh()
    {
        mesh.Clear();

        mesh.vertices = vertices;
        mesh.triangles = triangles;

        mesh.RecalculateNormals();
    }















    //[SerializeField]
    //private int xSize;

    //[SerializeField]
    //private int zSize;

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

    //// Update is called once per frame
    //void Update()
    //{

    //}

    //private void GenerateMesh()
    //{
    //    triangles = new int[xSize * zSize * 6];
    //    vertices = new Vector3[(xSize + 1) * (zSize + 1)];

    //    for (int i = 0, z = 0; z <= zSize; z++)
    //    {
    //        for (var x = 0; x <= xSize; x++)
    //        {
    //            vertices[i] = new Vector3(x, 0, z);
    //            i++;
    //        }
    //    }

    //    int tris = 0;
    //    int verts = 0;

    //    for (var z = 0; z < zSize; z++)
    //    {
    //        for (var x = 0; x < xSize; x++)
    //        {
    //            triangles[tris + 0] = verts + 0;
    //            triangles[tris + 1] = verts + zSize + 1;
    //            triangles[tris + 2] = verts + 1;

    //            triangles[tris + 3] = verts + 1;
    //            triangles[tris + 4] = verts + zSize + 1;
    //            triangles[tris + 5] = verts + zSize + 2;

    //            verts++;
    //            tris += 6;
    //        }
    //        verts++;
    //    }
    //}

    //private void UpdateMesh()
    //{
    //    mesh.Clear();

    //    mesh.vertices = vertices;
    //    mesh.triangles = triangles;

    //    mesh.RecalculateNormals();
    //}

    //private void OnDrawGizmos()
    //{
    //    if (vertices is null)
    //    {
    //        return;
    //    }

    //    for (int i = 0; i < vertices.Length; i++)
    //    {
    //        Gizmos.DrawSphere(vertices[i], 0.1f);
    //    }
    //}
}
