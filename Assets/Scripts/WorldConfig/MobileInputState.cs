using UnityEngine;

public class MobileInputState : MonoBehaviour
{
    public static MobileInputState Instance { get; private set; }

    public float PlayerMoveX { get; private set; }
    public float PlatformX { get; private set; }
    public float PlatformY { get; private set; }

    private bool jumpQueued;
    private bool switchQueued;
    private bool interactQueued;

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;
    }

    private void OnDestroy()
    {
        if (Instance == this)
            Instance = null;
    }

    public void SetPlayerMoveX(float value)
    {
        PlayerMoveX = Mathf.Clamp(value, -1f, 1f);
    }

    public void SetPlatformX(float value)
    {
        PlatformX = Mathf.Clamp(value, -1f, 1f);
    }

    public void SetPlatformY(float value)
    {
        PlatformY = Mathf.Clamp(value, -1f, 1f);
    }

    public void PressJump()
    {
        jumpQueued = true;
    }

    public void PressSwitch()
    {
        switchQueued = true;
    }

    public void PressInteract()
    {
        interactQueued = true;
    }

    public bool ConsumeJump()
    {
        bool value = jumpQueued;
        jumpQueued = false;
        return value;
    }

    public bool ConsumeSwitch()
    {
        bool value = switchQueued;
        switchQueued = false;
        return value;
    }

    public bool ConsumeInteract()
    {
        bool value = interactQueued;
        interactQueued = false;
        return value;
    }
}