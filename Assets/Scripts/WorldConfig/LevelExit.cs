using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections;

public class LevelExit : MonoBehaviour
{
    public string nextSceneName;

    private AudioSource audioSource;
    private bool finished = false;

    private void Start()
    {
        audioSource = GetComponent<AudioSource>();
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player") && !finished)
        {
            finished = true;

            // congela a player
            Rigidbody2D rb = other.GetComponent<Rigidbody2D>();

            if (rb != null)
            {
                rb.linearVelocity = Vector2.zero;
                rb.bodyType = RigidbodyType2D.Static;
            }

            // toca o som
            audioSource.Play();

            // espera terminar
            StartCoroutine(GoToNextScene());
        }
    }

    IEnumerator GoToNextScene()
    {
        yield return new WaitForSeconds(audioSource.clip.length);

        SceneManager.LoadScene(nextSceneName);
    }
}