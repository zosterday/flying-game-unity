using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;


public static class MeshGenerator
{
    public static MeshData GenerateTerrainMesh(float[,] heightMap, float heightMultiplier, AnimationCurve heightCurve, int levelOfDetail)
    {
        var workingHeightCurve = new AnimationCurve(heightCurve.keys);

        var width = heightMap.GetLength(0);
        var height = heightMap.GetLength(1);
        var topLeftX = (width - 1) * (-0.5f);
        var topLeftZ = (height - 1) * (0.5f);

        var meshSimplificationIncrement = (levelOfDetail == 0) ? 1 : levelOfDetail * 2;
        var verticesPerLine = (width - 1) / meshSimplificationIncrement + 1;

        var meshData = new MeshData(verticesPerLine, verticesPerLine);
        var vertexIndex = 0;

        for (var y = 0; y < height; y += meshSimplificationIncrement)
        {
            for (var x = 0; x < width; x += meshSimplificationIncrement)
            {
                var heightMapVal = heightMap[x, y];
                meshData.Vertices[vertexIndex] = new Vector3(topLeftX + x, workingHeightCurve.Evaluate(heightMapVal) * heightMultiplier, topLeftZ - y);
                meshData.Uvs[vertexIndex] = new Vector2(x / (float)width, y / (float)height);

                if (x < width - 1 && y < height - 1)
                {
                    meshData.AddTriangle(vertexIndex, vertexIndex + verticesPerLine + 1, vertexIndex + verticesPerLine);
                    meshData.AddTriangle(vertexIndex + verticesPerLine + 1, vertexIndex, vertexIndex + 1);
                }

                vertexIndex++;
            }
        }

        return meshData;
    }
}
