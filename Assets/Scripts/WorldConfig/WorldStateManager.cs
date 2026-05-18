using UnityEngine;

public enum WorldState
{
    Frost,
    Bloom
}

public class WorldStateManager : MonoBehaviour
{
    public static WorldStateManager Instance { get; private set; }

    public WorldState CurrentState { get; private set; } = WorldState.Bloom;

    // Primeiro evento: ativa/desativa objetos (WorldGroup assina aqui)
    public event System.Action<WorldState> OnStateChanged;

    // Segundo evento: dispara APÓS os SetActive (EnemyWorldRespawn assina aqui)
    public event System.Action<WorldState> OnStateChangedLate;

    [Header("Tutorial Lock (opcional)")]
    [SerializeField] private bool respectTutorialLock = true;

    private PlayerControlFlags controlFlags;

    private void Awake()
    {
        Instance = this;
    }

    private void Start()
    {
        GameObject player = GameObject.FindGameObjectWithTag("Player");
        if (player != null)
        {
            controlFlags = player.GetComponent<PlayerControlFlags>();
        }
    }

    private void Update()
    {
        if (respectTutorialLock && controlFlags != null && !controlFlags.canSwitchState)
            return;

        if (Input.GetKeyDown(KeyCode.Q))
        {
            ToggleState();
        }
    }

    public void ToggleState()
    {
        CurrentState = CurrentState == WorldState.Frost
            ? WorldState.Bloom
            : WorldState.Frost;

        OnStateChanged?.Invoke(CurrentState);
        OnStateChangedLate?.Invoke(CurrentState);
    }
}