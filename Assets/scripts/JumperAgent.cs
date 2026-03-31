using UnityEngine;
using Unity.MLAgents;
using Unity.MLAgents.Actuators;
using Unity.MLAgents.Sensors;

public class JumperAgent : Agent
{
    [SerializeField] private float jumpForce = 10f;
    [SerializeField] private SpawnManager spawnManager;
    [SerializeField] private RayPerceptionSensorComponent3D raySensor;

    private Rigidbody rb;
    private bool isGrounded;
    private Vector3 startPosition;

    public override void Initialize()
    {
        rb = GetComponent<Rigidbody>();
        startPosition = transform.localPosition;
        if (raySensor == null) raySensor = GetComponent<RayPerceptionSensorComponent3D>();
    }

    public override void OnEpisodeBegin()
    {
        // Reset fysica en positie
        rb.linearVelocity = Vector3.zero;
        transform.localPosition = startPosition;

        // Reset de spawner (verwijder oude blokken)
        if (spawnManager != null)
        {
            spawnManager.ResetSpawner();
        }
    }

    public override void OnActionReceived(ActionBuffers actions)
    {
        bool isJumping = actions.DiscreteActions[0] == 1;

        if (isJumping)
        {
            if (isGrounded)
            {
                rb.AddForce(Vector3.up * jumpForce, ForceMode.Impulse);
                isGrounded = false;
                // Kleine aanmoediging om de actie 'springen' te ontdekken
                AddReward(-0.05f); // Een kleine "kost" voor energieverbruik            }
            }
            else // De AI kiest om NIET te springen
            {
                if (CheckIfBlockInSight())
                {
                    AddReward(-0.01f);
                }
                else
                {
                    AddReward(0.01f);
                }
            }
        }
    }
    private bool CheckIfBlockInSight()
    {
        // Gebruik de sensor om te kijken of er een "Block" aankomt
        var rayOutputs = RayPerceptionSensor.Perceive(raySensor.GetRayPerceptionInput(), false).RayOutputs;

        if (rayOutputs != null)
        {
            foreach (var ray in rayOutputs)
            {
                if (ray.HitGameObject != null && ray.HitGameObject.CompareTag("Block"))
                {
                    return true;
                }
            }
        }
        return false;
    }

    public override void Heuristic(in ActionBuffers actionsOut)
    {
        var discreteActions = actionsOut.DiscreteActions;
        discreteActions[0] = Input.GetKey(KeyCode.Space) ? 1 : 0;
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.CompareTag("Ground"))
        {
            isGrounded = true;
        }

        if (collision.gameObject.CompareTag("Block"))
        {
            // Grote straf bij raken en stop de episode
            SetReward(-1.0f);
            EndEpisode();
        }
    }
}