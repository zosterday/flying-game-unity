using UnityEngine;

public class CameraController : MonoBehaviour
{
    [SerializeField]
    private Vector3 offset;

    [SerializeField]
    private GameObject player;

    private void Start()
    {
        transform.position = player.transform.position + offset;
    }

    // Update is called once per frame
    void LateUpdate()
    {
        var pos = transform.position;
        transform.position = new Vector3(pos.x, pos.y, player.transform.position.z + offset.z);
        //transform.position = player.transform.position + offset;
    }
}