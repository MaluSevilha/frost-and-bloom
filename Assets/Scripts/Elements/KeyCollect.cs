using UnityEngine;

public class KeyCollect : MonoBehaviour
{
    public Animator doorAnimator;

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            doorAnimator.SetTrigger("Open");

            gameObject.SetActive(false);
        }
    }
}