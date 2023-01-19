using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Checkpoint : MonoBehaviour
{
    public bool IsReadyForInactive { get; set; }

    // Start is called before the first frame update
    void Start()
    {
        IsReadyForInactive = false;
    }

    // Update is called once per frame
    void Update()
    {
        if (IsReadyForInactive)
        {

        }
    }

    private IEnumerator SetInactive()
    {
        yield return new WaitForSeconds(1.5f);
        gameObject.SetActive(false);
        IsReadyForInactive = false;
    }
}
