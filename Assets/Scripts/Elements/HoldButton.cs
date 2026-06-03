using UnityEngine;
using UnityEngine.EventSystems;

public class HoldButton : MonoBehaviour, IPointerDownHandler, IPointerUpHandler, IPointerExitHandler
{
    public enum HoldType
    {
        PlayerLeft,
        PlayerRight,
        PlatformLeft,
        PlatformRight,
        PlatformUp,
        PlatformDown
    }

    [SerializeField] private HoldType holdType;

    public void OnPointerDown(PointerEventData eventData)
    {
        if (MobileInputState.Instance == null) return;

        switch (holdType)
        {
            case HoldType.PlayerLeft:
                MobileInputState.Instance.SetPlayerMoveX(-1f);
                break;
            case HoldType.PlayerRight:
                MobileInputState.Instance.SetPlayerMoveX(1f);
                break;
            case HoldType.PlatformLeft:
                MobileInputState.Instance.SetPlatformX(-1f);
                break;
            case HoldType.PlatformRight:
                MobileInputState.Instance.SetPlatformX(1f);
                break;
            case HoldType.PlatformUp:
                MobileInputState.Instance.SetPlatformY(1f);
                break;
            case HoldType.PlatformDown:
                MobileInputState.Instance.SetPlatformY(-1f);
                break;
        }
    }

    public void OnPointerUp(PointerEventData eventData)
    {
        ResetValue();
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        ResetValue();
    }

    private void ResetValue()
    {
        if (MobileInputState.Instance == null) return;

        switch (holdType)
        {
            case HoldType.PlayerLeft:
            case HoldType.PlayerRight:
                MobileInputState.Instance.SetPlayerMoveX(0f);
                break;

            case HoldType.PlatformLeft:
            case HoldType.PlatformRight:
                MobileInputState.Instance.SetPlatformX(0f);
                break;

            case HoldType.PlatformUp:
            case HoldType.PlatformDown:
                MobileInputState.Instance.SetPlatformY(0f);
                break;
        }
    }
}