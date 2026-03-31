using UnityEngine;

public class SpawnManager : MonoBehaviour
{
    [Header("Settings")]
    [SerializeField] private GameObject blockPrefab;
    [SerializeField] private Transform spawnPoint;
    
    [Header("Speed Settings")]
    [SerializeField] private float minSpeed = 5f;
    [SerializeField] private float maxSpeed = 10f;
    
    private GameObject currentBlock;

    void Update()
    {
        // Als er geen blok is (omdat hij destroyed is door KillZone of Reset)
        if (currentBlock == null)
        {
            SpawnBlock();
        }
    }

    void SpawnBlock()
    {
        if (blockPrefab == null || spawnPoint == null) return;

        currentBlock = Instantiate(blockPrefab, spawnPoint.position, Quaternion.identity, transform);

        float randomSpeed = Random.Range(minSpeed, maxSpeed);
        BlockMovement moveScript = currentBlock.GetComponent<BlockMovement>();
        
        if (moveScript != null)
        {
            moveScript.SetSpeed(randomSpeed);
        }
    }

    public void ResetSpawner()
    {
        if (currentBlock != null) 
        {
            Destroy(currentBlock);
        }
    }
}