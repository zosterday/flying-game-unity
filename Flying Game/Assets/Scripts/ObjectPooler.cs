using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ObjectPooler : MonoBehaviour
{
    public static ObjectPooler Instance;

    private const int CheckpointPoolAmount = 10;

    [SerializeField]
    private GameObject checkpointObj;

    private List<GameObject> checkpointPool;


    private void Awake()
    {
        Instance = this;
    }

    // Start is called before the first frame update
    void Start()
    {
        checkpointPool = new();

        for (var i = 0; i < CheckpointPoolAmount; i++)
        {
            var obj = Instantiate(checkpointObj);
            obj.SetActive(false);
            checkpointPool.Add(obj);
            obj.transform.SetParent(this.transform);
        }
    }

    public GameObject GetCheckpointObject()
    {
        foreach (var checkpoint in checkpointPool)
        {
            if (!checkpoint.activeInHierarchy)
            {
                return checkpoint;
            }
        }

        return null;
    }
}
