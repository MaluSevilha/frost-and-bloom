using UnityEngine;
using UnityEngine.UI;

public class LuminiAnim : MonoBehaviour
{
    [SerializeField] private Image portraitImage;
    [SerializeField] private Sprite[] frames;
    [SerializeField] private float frameRate = 0.12f;

    private int currentFrame;
    private float timer;

    private void Awake()
    {
        if (portraitImage == null)
            portraitImage = GetComponent<Image>();
    }

    private void Update()
    {
        if (frames == null || frames.Length == 0 || portraitImage == null)
            return;

        timer += Time.unscaledDeltaTime;

        if (timer >= frameRate)
        {
            timer = 0f;
            currentFrame = (currentFrame + 1) % frames.Length;
            portraitImage.overrideSprite = frames[currentFrame];
        }
    }
}