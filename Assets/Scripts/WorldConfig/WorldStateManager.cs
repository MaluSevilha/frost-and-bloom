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

    public event System.Action<WorldState> OnStateChanged;

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

        OnStateChanged?.Invoke(CurrentState);
    }
}
