using UnityEngine;

public class TutorialManager : MonoBehaviour
{
    public static TutorialManager Instance { get; private set; }

    public enum TutorialStepType
    {
        PressEnter,
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
        Instance = this;
    }

    private void Start()
    {
        dialogueUI.gameObject.SetActive(true);
        ShowCurrentStep();
    }

    private void Update()
    {
        if (currentStepIndex >= steps.Length)
            return;

        var step = steps[currentStepIndex];

        if (step.stepType == TutorialStepType.PressEnter)
        {
            if (Input.GetKeyDown(KeyCode.Return) || Input.GetKeyDown(KeyCode.KeypadEnter))
                NextStep();
        }
        else if (step.stepType == TutorialStepType.WaitMove)
        {
            if (Mathf.Abs(Input.GetAxisRaw("Horizontal")) > 0.1f)
                NextStep();
        }
        else if (step.stepType == TutorialStepType.WaitJump)
        {
            if (Input.GetKeyDown(KeyCode.Space))
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
        dialogueUI.Show("Lumini", step.text, step.stepType == TutorialStepType.PressEnter);
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
            case TutorialStepType.PressEnter:
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
        if (WorldStateManager.Instance != null)
            WorldStateManager.Instance.OnStateChanged += HandleWorldStateChanged;
    }

    private void OnDisable()
    {
        if (WorldStateManager.Instance != null)
            WorldStateManager.Instance.OnStateChanged -= HandleWorldStateChanged;
    }

    private void HandleWorldStateChanged(WorldState state)
    {
        if (currentStepIndex < steps.Length &&
            steps[currentStepIndex].stepType == TutorialStepType.WaitSwitch)
        {
            NextStep();
        }
    }
}