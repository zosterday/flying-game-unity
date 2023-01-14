//using System.Collections;
//using System.Collections.Generic;
//using UnityEngine;

//public class GenerateGrid : MonoBehaviour
//{
//    private const float DetailScale = 8f;

//    private const int MaxSceneObjectCount = 20;

//    private const float BlockSize = 1f;

//    [SerializeField]
//    private GameObject blockGameObject;

//    [SerializeField]
//    private GameObject objectToSpawn;

//    [SerializeField]
//    private GameObject player;

//    [SerializeField]
//    private int worldSizeX = 40;

//    [SerializeField]
//    private int worldSizeZ = 40;

//    private int noiseHeight = 5;

//    private float gridOffset = 1.1f;

//    private Vector3 startPosition;

//    private Hashtable blockContainer = new();

//    private List<Vector3> blockPositions = new();

//    private int xPlayerLocation => (int)Mathf.Floor(player.transform.position.x);
//    private int zPlayerLocation => (int)Mathf.Floor(player.transform.position.z);

//    private int xPlayerMove => (int)(player.transform.position.x - startPosition.x);
//    private int zPlayerMove => (int)(player.transform.position.z - startPosition.z);

//    // Start is called before the first frame update
//    void Start()
//    {
//        for (var x = -worldSizeX; x < worldSizeX; x++)
//        {
//            for (int z = -worldSizeZ; z < worldSizeZ; z++)
//            {
//                var pos = new Vector3(x * BlockSize + startPosition.x,
//                    GenerateNoise(x, z, DetailScale) * noiseHeight,
//                    z * BlockSize + startPosition.z);

//                var block = Instantiate(blockGameObject, pos, Quaternion.identity) as GameObject;


//                blockContainer.Add(pos, block);
//                blockPositions.Add(block.transform.position);

//                block.transform.SetParent(this.transform);
//            }
//        }

//        //SpawnObjects();
//    }

//    // Update is called once per frame
//    void Update()
//    {
//        if (Mathf.Abs(xPlayerMove) >= BlockSize || Mathf.Abs(zPlayerMove) >= BlockSize)
//        {
//            for (var x = -worldSizeX; x < worldSizeX; x++)
//            {
//                for (int z = -worldSizeZ; z < worldSizeZ; z++)
//                {
//                    var pos = new Vector3(x * BlockSize + xPlayerLocation,
//                        GenerateNoise(x + xPlayerLocation, z + zPlayerLocation, DetailScale) * noiseHeight,
//                        z * BlockSize + zPlayerLocation);

//                    if (!blockContainer.ContainsKey(pos))
//                    {
//                        var block = Instantiate(blockGameObject, pos, Quaternion.identity) as GameObject;

//                        blockContainer.Add(pos, block);
//                        blockPositions.Add(block.transform.position);

//                        block.transform.SetParent(this.transform);
//                    }
//                }
//            }
//        }
//    }

//    private void GenerateTerrain()
//    {

//    }

//    private float GenerateNoise(int x, int z, float detailScale)
//    {
//        float xNoise = (x + transform.position.x) / detailScale;
//        float zNoise = (z + transform.position.z) / detailScale;

//        return Mathf.PerlinNoise(xNoise, zNoise);
//    }
//}
