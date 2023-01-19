using UnityEngine;
using UnityEngine.AI;

public class PlayerController : MonoBehaviour
{
    private const string CheckpointTag = "Checkpoint";
    [SerializeField]
    private GameObject raycastPlane;

    //[SerializeField]
    private float forwardSpeed = 50f;

    //[SerializeField]
    private float maxSpeed = 500f;

    //[SerializeField]
    float acceleration = 5f;

    //[SerializeField]
    private float tiltAmount = 20f;

    private Rigidbody rb;
    private Ray position;
    private float roll;
    private float pitch;

    private void Start()
    {
        rb = GetComponent<Rigidbody>();
    }

    private void Update()
    {
        // Get the position of the mouse pointer
        position = Camera.main.ScreenPointToRay(Input.mousePosition);

        if (Physics.Raycast(position, out var raycastHit))
        {
            var pos = raycastHit.point;
            transform.position = new Vector3(pos.x, pos.y, transform.position.z);
        }

        // Get mouse input to control player's roll
        float mouseX = Input.GetAxis("Mouse X");
        float mouseY = Input.GetAxis("Mouse Y");

        // Apply roll and pitch rotations to the player
        roll -= mouseX * tiltAmount;
        roll = Mathf.Clamp(roll, -55f, 55f);
        pitch -= mouseY * tiltAmount;
        pitch = Mathf.Clamp(pitch, -35f, 35f);
        transform.eulerAngles = new Vector3(pitch, 0f, roll);

        // Accelerate the player forward over time
        forwardSpeed += Time.deltaTime * acceleration;
        forwardSpeed = Mathf.Min(forwardSpeed, maxSpeed);

        // Move the player forward on the z-axis
        Vector3 forwardMovement = new Vector3(0, 0, forwardSpeed);
        rb.velocity = forwardMovement;

        var raycastPlanePos = raycastPlane.transform.position;
        raycastPlane.transform.position = new Vector3(raycastPlanePos.x, raycastPlanePos.y, transform.position.z + 10f);
    }

    private void OnCollisionEnter(Collision collision)
    {
        rb.velocity = Vector3.zero;
    }

    private void OnTriggerEnter(Collider other)
    {
        rb.velocity = Vector3.zero;
        //Add particle effect for little poof thing around the loop
        if (other.CompareTag(CheckpointTag))
        {
            other.gameObject.SetActive(false);
            SpawnManager.CheckpointSpawnCount--;
            Debug.Log("Gay");
        }
    }
}
