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

    private void Awake()
    {
        Instance = this;
    }

    private void Update()
    {
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

        // 1º: ativa/desativa GameObjects
        OnStateChanged?.Invoke(CurrentState);

        // 2º: agora os objetos já estão ativos, respawn pode rodar
        OnStateChangedLate?.Invoke(CurrentState);
    }
}