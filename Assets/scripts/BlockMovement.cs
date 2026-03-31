using UnityEngine;

public class BlockMovement : MonoBehaviour
{
    private float speed;
    private JumperAgent agent;

    public void SetSpeed(float newSpeed)
    {
        speed = newSpeed;
        // Zoek de agent in de TrainingArea (parent)
        agent = transform.parent.GetComponentInChildren<JumperAgent>();
    }

    void Update()
    {
        transform.Translate(Vector3.right * speed * Time.deltaTime);
    }

    private void OnTriggerEnter(Collider other)
    {
        // Beloon de agent als het blok gepasseerd is
        if (other.CompareTag("KillZone"))
        {
            if (agent != null)
            {
                agent.AddReward(2.0f); // De hoofdprijs voor ontwijken
            }
            Destroy(gameObject); // Dit triggert de SpawnManager om een nieuwe te maken
        }
    }
}