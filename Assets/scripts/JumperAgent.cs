using UnityEngine;
using Unity.MLAgents;
using Unity.MLAgents.Actuators;
using Unity.MLAgents.Sensors;

public class JumperAgent : Agent
{
    [Header("Settings")]
    public float jumpForce = 15f;
    public float fallMultiplier = 2.5f;
    
    [Header("References")]
    public SpawnManager spawner;
    public RayPerceptionSensorComponent3D raySensor;

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
        rb.linearVelocity = Vector3.zero;
        transform.localPosition = startPosition;
        isGrounded = true;

        if (spawner != null)
        {
            spawner.ClearObstacles();
        }
    }

    private void Update()
    {
        // Makes the jump snappy and fast
        if (rb.linearVelocity.y < 0)
        {
            rb.linearVelocity += Vector3.up * Physics.gravity.y * (fallMultiplier - 1) * Time.deltaTime;
        }
    }

    public override void OnActionReceived(ActionBuffers actions)
    {
        // 1. Tiny reward for surviving every step
        AddReward(0.001f);

        int jumpAction = actions.DiscreteActions[0];

        if (jumpAction == 1 && isGrounded)
        {
            rb.AddForce(Vector3.up * jumpForce, ForceMode.Impulse);
            isGrounded = false;
            
            // 2. Penalty for jumping when the ray sensor sees nothing
            if (!CheckIfBlockInSight())
            {
                AddReward(-0.05f); 
            }
        }
    }

    private bool CheckIfBlockInSight()
    {
        // Check if the sensor detects anything with the tag "Block"
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

        // 3. Real physical collision with the block
        if (collision.gameObject.CompareTag("Block"))
        {
            SetReward(-1.0f);
            EndEpisode();
        }
    }
}