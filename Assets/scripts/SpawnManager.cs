using UnityEngine;

public class SpawnManager : MonoBehaviour
{
    public GameObject obstaclePrefab;
    public JumperAgent agent;

    public float minSpeed = 10f;
    public float maxSpeed = 15f;

    private GameObject currentObstacle;

    void Update()
    {
        if (currentObstacle == null)
        {
            Spawn();
        }
    }

    private void Spawn()
    {
        currentObstacle = Instantiate(obstaclePrefab, transform.position, Quaternion.identity, transform.parent);

        Obstacle obstacleScript = currentObstacle.GetComponent<Obstacle>();
        if (obstacleScript != null)
        {
            obstacleScript.speed = Random.Range(minSpeed, maxSpeed);
            obstacleScript.agent = agent;
        }
    }

    public void ClearObstacles()
    {
        if (currentObstacle != null)
        {
            Destroy(currentObstacle);
        }
    }
}