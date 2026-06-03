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

    private void Update()
    {
        if (currentStepIndex >= steps.Length)
            return;

        TutorialStep step = steps[currentStepIndex];

        if (step.stepType == TutorialStepType.WaitMove)
        {
            float moveX = 0f;

            if (MobileInputState.Instance != null)
            {
                moveX = MobileInputState.Instance.PlayerMoveX;
            }
            else
            {
                if (Input.GetKeyDown(KeyCode.LeftArrow))
                    moveX = -1f;
                else if (Input.GetKeyDown(KeyCode.RightArrow))
                    moveX = 1f;
            }

            if (Mathf.Abs(moveX) > 0.01f)
                NextStep();
        }
    }

    public void ContinuePressed()
    {
        if (currentStepIndex >= steps.Length)
            return;

        TutorialStep step = steps[currentStepIndex];

        if (step.stepType == TutorialStepType.PressContinue)
        {
            NextStep();
        }
    }
    private void ShowCurrentStep()
    {
        if (currentStepIndex >= steps.Length)
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
        if (currentStepIndex < steps.Length &&
            steps[currentStepIndex].stepType == TutorialStepType.WaitJump)
        {
            NextStep();
        }
    }

    private void HandleWorldSwitch()
    {
        if (currentStepIndex < steps.Length &&
            steps[currentStepIndex].stepType == TutorialStepType.WaitSwitch)
        {
            NextStep();
        }
    }
}