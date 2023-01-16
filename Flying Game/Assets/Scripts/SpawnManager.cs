using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpawnManager : MonoBehaviour
{
    private const int CheckpointSpawnMax = 5;

    private const int CheckpointPosYCenter = 100;

    private const float CheckpointZOffset = 100f;

    private const float CheckpointPosMinOffset = -100f;

    private const float CheckpointPosMaxOffset = 100f;

    private Vector3 firstCheckpointSpawnPos = new(0, 100, 50);

    private int checkpointSpawnCount;

    private Vector3 lastCheckpointPos;

    private readonly System.Random random = new();

    // Start is called before the first frame update
    void Start()
    {
 
    }

    // Update is called once per frame
    void Update()
    {
        var checkpointsToSpawn = CheckpointSpawnMax - checkpointSpawnCount;

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
            if (i == 0)
            {
                lastCheckpointPos = firstCheckpointSpawnPos;
            }
            else
            {
                var xPos = GetRandomFloat(CheckpointPosMinOffset, CheckpointPosMaxOffset);
                var yPos = CheckpointPosYCenter + GetRandomFloat(CheckpointPosMinOffset, CheckpointPosMaxOffset);
                var zPos = lastCheckpointPos.z + CheckpointZOffset;

                lastCheckpointPos = new Vector3(xPos, yPos, zPos);
            }

            var checkpoint = ObjectPooler.Instance.GetCheckpointObject();
            checkpoint.transform.position = lastCheckpointPos;
            checkpoint.SetActive(true);
            checkpointSpawnCount++;
        }
    }
}
