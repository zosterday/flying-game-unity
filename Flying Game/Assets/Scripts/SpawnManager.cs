using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpawnManager : MonoBehaviour
{
    public int CheckpointSpawnCount { get; set; }

    public List<GameObject> ActiveCheckpointList { get; set; }

    private const int CheckpointSpawnMax = 8;

    private const int CheckpointPosYCenter = 100;

    private const float CheckpointZOffset = 50f;

    private const float CheckpointPosMinOffset = -20;

    private const float CheckpointPosMaxOffset = 20f;

    private Vector3 firstCheckpointSpawnPos = new(0, 100, 50);

    private Vector3 lastCheckpointPos;

    private bool firstCheckpoint;

    private readonly System.Random random = new();

    // Start is called before the first frame update
    void Start()
    {
        firstCheckpoint = true;
        ActiveCheckpointList = new();
    }

    // Update is called once per frame
    void Update()
    {
        var checkpointsToSpawn = CheckpointSpawnMax - CheckpointSpawnCount;

        if (checkpointsToSpawn != 0)
        {
            SpawnCheckpoints(checkpointsToSpawn);
        }
    }

    private float GetRandomFloat(float min, float max)
    {
        return (float)random.NextDouble() * (max - min) + min;
    }

    private void SpawnCheckpoints(int count)
    {
        for (var i = 0; i < count; i++)
        {
            if (firstCheckpoint)
            {
                lastCheckpointPos = firstCheckpointSpawnPos;
                firstCheckpoint = false;
            }
            else
            {
                var xPos = GetRandomFloat(CheckpointPosMinOffset * 1.5f, CheckpointPosMaxOffset * 1.5f);
                var yPos = CheckpointPosYCenter + GetRandomFloat(CheckpointPosMinOffset * 0.8f, CheckpointPosMaxOffset * 0.8f);
                var zPos = lastCheckpointPos.z + CheckpointZOffset;

                lastCheckpointPos = new Vector3(xPos, yPos, zPos);
            }

            var checkpoint = ObjectPooler.Instance.GetCheckpointObject();
            checkpoint.transform.position = lastCheckpointPos;
            checkpoint.SetActive(true);
            ActiveCheckpointList.Add(checkpoint);
            CheckpointSpawnCount++;
        }
    }
}
