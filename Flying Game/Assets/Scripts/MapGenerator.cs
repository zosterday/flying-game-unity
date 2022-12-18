using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MapGenerator : MonoBehaviour
{
    [SerializeField]
    private DrawMode drawMode;

    [SerializeField]
    private int mapWidth;

    [SerializeField]
    private int mapHeight;

    [SerializeField]
    private float noiseScale;

    [SerializeField]
    private int octaves;

    [Range(0,1)]
    [SerializeField]
    private float persistence;

    [SerializeField]
    private float lacunarity;

    [SerializeField]
    private int seed;

    [SerializeField]
    private Vector2 offset;

    [SerializeField]
    private TerrainType[] regions;

    public bool autoUpdate;

    public void GenerateMap()
    {
        var noiseMap = Noise.GenerateNoiseMap(mapWidth, mapHeight, seed, noiseScale,octaves, persistence, lacunarity, offset);

        var colorMap = new Color[mapWidth * mapHeight];

        for (var y = 0; y < mapHeight; y++)
        {
            for (var x = 0; x < mapWidth; x++)
            {
                var currentHeight = noiseMap[x, y];
                for (var i = 0; i < regions.Length; i++)
                {
                    if (currentHeight <= regions[i].Height)
                    {
                        colorMap[y * mapWidth + x] = regions[i].Color;
                        break;
                    }
                }
            }
        }

        MapDisplay display = FindObjectOfType<MapDisplay>();

        if (drawMode == DrawMode.NoiseMap)
        {
            display.DrawTexture(TextureGenerator.TextureFromHeightMap(noiseMap));
        }
        else if (drawMode == DrawMode.ColorMap)
        {
            display.DrawTexture(TextureGenerator.TextureFromColorMap(colorMap, mapWidth, mapHeight));
        }
        else if (drawMode == DrawMode.Mesh)
        {
            display.DrawMesh(MeshGenerator.GenerateTerrainMesh(noiseMap), TextureGenerator.TextureFromColorMap(colorMap, mapWidth, mapHeight));
        }
    }

    private void OnValidate()
    {
        if (mapWidth < 1)
        {
            mapWidth = 1;
        }
        if (mapHeight < 1)
        {
            mapHeight = 1;
        }
        if (lacunarity < 1)
        {
            lacunarity = 1;
        }
        if (octaves < 0)
        {
            octaves = 0;
        }
    }

    public enum DrawMode
    {
        NoiseMap,
        ColorMap,
        Mesh,
    };
}

[System.Serializable]
public struct TerrainType
{
    [SerializeField]
    private string name;

    [SerializeField]
    private float height;

    [SerializeField]
    private Color color;

    public float Height => height;

    public Color Color => color;
}