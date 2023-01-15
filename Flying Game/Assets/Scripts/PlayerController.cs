//using UnityEngine;

//public class PlayerController : MonoBehaviour
//{
//    [SerializeField]
//    private float forwardSpeed = 10f;
//    [SerializeField]
//    private float maxSpeed = 50f;
//    [SerializeField]
//    private float acceleration = 1f;
//    //[SerializeField]
//    //private float tiltAmount = 20.0f;
//    //[SerializeField]
//    //private float tiltSmoothTime = 0.1f;

//    private Rigidbody rb;
//    //private float yaw;
//    //private float pitch;
//    private Vector3 targetPosition;

//    private void Start()
//    {
//        rb = GetComponent<Rigidbody>();
//    }

//    private void Update()
//    {
//        // Get the position of the mouse pointer
//        targetPosition = Camera.main.ScreenToWorldPoint(new Vector3(Input.mousePosition.x, Input.mousePosition.y, 0));

//        // Get mouse input to control player's tilt
//        //float mouseX = Input.GetAxis("Mouse X");
//        //float mouseY = Input.GetAxis("Mouse Y");

//        //// Apply tilt to the player
//        //yaw += mouseX * tiltAmount;
//        //pitch -= mouseY * tiltAmount;
//        //pitch = Mathf.Clamp(pitch, -90, 90);
//        //transform.eulerAngles = new Vector3(pitch, yaw, 0);

//        // Accelerate the player forward over time
//        forwardSpeed += Time.deltaTime * acceleration;
//        forwardSpeed = Mathf.Min(forwardSpeed, maxSpeed);

//        // Move the player towards the position of the mouse pointer
//        Vector3 forwardMovement = transform.forward * forwardSpeed;
//        Vector3 towardsMouse = (targetPosition - transform.position).normalized * forwardSpeed;
//        rb.velocity = forwardMovement + towardsMouse;
//    }

//    private void OnCollisionEnter(Collision collision)
//    {
//        // Stop the player's movement when it hits an object
//        rb.velocity = Vector3.zero;
//    }
//}




















using UnityEngine;
using UnityEngine.AI;

public class PlayerController : MonoBehaviour
{
    [SerializeField]
    private GameObject raycastPlane;

    //[SerializeField]
    private float forwardSpeed = 50f;

    //[SerializeField]
    private float maxSpeed = 500f;

    //[SerializeField]
    float acceleration = 5f;

    //[SerializeField]
    private float tiltAmount = 20.0f;

    //[SerializeField]
    //private float forwardMovementSpeed = 1f;

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
        roll = Mathf.Clamp(roll, -70f, 70f);
        pitch += mouseY * tiltAmount;
        pitch = Mathf.Clamp(pitch, -60f, 60f);
        transform.eulerAngles = new Vector3(0f, pitch, roll);

        // Accelerate the player forward over time
        forwardSpeed += Time.deltaTime * acceleration;
        forwardSpeed = Mathf.Min(forwardSpeed, maxSpeed);

        // Move the player forward on the z-axis
        Vector3 forwardMovement = new Vector3(0, 0, forwardSpeed);
        rb.velocity = forwardMovement;

        var raycastPlanePos = raycastPlane.transform.position;
        raycastPlane.transform.position = new Vector3(raycastPlanePos.x, raycastPlanePos.y, transform.position.z + 4f);
    }

    private void OnCollisionEnter(Collision collision)
    {
        // Stop the player's movement when it hits an object
        rb.velocity = Vector3.zero;
    }
}
