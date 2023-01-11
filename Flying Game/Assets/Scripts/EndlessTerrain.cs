using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EndlessTerrain : MonoBehaviour
{
    const float playerMoveThresholdForUpdate = 25f;

    const float sqrViewerMoveThresholdForUpdate = playerMoveThresholdForUpdate * playerMoveThresholdForUpdate;

    public LODInfo[] detailLevels;

    public static float MaxViewDistance = 350f;

    public Transform viewer;

    [SerializeField]
    private Material mapMaterial;

    public static Vector2 ViewerPosition;

    private static MapGenerator mapGenerator;

    private int chunkSize;

    private int chunksVisibleInViewDistance;

    private Vector2 playerPositionOld;

    private Dictionary<Vector2, TerrainChunk> terrainChunksByCoords = new();

    private List<TerrainChunk> chunksVisibleLastUpdate = new();

    private void Start()
    {
        mapGenerator = FindObjectOfType<MapGenerator>();

        MaxViewDistance = detailLevels[detailLevels.Length - 1].visibleDistThreshold;
        chunkSize = MapGenerator.MapChunkSize - 1;
        chunksVisibleInViewDistance = Mathf.RoundToInt(MaxViewDistance / chunkSize);

        UpdateVisibleChunks();
    }

    private void Update()
    {
        ViewerPosition = new Vector2(viewer.position.x, viewer.position.z);

        if ((playerPositionOld - ViewerPosition).sqrMagnitude > sqrViewerMoveThresholdForUpdate)
        {
            playerPositionOld = ViewerPosition;
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

        var currentChunkCoordX = Mathf.RoundToInt(ViewerPosition.x / chunkSize);
        var currentChunkCoordY = Mathf.RoundToInt(ViewerPosition.y / chunkSize);

        for (var yOffset = -chunksVisibleInViewDistance; yOffset <= chunksVisibleInViewDistance; yOffset++)
        {
            for (var xOffset = -chunksVisibleInViewDistance; xOffset <= chunksVisibleInViewDistance; xOffset++)
            {
                var viewedChunkCoord = new Vector2(currentChunkCoordX + xOffset, currentChunkCoordY + yOffset);

                if (terrainChunksByCoords.ContainsKey(viewedChunkCoord))
                {
                    terrainChunksByCoords[viewedChunkCoord].UpdateTerrainChunk();

                    if (terrainChunksByCoords[viewedChunkCoord].IsVisible())
                    {
                        chunksVisibleLastUpdate.Add(terrainChunksByCoords[viewedChunkCoord]);
                    }
                }
                else
                {
                    terrainChunksByCoords.Add(viewedChunkCoord, new TerrainChunk(viewedChunkCoord, chunkSize, detailLevels, this.transform, mapMaterial));
                }
            }
        }
    }

    public class TerrainChunk
    {
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

            meshObject = new GameObject("Terrain Chunk");
            meshRenderer = meshObject.AddComponent<MeshRenderer>();
            meshFilter = meshObject.AddComponent<MeshFilter>();
            meshRenderer.material = material;
            meshObject.transform.position = positionV3;
            meshObject.transform.parent = parent;
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

        private void OnMeshDataReceived(MeshData meshData)
        {
            meshFilter.mesh = meshData.CreateMesh();
        }

        public void UpdateTerrainChunk()
        {
            if (!mapDataReceieved)
            {
                return;
            }

            var viewerDistanceFromNearestEdge = Mathf.Sqrt(bounds.SqrDistance(ViewerPosition));
            var visible = viewerDistanceFromNearestEdge <= MaxViewDistance;

            if (visible)
            {
                var lodIndex = 0;

                for (var i = 0; i < detailLevels.Length - 1; i++)
                {
                    if (viewerDistanceFromNearestEdge > detailLevels[i].visibleDistThreshold)
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
                    if (lodMesh.hasMesh)
                    {
                        previousLodIndex = lodIndex;
                        meshFilter.mesh = lodMesh.mesh;
                    }
                    else if (!lodMesh.hasRequestedMesh)
                    {
                        lodMesh.RequestMesh(mapData);
                    }
                }
            }

            SetVisible(visible);
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
        public Mesh mesh;

        public bool hasRequestedMesh;

        public bool hasMesh;

        int lod;

        System.Action updateCallback;

        public LODMesh(int lod, System.Action updateCallback)
        {
            this.lod = lod;
            this.updateCallback = updateCallback;
        }

        private void OnMeshDataReceived(MeshData meshData)
        {
            mesh = meshData.CreateMesh();
            hasMesh = true;

            updateCallback();
        }

        public void RequestMesh(MapData mapData)
        {
            hasRequestedMesh = true;
            mapGenerator.RequestMeshData(mapData, lod, OnMeshDataReceived);
        }
    }

    [System.Serializable]
    public struct LODInfo
    {
        public int lod;
        public float visibleDistThreshold;
    }
}
