using UnityEngine;
using UnityEngine.EventSystems;

public class TapButton : MonoBehaviour, IPointerDownHandler
{
    public enum TapType
    {
        Jump,
        SwitchWorld,
        Interact
    }

    [SerializeField] private TapType tapType;

    public void OnPointerDown(PointerEventData eventData)
    {
        #if UNITY_ANDROID || UNITY_IOS
            Handheld.Vibrate();
        #endif
        
        if (MobileInputState.Instance == null) return;

        switch (tapType)
        {
            case TapType.Jump:
                MobileInputState.Instance.PressJump();
                break;

            case TapType.SwitchWorld:
                MobileInputState.Instance.PressSwitch();
                break;

            case TapType.Interact:
                MobileInputState.Instance.PressInteract();
                break;
        }
    }
}