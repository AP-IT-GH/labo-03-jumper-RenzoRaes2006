using UnityEngine;

public class Obstacle : MonoBehaviour
{
    public float speed;
    public JumperAgent agent;
    
    private Rigidbody rb;

    void Start()
    {
        rb = GetComponent<Rigidbody>();
    }

    void FixedUpdate()
    {
        if (rb != null)
        {
            Vector3 targetPosition = rb.position + Vector3.back * speed * Time.fixedDeltaTime;
            rb.MovePosition(targetPosition);
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("KillZone"))
        {
            if (agent != null)
            {
                if (gameObject.CompareTag("Block"))
                {
                    agent.SetReward(1.0f);
                    agent.EndEpisode();
                }
                else if (gameObject.CompareTag("Coin"))
                {
                    agent.SetReward(-1.0f);
                    agent.EndEpisode();
                }
            }
            Destroy(gameObject);
        }
    }
}