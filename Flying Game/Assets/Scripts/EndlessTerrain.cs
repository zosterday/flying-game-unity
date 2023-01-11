using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EndlessTerrain : MonoBehaviour
{
    public const float MaxViewDistance = 450f;

    public Transform viewer;

    [SerializeField]
    private Material mapMaterial;

    public static Vector2 ViewerPosition;

    private static MapGenerator mapGenerator;

    private int chunkSize;

    private int chunksVisibleInViewDistance;

    private Dictionary<Vector2, TerrainChunk> terrainChunksByCoords = new();

    private List<TerrainChunk> chunksVisibleLastUpdate = new();

    private void Start()
    {
        mapGenerator = FindObjectOfType<MapGenerator>();
        chunkSize = MapGenerator.MapChunkSize - 1;
        chunksVisibleInViewDistance = Mathf.RoundToInt(MaxViewDistance / chunkSize);
    }

    private void Update()
    {
        ViewerPosition = new Vector2(viewer.position.x, viewer.position.z);
        UpdateVisibleChunks();
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
                    terrainChunksByCoords.Add(viewedChunkCoord, new TerrainChunk(viewedChunkCoord, chunkSize, this.transform, mapMaterial));
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

        public TerrainChunk(Vector2 coord, int size, Transform parent, Material material)
        {
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

            mapGenerator.RequestMapData(OnMapDataReceived);
        }

        private void OnMapDataReceived(MapData mapData)
        {
            mapGenerator.RequestMeshData(mapData, OnMeshDataReceived);
        }

        private void OnMeshDataReceived(MeshData meshData)
        {
            meshFilter.mesh = meshData.CreateMesh();
        }

        public void UpdateTerrainChunk()
        {
            var viewerDistanceFromNearestEdge = Mathf.Sqrt(bounds.SqrDistance(ViewerPosition));
            var visible = viewerDistanceFromNearestEdge <= MaxViewDistance;
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
}
