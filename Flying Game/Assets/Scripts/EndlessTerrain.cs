using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EndlessTerrain : MonoBehaviour
{
    public const float MaxViewDistance = 300f;

    public Transform viewer;

    public static Vector2 ViewerPosition;

    private int chunkSize;

    private int chunksVisibleInViewDistance;

    private Dictionary<Vector2, TerrainChunk> terrainChunksByCoords = new();

    private void Start()
    {
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
                }
                else
                {
                    terrainChunksByCoords.Add(viewedChunkCoord, new TerrainChunk(viewedChunkCoord, chunkSize));
                }
            }
        }
    }

    public class TerrainChunk
    {
        private GameObject meshObject;

        private Vector2 position;

        private Bounds bounds;

        public TerrainChunk(Vector2 coord, int size)
        {
            position = coord * size;
            bounds = new Bounds(position, Vector2.one * size);
            var positionV3 = new Vector3(position.x, 0, position.y);

            meshObject = GameObject.CreatePrimitive(PrimitiveType.Plane);
            meshObject.transform.position = positionV3;
            meshObject.transform.localScale = Vector3.one * size / 10f;
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
    }
}
