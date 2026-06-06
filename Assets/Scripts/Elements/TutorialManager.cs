using UnityEngine;

public class TutorialManager : MonoBehaviour
{
    public static TutorialManager Instance { get; private set; }

    public enum TutorialStepType
    {
        PressContinue,
        WaitMove,
        WaitJump,
        WaitSwitch
    }

    [System.Serializable]
    public class TutorialStep
    {
        public string text;
        public TutorialStepType stepType;
    }

    [Header("UI")]
    [SerializeField] private DialogueUI dialogueUI;
    [SerializeField] private Sprite luminiPortrait;

    [Header("Player")]
    [SerializeField] private PlayerControlFlags controlFlags;

    [Header("Dialogue")]
    [SerializeField] private TutorialStep[] steps;

    private int currentStepIndex = 0;

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
        if (dialogueUI != null)
            dialogueUI.gameObject.SetActive(true);

        ShowCurrentStep();
    }

    private bool WasContinuePressed()
    {
        return Input.GetKeyDown(KeyCode.Return)
            || Input.GetKeyDown(KeyCode.KeypadEnter)
            || Input.GetKeyDown(KeyCode.Space)
            || Input.GetKeyDown(KeyCode.E);
    }

    private bool WasMovePressed()
    {
        bool keyboardMove =
            Input.GetKeyDown(KeyCode.LeftArrow) ||
            Input.GetKeyDown(KeyCode.RightArrow) ||
            Input.GetKeyDown(KeyCode.A) ||
            Input.GetKeyDown(KeyCode.D);

        bool mobileMove =
            MobileInputState.Instance != null &&
            Mathf.Abs(MobileInputState.Instance.PlayerMoveX) > 0.01f;

        return keyboardMove || mobileMove;
    }

    private void Update()
    {
        if (steps == null || currentStepIndex >= steps.Length)
            return;

        TutorialStep step = steps[currentStepIndex];

        switch (step.stepType)
        {
            case TutorialStepType.PressContinue:
                if (WasContinuePressed())
                    NextStep();
                break;

            case TutorialStepType.WaitMove:
                if (WasMovePressed())
                    NextStep();
                break;
        }
    }

    public void ContinuePressed()
    {
        if (steps == null || currentStepIndex >= steps.Length)
            return;

        TutorialStep step = steps[currentStepIndex];

        if (step.stepType == TutorialStepType.PressContinue)
        {
            NextStep();
        }
    }

    private void ShowCurrentStep()
    {
        if (steps == null || currentStepIndex >= steps.Length)
        {
            EndTutorial();
            return;
        }

        TutorialStep step = steps[currentStepIndex];

        SetPermissions(step.stepType);

        bool showContinueHint = step.stepType == TutorialStepType.PressContinue;

        if (dialogueUI != null)
            dialogueUI.Show("Lumini", step.text, showContinueHint);
    }

    private void SetPermissions(TutorialStepType type)
    {
        if (controlFlags == null) return;

        controlFlags.canMove = false;
        controlFlags.canJump = false;
        controlFlags.canSwitchState = false;
        controlFlags.canInteract = false;

        switch (type)
        {
            case TutorialStepType.PressContinue:
                break;

            case TutorialStepType.WaitMove:
                controlFlags.canMove = true;
                break;

            case TutorialStepType.WaitJump:
                controlFlags.canMove = true;
                controlFlags.canJump = true;
                break;

            case TutorialStepType.WaitSwitch:
                controlFlags.canMove = true;
                controlFlags.canJump = true;
                controlFlags.canSwitchState = true;
                break;
        }
    }

    private void NextStep()
    {
        currentStepIndex++;
        ShowCurrentStep();
    }

    private void EndTutorial()
    {
        if (dialogueUI != null)
            dialogueUI.Hide();

        if (controlFlags != null)
        {
            controlFlags.canMove = true;
            controlFlags.canJump = true;
            controlFlags.canSwitchState = true;
            controlFlags.canInteract = true;
        }
    }

    private void OnEnable()
    {
        PlayerMovement.OnPlayerJump += HandlePlayerJump;

        if (WorldStateManager.Instance != null)
            WorldStateManager.Instance.OnWorldSwitched += HandleWorldSwitch;
    }

    private void OnDisable()
    {
        PlayerMovement.OnPlayerJump -= HandlePlayerJump;

        if (WorldStateManager.Instance != null)
            WorldStateManager.Instance.OnWorldSwitched -= HandleWorldSwitch;
    }

    private void HandlePlayerJump()
    {
        if (steps == null || currentStepIndex >= steps.Length)
            return;

        if (steps[currentStepIndex].stepType == TutorialStepType.WaitJump)
        {
            NextStep();
        }
    }

    private void HandleWorldSwitch()
    {
        if (steps == null || currentStepIndex >= steps.Length)
            return;

        if (steps[currentStepIndex].stepType == TutorialStepType.WaitSwitch)
        {
            NextStep();
        }
    }
}