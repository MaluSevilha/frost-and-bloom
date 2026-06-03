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
    public event System.Action OnWorldSwitched;

    // Segundo evento: dispara APÓS os SetActive (EnemyWorldRespawn assina aqui)
    public event System.Action<WorldState> OnStateChangedLate;

    private PlayerControlFlags controlFlags;

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }

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

    private void OnDestroy()
    {
        if (Instance == this)
            Instance = null;

        OnStateChanged = null;
        OnStateChangedLate = null;
        OnWorldSwitched = null;
    }

    public void ToggleState()
    {
        CurrentState = CurrentState == WorldState.Frost
            ? WorldState.Bloom
            : WorldState.Frost;

        OnStateChanged?.Invoke(CurrentState);
        OnStateChangedLate?.Invoke(CurrentState);

        OnWorldSwitched?.Invoke();
    }
}