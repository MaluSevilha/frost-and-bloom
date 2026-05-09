using UnityEngine;

public class WorldGroup : MonoBehaviour
{
    [SerializeField] private WorldState activeInState;

    private void Start()
    {
        if (WorldStateManager.Instance != null)
        {
            WorldStateManager.Instance.OnStateChanged += Refresh;
            Refresh(WorldStateManager.Instance.CurrentState);
        }
    }

    private void OnDestroy()
    {
        if (WorldStateManager.Instance != null)
            WorldStateManager.Instance.OnStateChanged -= Refresh;
    }

    private void Refresh(WorldState state)
    {
        gameObject.SetActive(state == activeInState);
    }
}