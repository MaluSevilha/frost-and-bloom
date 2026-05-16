using UnityEngine;

public class EnemyWorldRespawn : MonoBehaviour
{
    [SerializeField] private WorldState enemyWorld;
    [SerializeField] private Transform spawnPoint;
    [SerializeField] private EnemyAI enemyAI;
    [SerializeField] private EnemyPlatformFollow platformFollow;
    [SerializeField] private Rigidbody2D rb;

    private void Awake()
    {
        if (rb == null) rb = GetComponent<Rigidbody2D>();
        if (enemyAI == null) enemyAI = GetComponent<EnemyAI>();
        if (platformFollow == null) platformFollow = GetComponent<EnemyPlatformFollow>();
    }

    private void Start()
    {
        if (WorldStateManager.Instance != null)
            WorldStateManager.Instance.OnStateChangedLate += OnWorldChanged;

        // Posiciona no spawnPoint desde o início,
        // independente de onde o inimigo foi colocado no Editor.
        Respawn();
    }

    private void OnDestroy()
    {
        if (WorldStateManager.Instance != null)
            WorldStateManager.Instance.OnStateChangedLate -= OnWorldChanged;
    }

    private void OnWorldChanged(WorldState newState)
    {
        if (newState != enemyWorld)
            return;

        Respawn();
    }

    private void Respawn()
    {
        if (spawnPoint == null)
        {
            Debug.LogWarning("SpawnPoint não atribuído em EnemyWorldRespawn.");
            return;
        }

        rb.linearVelocity = Vector2.zero;
        rb.angularVelocity = 0f;
        rb.position = spawnPoint.position;
        Physics2D.SyncTransforms();

        if (platformFollow != null)
            platformFollow.ResetFollow();

        if (enemyAI != null)
        {
            enemyAI.RecalcularPontoDeRetorno(spawnPoint);
            enemyAI.ResetEnemyState();
        }
    }
}