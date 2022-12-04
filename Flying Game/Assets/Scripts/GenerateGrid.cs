using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GenerateGrid : MonoBehaviour
{
    private const float DetailScale = 8f;

    private const int MaxSceneObjectCount = 20;

    [SerializeField]
    private GameObject blockGameObject;

    [SerializeField]
    private GameObject objectToSpawn;

    [SerializeField]
    private GameObject player;

    private int worldSizeX = 40;

    private int worldSizeZ = 40;

    private int noiseHeight = 5;

    private float gridOffset = 1.1f;

    private Vector3 startPosition;

    private Hashtable blockContainer = new();

    private List<Vector3> blockPositions = new();

    private int xPlayerLocation => (int)Mathf.Floor(player.transform.position.x);

    private int zPlayerLocation => (int)Mathf.Floor(player.transform.position.z);

    public int XPlayerMove => (int)(player.transform.position.x - startPosition.x);

    public int ZPlayerMove => (int)(player.transform.position.z - startPosition.z);

    // Start is called before the first frame update
    void Start()
    {
        for (var x = -worldSizeX; x < worldSizeX; x++)
        {
            for (int z = -worldSizeZ; z < worldSizeZ; z++)
            {
                var blockSize = 1f;
                var pos = new Vector3(x * blockSize * startPosition.x,
                    GenerateNoise(x, z, DetailScale) * noiseHeight,
                    z * blockSize * startPosition.z);

                var block = Instantiate(blockGameObject, pos, Quaternion.identity) as GameObject;


                blockContainer.Add(pos, block);
                blockPositions.Add(block.transform.position);

                block.transform.SetParent(this.transform);
            }
        }

        //SpawnObjects();
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    //private void SpawnObjects()
    //{
    //    for (var i = 0; i < MaxSceneObjectCount; i++)
    //    {
    //        var sceneObject = Instantiate(objectToSpawn, ObjectSpawnLocation(), Quaternion.identity);
    //    }
    //}

    //private Vector3 ObjectSpawnLocation()
    //{
    //    int randomIndex = Random.Range(0, blockPositions.Count);

    //    Vector3 newPos = new Vector3(
    //        blockPositions[randomIndex].x,
    //        blockPositions[randomIndex].y + 0.5f,
    //        blockPositions[randomIndex].z);

    //    blockPositions.RemoveAt(randomIndex);
    //    return newPos;
    //}

    private float GenerateNoise(int x, int z, float detailScale)
    {
        float xNoise = (x + transform.position.x) / detailScale;
        float zNoise = (z + transform.position.z) / detailScale;

        return Mathf.PerlinNoise(xNoise, zNoise);
    }
}
