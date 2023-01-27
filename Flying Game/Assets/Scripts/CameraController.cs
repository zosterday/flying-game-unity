using UnityEngine;
using static UnityEditor.PlayerSettings;
using static UnityEngine.GraphicsBuffer;

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
        //transform.position = new Vector3(pos.x, pos.y, player.transform.position.z + offset.z);

        //var destination = player.transform.position + offset;
        var destination = new Vector3(pos.x, pos.y, player.transform.position.z + offset.z); ;
        transform.position = Vector3.Lerp(transform.position, destination, 0.1f);
        //transform.LookAt(player.transform);
    }
}