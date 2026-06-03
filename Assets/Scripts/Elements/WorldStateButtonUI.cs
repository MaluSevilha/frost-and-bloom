using UnityEngine;
using UnityEngine.UI;

public class WorldStateButtonUI : MonoBehaviour
{
    [SerializeField] private Image targetImage;
    [SerializeField] private Sprite frostSprite;
    [SerializeField] private Sprite bloomSprite;

    private WorldStateManager worldStateManager;

    private void Awake()
    {
        if (targetImage == null)
            targetImage = GetComponent<Image>();
    }

    private void OnEnable()
    {
        worldStateManager = FindFirstObjectByType<WorldStateManager>();

        if (worldStateManager != null)
        {
            worldStateManager.OnStateChanged += UpdateIcon;
            UpdateIcon(worldStateManager.CurrentState);
        }
    }

    private void OnDisable()
    {
        if (worldStateManager != null)
            worldStateManager.OnStateChanged -= UpdateIcon;
    }

    private void UpdateIcon(WorldState currentState)
    {
        if (targetImage == null) return;

        // mostra o estado oposto ao atual
        targetImage.sprite = currentState == WorldState.Frost
            ? bloomSprite
            : frostSprite;
    }
}