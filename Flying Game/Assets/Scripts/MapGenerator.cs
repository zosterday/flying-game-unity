using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using System.Threading;

public class MapGenerator : MonoBehaviour
{
    public const int MapChunkSize = 241;

    public bool AutoUpdate { get; }

    [SerializeField]
    private DrawMode drawMode;

    [SerializeField]
    private Noise.NormalizeMode normalizeMode;

    [SerializeField]
    [Range(0, 6)]
    private int editorPreviewLod;

    [SerializeField]
    private float noiseScale;

    [SerializeField]
    private int octaves;

    [Range(0, 1)]
    [SerializeField]
    private float persistance;

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

    private readonly Queue<MapThreadInfo<MapData>> mapDataThreadInfoQueue = new();

    private readonly Queue<MapThreadInfo<MeshData>> meshDataThreadInfoQueue = new();

    private void Update()
    {
        if (mapDataThreadInfoQueue.Count > 0)
        {
            for (var i = 0; i < mapDataThreadInfoQueue.Count; i++)
            {
                MapThreadInfo<MapData> threadInfo = mapDataThreadInfoQueue.Dequeue();
                threadInfo.callback(threadInfo.parameter);
            }
        }

        if (meshDataThreadInfoQueue.Count > 0)
        {
            for (var i = 0; i < meshDataThreadInfoQueue.Count; i++)
            {
                MapThreadInfo<MeshData> threadInfo = meshDataThreadInfoQueue.Dequeue();
                threadInfo.callback(threadInfo.parameter);
            }
        }
    }

    public void DrawMapInEditor()
    {
        var mapData = GenerateMapData(Vector2.zero);

        MapDisplay display = FindObjectOfType<MapDisplay>();

        //todo use switch statement instead
        if (drawMode == DrawMode.NoiseMap)
        {
            display.DrawTexture(TextureGenerator.TextureFromHeightMap(mapData.heightMap));
        }
        else if (drawMode == DrawMode.ColorMap)
        {
            display.DrawTexture(TextureGenerator.TextureFromColorMap(mapData.colorMap, MapChunkSize, MapChunkSize));
        }
        else if (drawMode == DrawMode.Mesh)
        {
            display.DrawMesh(
                MeshGenerator.GenerateTerrainMesh(mapData.heightMap, meshHeightMultiplier, meshHeightCurve, editorPreviewLod),
                TextureGenerator.TextureFromColorMap(mapData.colorMap, MapChunkSize, MapChunkSize));
        }
    }

    public void RequestMapData(Vector2 center, Action<MapData> callback)
    {
        ThreadStart threadStart = delegate
        {
            MapDataThread(center, callback);
        };

        new Thread(threadStart).Start();
    }

    private void MapDataThread(Vector2 center, Action<MapData> callback)
    {
        var mapData = GenerateMapData(center);
        lock (mapDataThreadInfoQueue)
        {
            mapDataThreadInfoQueue.Enqueue(new MapThreadInfo<MapData>(callback, mapData));
        }
    }

    public void RequestMeshData(MapData mapData, int lod, Action<MeshData> callback)
    {
        ThreadStart threadStart = delegate
        {
            MeshDataThread(mapData, lod, callback);
        };

        new Thread(threadStart).Start();
    }

    private void MeshDataThread(MapData mapData, int lod, Action<MeshData> callback)
    {
        var meshData = MeshGenerator.GenerateTerrainMesh(mapData.heightMap, meshHeightMultiplier, meshHeightCurve, lod);
        lock (meshDataThreadInfoQueue)
        {
            meshDataThreadInfoQueue.Enqueue(new MapThreadInfo<MeshData>(callback, meshData));
        }
    }

    public MapData GenerateMapData(Vector2 center)
    {
        var noiseMap = Noise.GenerateNoiseMap(MapChunkSize, MapChunkSize, seed, noiseScale, octaves, persistance, lacunarity, center + offset, normalizeMode);

        var colorMap = new Color[MapChunkSize * MapChunkSize];

        for (var y = 0; y < MapChunkSize; y++)
        {
            for (var x = 0; x < MapChunkSize; x++)
            {
                var currentHeight = noiseMap[x, y];
                for (var i = 0; i < regions.Length; i++)
                {
                    if (currentHeight >= regions[i].Height)
                    {
                        colorMap[y * MapChunkSize + x] = regions[i].Color;
                    }
                    else
                    {
                        break;
                    }
                }
            }
        }

        return new MapData(noiseMap, colorMap);
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

    private struct MapThreadInfo<T>
    {
        public readonly Action<T> callback;
        public readonly T parameter;

        public MapThreadInfo(Action<T> callback, T parameter)
        {
            this.callback = callback;
            this.parameter = parameter;
        }
    }
}

