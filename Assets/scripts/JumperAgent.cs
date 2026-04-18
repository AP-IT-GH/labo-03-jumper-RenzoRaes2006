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
        if (rb.linearVelocity.y < 0)
        {
            rb.linearVelocity += Vector3.up * Physics.gravity.y * (fallMultiplier - 1) * Time.deltaTime;
        }
    }

    public override void OnActionReceived(ActionBuffers actions)
    {
        AddReward(0.001f); // Survival bonus

        int jumpAction = actions.DiscreteActions[0];

        if (jumpAction == 1 && isGrounded)
        {
            rb.AddForce(Vector3.up * jumpForce, ForceMode.Impulse);
            isGrounded = false;
            
            if (!CheckIfObjectInSight())
            {
                AddReward(-0.05f);
            }
        }
    }

    private bool CheckIfObjectInSight()
    {
        var rayOutputs = RayPerceptionSensor.Perceive(raySensor.GetRayPerceptionInput(), false).RayOutputs;
        if (rayOutputs != null)
        {
            foreach (var ray in rayOutputs)
            {
                if (ray.HitGameObject != null && 
                   (ray.HitGameObject.CompareTag("Block") || ray.HitGameObject.CompareTag("Coin")))
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
            SetReward(-1.0f);
            EndEpisode();
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Coin"))
        {
            SetReward(1.0f);
            EndEpisode();
            Destroy(other.gameObject);
        }
    }
}