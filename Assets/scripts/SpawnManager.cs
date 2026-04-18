using UnityEngine;

public class SpawnManager : MonoBehaviour
{
[Header("Prefabs")]
    public GameObject blockPrefab;
    public GameObject coinPrefab;
    
    [Header("Settings")]
    public JumperAgent agent;
    public float minSpeed = 10f;
    public float maxSpeed = 15f;
    
    // Hoe hoog moet het muntje zweven? Pas dit aan in Unity als hij er niet bij kan!
    public float coinSpawnHeight = 2.5f; 

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
        bool spawnCoin = Random.value > 0.5f; 
        GameObject prefabToSpawn = spawnCoin ? coinPrefab : blockPrefab;

        Vector3 spawnPos = transform.position;
        if (spawnCoin)
        {
            spawnPos.y = coinSpawnHeight; 
        }

        currentObstacle = Instantiate(prefabToSpawn, spawnPos, Quaternion.identity, transform.parent);
        
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