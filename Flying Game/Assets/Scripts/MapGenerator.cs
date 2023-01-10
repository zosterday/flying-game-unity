using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MapGenerator : MonoBehaviour
{
    [SerializeField]
    private DrawMode drawMode;

    public const int MapChunkSize = 241;

    [SerializeField]
    [Range(0,6)]
    private int levelOfDetail;

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
    private float meshHeightMultiplier;

    [SerializeField]
    private AnimationCurve meshHeightCurve;

    [SerializeField]
    private Vector2 offset;

    [SerializeField]
    private TerrainType[] regions;

    public bool autoUpdate;

    public void GenerateMap()
    {
        var noiseMap = Noise.GenerateNoiseMap(MapChunkSize, MapChunkSize, seed, noiseScale,octaves, persistence, lacunarity, offset);

        var colorMap = new Color[MapChunkSize * MapChunkSize];

        for (var y = 0; y < MapChunkSize; y++)
        {
            for (var x = 0; x < MapChunkSize; x++)
            {
                var currentHeight = noiseMap[x, y];
                for (var i = 0; i < regions.Length; i++)
                {
                    if (currentHeight <= regions[i].Height)
                    {
                        colorMap[y * MapChunkSize + x] = regions[i].Color;
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
            display.DrawTexture(TextureGenerator.TextureFromColorMap(colorMap, MapChunkSize, MapChunkSize));
        }
        else if (drawMode == DrawMode.Mesh)
        {
            display.DrawMesh(MeshGenerator.GenerateTerrainMesh(noiseMap, meshHeightMultiplier, meshHeightCurve, levelOfDetail), TextureGenerator.TextureFromColorMap(colorMap, MapChunkSize, MapChunkSize));
        }
    }

    private void OnValidate()
    {
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