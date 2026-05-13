using UnityEngine;
using UnityEngine.UI;

public class HUDState : MonoBehaviour
{
    [SerializeField] private Image frostImage;
    [SerializeField] private Image bloomImage;

    private Color activeColor = Color.white;
    private Color inactiveColor = new Color(0.4f, 0.4f, 0.4f, 0.6f);

    private void Start()
    {
        UpdateHUD(WorldStateManager.Instance.CurrentState);
        WorldStateManager.Instance.OnStateChanged += UpdateHUD;
    }

    private void OnDestroy()
    {
        if (WorldStateManager.Instance != null)
            WorldStateManager.Instance.OnStateChanged -= UpdateHUD;
    }

    private void UpdateHUD(WorldState state)
    {
        if (state == WorldState.Frost)
        {
            frostImage.color = activeColor;
            bloomImage.color = inactiveColor;
        }
        else
        {
            frostImage.color = inactiveColor;
            bloomImage.color = activeColor;
        }
    }
}   