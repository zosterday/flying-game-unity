using System.Collections;
using System.Threading;
using UnityEngine;
using UnityEngine.AI;

public class PlayerController : MonoBehaviour
{
    private const string CheckpointTag = "Checkpoint";

    private const string GameManagerName = "GameManager";

    private const string SpawnManagerName = "SpawnManager";

    private const float TransformMovementSpeed = 10f;

    [SerializeField]
    private GameObject raycastPlane;

    [SerializeField]
    private ParticleSystem checkpointParticle;

    [SerializeField]
    private ParticleSystem explosionParticle;

    private GameManager gameManager;

    private SpawnManager spawnManager;

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

        gameManager = GameObject.Find(GameManagerName).GetComponent<GameManager>();
        spawnManager = GameObject.Find(SpawnManagerName).GetComponent<SpawnManager>();
    }

    private void Update()
    {
        if (!gameManager.IsGameActive)
        {
            return;
        }

        // Get the position of the mouse pointer
        position = Camera.main.ScreenPointToRay(Input.mousePosition);

        if (Physics.Raycast(position, out var raycastHit))
        {
            var pos = raycastHit.point;
            var destination = new Vector3(pos.x, pos.y, transform.position.z);
            transform.position = Vector3.Lerp(transform.position, destination, 0.1f);
        }

        // Get mouse input to control player's roll
        float mouseX = Input.GetAxis("Mouse X");
        float mouseY = Input.GetAxis("Mouse Y");

        // Apply roll and pitch rotations to the player
        var newTilt = CalculatePlayerAngle(mouseX, mouseY);
        transform.eulerAngles = Vector3.Lerp(transform.eulerAngles, newTilt, TransformMovementSpeed);

        // Accelerate the player forward over time
        forwardSpeed += Time.deltaTime * acceleration;
        forwardSpeed = Mathf.Min(forwardSpeed, maxSpeed);

        // Move the player forward on the z-axis
        Vector3 forwardMovement = new Vector3(0, 0, forwardSpeed);
        rb.velocity = forwardMovement;

        var raycastPlanePos = raycastPlane.transform.position;
        var raycastPlaneDestination = new Vector3(raycastPlanePos.x, raycastPlanePos.y, transform.position.z + 15f);
        raycastPlane.transform.position = Vector3.Lerp(raycastPlane.transform.position, raycastPlaneDestination, 0.1f);

        //Check if player missed a checkpoint
        var frontCheckpoint = spawnManager.ActiveCheckpointList[0];

        if (transform.position.z > frontCheckpoint.transform.position.z)
        {
            //Game ends
            EndGame();
        }
    }

    private Vector3 CalculatePlayerAngle(float mouseX, float mouseY)
    {
        roll -= mouseX * tiltAmount;
        roll = Mathf.Clamp(roll, -55f, 55f);
        pitch -= mouseY * tiltAmount;
        pitch = Mathf.Clamp(pitch, -35f, 35f);
        return new Vector3(pitch, 0f, roll);
    }

    private void EndGame()
    {
        rb.velocity = Vector3.zero;
        explosionParticle.transform.position = transform.position;
        explosionParticle.Play();
        var planeMesh = transform.Find("Plane");
        planeMesh.gameObject.SetActive(false);
        gameManager.GameOver();
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag(CheckpointTag))
        {
            checkpointParticle.transform.position = other.transform.position;
            checkpointParticle.Play();
            other.gameObject.SetActive(false);
            spawnManager.CheckpointSpawnCount--;
            spawnManager.ActiveCheckpointList.Remove(other.gameObject);
            gameManager.UpdateScore(1);
        }
    }
}
