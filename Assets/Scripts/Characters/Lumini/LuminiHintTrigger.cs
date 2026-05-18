using System.Collections;
using UnityEngine;

[RequireComponent(typeof(Collider2D))]
public class LuminiHintTrigger : MonoBehaviour
{
    [Header("UI")]
    [SerializeField] private DialogueUI dialogueUI;

    [Header("Hint")]
    [TextArea(2, 4)]
    [SerializeField] private string hintText = "Texto da dica aqui.";
    [SerializeField] private string speakerName = "Lumini";

    [Header("Behavior")]
    [SerializeField] private bool triggerOnce = true;
    [SerializeField] private bool usePlayerTag = true;
    [SerializeField] private bool autoHide = true;
    [SerializeField] private float visibleTime = 3.5f;

    private bool hasTriggered;
    private Coroutine routine;

    private void Reset()
    {
        Collider2D col = GetComponent<Collider2D>();
        col.isTrigger = true;
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (usePlayerTag && !other.CompareTag("Player"))
            return;

        PlayHint();
    }

    public void PlayHint()
    {
        if (hasTriggered && triggerOnce)
            return;

        hasTriggered = true;

        if (routine != null)
            StopCoroutine(routine);

        routine = StartCoroutine(PlayRoutine());
    }

    private IEnumerator PlayRoutine()
    {
        if (dialogueUI != null)
        {
            dialogueUI.gameObject.SetActive(true);
            dialogueUI.Show(speakerName, hintText, false);
        }

        if (autoHide)
        {
            yield return new WaitForSecondsRealtime(visibleTime);

            if (dialogueUI != null)
                dialogueUI.Hide();
        }
    }

    public void ResetTrigger()
    {
        hasTriggered = false;
    }
}