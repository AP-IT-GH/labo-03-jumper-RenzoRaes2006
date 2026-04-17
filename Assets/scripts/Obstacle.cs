using UnityEngine;

public class Obstacle : MonoBehaviour
{
    public float speed;
    public JumperAgent agent;
    
    private Rigidbody rb;

    void Start()
    {
        // Haal de rigidbody van het blokje op
        rb = GetComponent<Rigidbody>();
    }

    // Gebruik FixedUpdate in plaats van Update voor ALLES wat met Physics te maken heeft
    void FixedUpdate()
    {
        if (rb != null)
        {
            // Bereken de nieuwe positie en verplaats hem met de physics engine
            Vector3 targetPosition = rb.position + Vector3.back * speed * Time.fixedDeltaTime;
            rb.MovePosition(targetPosition);
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        // Dit blijft OnTriggerEnter omdat de muur achter de agent (KillZone) wel een Trigger is
        if (other.CompareTag("KillZone"))
        {
            if (agent != null)
            {
                agent.SetReward(1.0f);
                agent.EndEpisode();
            }
            Destroy(gameObject);
        }
    }
}   