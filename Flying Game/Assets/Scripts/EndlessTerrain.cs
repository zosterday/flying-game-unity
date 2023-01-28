using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EndlessTerrain : MonoBehaviour
{
    private const float Scale = 5f;

    private const float PlayerMoveThresholdForUpdate = 25f;
    private const float PlayerMoveThresholdForUpdateSqr = PlayerMoveThresholdForUpdate * PlayerMoveThresholdForUpdate;

    private static float maxViewDst;

    [SerializeField]
    private LODInfo[] detailLevels;

    [SerializeField]
    private Transform player;

    [SerializeField]
    private Material mapMaterial;

    private static Vector2 playerPosition;

    private Vector2 playerPositionOld;

    private static MapGenerator mapGenerator;

    private int chunkSize;

    private int chunksVisibleInViewDistance;

    private Dictionary<Vector2, TerrainChunk> terrainChunksByCoords;

    private static List<TerrainChunk> chunksVisibleLastUpdate;

    private void Start()
    {
        mapGenerator = FindObjectOfType<MapGenerator>();

        terrainChunksByCoords = new();
        chunksVisibleLastUpdate = new();

        maxViewDst = detailLevels[detailLevels.Length - 1].visibleDistThreshold;
        chunkSize = MapGenerator.MapChunkSize - 1;
        chunksVisibleInViewDistance = Mathf.RoundToInt(maxViewDst / chunkSize);

        UpdateVisibleChunks();
    }

    private void Update()
    {
        playerPosition = new Vector2(player.position.x, player.position.z) / Scale;

        if ((playerPositionOld - playerPosition).sqrMagnitude > PlayerMoveThresholdForUpdateSqr)
        {
            playerPositionOld = playerPosition;
            UpdateVisibleChunks();
        }
    }

    private void UpdateVisibleChunks()
    {
        foreach (var chunk in chunksVisibleLastUpdate)
        {
            chunk.SetVisible(false);
        }
        chunksVisibleLastUpdate.Clear();

        var currentChunkCoordX = Mathf.RoundToInt(playerPosition.x / chunkSize);
        var currentChunkCoordY = Mathf.RoundToInt(playerPosition.y / chunkSize);

        for (var yOffset = -chunksVisibleInViewDistance; yOffset <= chunksVisibleInViewDistance; yOffset++)
        {
            for (var xOffset = -chunksVisibleInViewDistance; xOffset <= chunksVisibleInViewDistance; xOffset++)
            {
                var viewedChunkCoord = new Vector2(currentChunkCoordX + xOffset, currentChunkCoordY + yOffset);

                if (terrainChunksByCoords.ContainsKey(viewedChunkCoord))
                {
                    terrainChunksByCoords[viewedChunkCoord].UpdateTerrainChunk();
                }
                else
                {
                    terrainChunksByCoords.Add(viewedChunkCoord, new TerrainChunk(viewedChunkCoord, chunkSize, detailLevels, this.transform, mapMaterial));
                }
            }
        }
    }

    private class TerrainChunk
    {
        private const string TerrainChunkName = "Terrain Chunk";

        private GameObject meshObject;
        private Vector2 position;
        private Bounds bounds;

        private MeshRenderer meshRenderer;
        private MeshFilter meshFilter;

        private LODInfo[] detailLevels;
        private LODMesh[] lodMeshes;

        private MapData mapData;
        private bool mapDataReceieved;
        private int previousLodIndex = -1;

        public TerrainChunk(Vector2 coord, int size, LODInfo[] detailLevels, Transform parent, Material material)
        {
            this.detailLevels = detailLevels;

            position = coord * size;
            bounds = new Bounds(position, Vector2.one * size);
            var positionV3 = new Vector3(position.x, 0, position.y);

            meshObject = new GameObject(TerrainChunkName);
            meshRenderer = meshObject.AddComponent<MeshRenderer>();
            meshFilter = meshObject.AddComponent<MeshFilter>();
            meshRenderer.material = material;

            meshObject.transform.position = positionV3 * Scale;
            meshObject.transform.parent = parent;
            meshObject.transform.localScale = Vector3.one * Scale;
            SetVisible(false);

            lodMeshes = new LODMesh[detailLevels.Length];
            for (var i = 0; i < detailLevels.Length; i++)
            {
                lodMeshes[i] = new LODMesh(detailLevels[i].lod, UpdateTerrainChunk);
            }

            mapGenerator.RequestMapData(position, OnMapDataReceived);
        }

        private void OnMapDataReceived(MapData mapData)
        {
            this.mapData = mapData;
            mapDataReceieved = true;

            var texture = TextureGenerator.TextureFromColorMap(mapData.colorMap, MapGenerator.MapChunkSize, MapGenerator.MapChunkSize);
            meshRenderer.material.mainTexture = texture;

            UpdateTerrainChunk();
        }

        public void UpdateTerrainChunk()
        {
            if (!mapDataReceieved)
            {
                return;
            }

            var playerDistanceFromNearestEdge = Mathf.Sqrt(bounds.SqrDistance(playerPosition));
            var visible = playerDistanceFromNearestEdge <= maxViewDst;

            SetVisible(visible);

            if (!visible)
            {
                return;
            }

            var lodIndex = 0;

            for (var i = 0; i < detailLevels.Length - 1; i++)
            {
                if (playerDistanceFromNearestEdge > detailLevels[i].visibleDistThreshold)
                {
                    lodIndex = i + 1;
                }
                else
                {
                    break;
                }
            }

            if (lodIndex != previousLodIndex)
            {
                var lodMesh = lodMeshes[lodIndex];
                if (lodMesh.HasMesh)
                {
                    previousLodIndex = lodIndex;
                    meshFilter.mesh = lodMesh.Mesh;
                }
                else if (!lodMesh.HasRequestedMesh)
                {
                    lodMesh.RequestMesh(mapData);
                }
            }

            chunksVisibleLastUpdate.Add(this);
            //TODO: maybe destry the old terrain chunk here if it is no longer visible
        }

        public void SetVisible(bool visible)
        {
            meshObject.SetActive(visible);
        }

        public bool IsVisible()
        {
            return meshObject.activeSelf;
        }
    }

    private class LODMesh
    {
        public Mesh Mesh { get; private set; }
        public bool HasRequestedMesh { get; private set; }
        public bool HasMesh { get; private set; }

        int lod;

        System.Action updateCallback;

        public LODMesh(int lod, System.Action updateCallback)
        {
            this.lod = lod;
            this.updateCallback = updateCallback;
        }

        private void OnMeshDataReceived(MeshData meshData)
        {
            Mesh = meshData.CreateMesh();
            HasMesh = true;

            updateCallback();
        }

        public void RequestMesh(MapData mapData)
        {
            HasRequestedMesh = true;
            mapGenerator.RequestMeshData(mapData, lod, OnMeshDataReceived);
        }
    }

    [Serializable]
    public struct LODInfo
    {
        public int lod;
        public float visibleDistThreshold;
    }
}
