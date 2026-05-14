using UnityEngine;

public class EnemyWorldRespawn : MonoBehaviour
{
    [Header("Referências")]
    [SerializeField] private Transform spawnPoint;
    [SerializeField] private Rigidbody2D rb;
    [SerializeField] private EnemyAI enemyAI;

    private void Awake()
    {
        if (rb == null)
            rb = GetComponent<Rigidbody2D>();

        if (enemyAI == null)
            enemyAI = GetComponent<EnemyAI>();
    }

    public void RespawnToSpawnPoint()
    {
        if (spawnPoint == null)
        {
            Debug.LogWarning("SpawnPoint não atribuído.");
            return;
        }

        transform.position = spawnPoint.position;

        if (rb != null)
        {
            rb.linearVelocity = Vector2.zero;
            rb.angularVelocity = 0f;
            rb.position = spawnPoint.position;
        }

        if (enemyAI != null)
            enemyAI.ResetEnemyState();
    }
}