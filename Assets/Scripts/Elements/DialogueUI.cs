using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class DialogueUI : MonoBehaviour
{
    [Header("UI Refs")]
    [SerializeField] private CanvasGroup canvasGroup;
    [SerializeField] private RectTransform portraitRect;
    [SerializeField] private Image portraitImage;
    [SerializeField] private TMP_Text speakerNameText;
    [SerializeField] private TMP_Text dialogueText;
    [SerializeField] private GameObject enterHint;

    [Header("Fade")]
    [SerializeField] private float fadeDuration = 0.2f;

    [Header("Typewriter")]
    [SerializeField] private float typeSpeed = 0.03f;

    [Header("Portrait Bounce")]
    [SerializeField] private float bounceAmount = 8f;
    [SerializeField] private float bounceDuration = 0.12f;

    [Header("Audio")]
    [SerializeField] private AudioSource audioSource;
    [SerializeField] private AudioClip talkBlip;
    [SerializeField] private float blipInterval = 3f;

    private Coroutine fadeRoutine;
    private Coroutine typeRoutine;
    private Coroutine bounceRoutine;
    private bool isVisible;

    private void Awake()
    {
        if (canvasGroup == null)
            canvasGroup = GetComponent<CanvasGroup>();

        HideImmediate();
    }

    public void Show(string speakerName, string line, bool showEnterHint = false)
    {
        if (speakerNameText != null)
            speakerNameText.text = speakerName;

        if (enterHint != null)
            enterHint.SetActive(showEnterHint);

        if (typeRoutine != null)
            StopCoroutine(typeRoutine);

        if (bounceRoutine != null)
            StopCoroutine(bounceRoutine);

        bounceRoutine = StartCoroutine(BouncePortrait());

        typeRoutine = StartCoroutine(TypeLine(line));

        if (!isVisible)
        {
            if (fadeRoutine != null)
                StopCoroutine(fadeRoutine);

            fadeRoutine = StartCoroutine(FadeTo(1f));
        }

        isVisible = true;
    }

    public void Hide()
    {
        if (typeRoutine != null)
            StopCoroutine(typeRoutine);

        if (bounceRoutine != null)
            StopCoroutine(bounceRoutine);

        if (fadeRoutine != null)
            StopCoroutine(fadeRoutine);

        fadeRoutine = StartCoroutine(FadeTo(0f));
        isVisible = false;
    }

    public void HideImmediate()
    {
        if (canvasGroup != null)
        {
            canvasGroup.alpha = 0f;
            canvasGroup.interactable = false;
            canvasGroup.blocksRaycasts = false;
        }

        isVisible = false;
    }

    private IEnumerator FadeTo(float targetAlpha)
    {
        float startAlpha = canvasGroup.alpha;
        float elapsed = 0f;

        canvasGroup.interactable = targetAlpha > 0f;
        canvasGroup.blocksRaycasts = targetAlpha > 0f;

        while (elapsed < fadeDuration)
        {
            elapsed += Time.unscaledDeltaTime;
            float t = elapsed / fadeDuration;
            canvasGroup.alpha = Mathf.Lerp(startAlpha, targetAlpha, t);
            yield return null;
        }

        canvasGroup.alpha = targetAlpha;
    }

    private IEnumerator TypeLine(string line)
    {
        if (dialogueText == null)
            yield break;

        dialogueText.text = "";

        float blipTimer = 0f;

        foreach (char c in line)
        {
            dialogueText.text += c;

            blipTimer += 1f;
            if (audioSource != null && talkBlip != null && blipTimer >= blipInterval && !char.IsWhiteSpace(c))
            {
                audioSource.PlayOneShot(talkBlip);
                blipTimer = 0f;
            }

            yield return new WaitForSecondsRealtime(typeSpeed);
        }
    }

    private IEnumerator BouncePortrait()
    {
        if (portraitRect == null)
            yield break;

        Vector2 startPos = portraitRect.anchoredPosition;
        Vector2 peakPos = startPos + Vector2.up * bounceAmount;

        float t = 0f;
        while (t < bounceDuration)
        {
            t += Time.unscaledDeltaTime;
            portraitRect.anchoredPosition = Vector2.Lerp(startPos, peakPos, t / bounceDuration);
            yield return null;
        }

        t = 0f;
        while (t < bounceDuration)
        {
            t += Time.unscaledDeltaTime;
            portraitRect.anchoredPosition = Vector2.Lerp(peakPos, startPos, t / bounceDuration);
            yield return null;
        }

        portraitRect.anchoredPosition = startPos;
    }
}