using UnityEngine;

public class KeyCollect : MonoBehaviour
{
    private AudioSource audioSource;
    private SpriteRenderer spriteRenderer;
    private Collider2D col;

    void Start()
    {
        audioSource = GetComponent<AudioSource>();
        spriteRenderer = GetComponent<SpriteRenderer>();
        col = GetComponent<Collider2D>();
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            PlayerInventory inventory = other.GetComponent<PlayerInventory>();

            if (inventory != null)
            {
                inventory.hasKey = true;
            }

            // toca o som
            audioSource.Play();

            // esconde a chave
            spriteRenderer.enabled = false;
            col.enabled = false;

            // destrói depois do som tocar
            Destroy(gameObject, 1f);
        }
    }
}