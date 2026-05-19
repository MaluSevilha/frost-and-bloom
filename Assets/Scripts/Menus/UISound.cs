using UnityEngine;
using UnityEngine.EventSystems;

public class UIButtonSound : MonoBehaviour, IPointerEnterHandler, IPointerClickHandler
{
    public AudioSource source;
    public AudioClip hoverSound;
    public AudioClip clickSound;

    public void OnPointerEnter(PointerEventData eventData)
    {
        source.PlayOneShot(hoverSound);
    }

    public void OnPointerClick(PointerEventData eventData)
    {
        source.PlayOneShot(clickSound);
    }
}