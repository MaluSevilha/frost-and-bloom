using UnityEngine;

public class Door : MonoBehaviour
{
    public Sprite closedSprite;
    public Sprite openSprite;
    public SpriteRenderer spriteRenderer;

    public CapsuleCollider2D blockingCollider;   // collider sólido
    public CapsuleCollider2D interactionCollider; // trigger

    private bool isOpen = false;
    private bool playerInRange = false;
    private PlayerInventory playerInventory;

    private void Start()
    {
        if (spriteRenderer == null)
            spriteRenderer = GetComponent<SpriteRenderer>();

        spriteRenderer.sprite = closedSprite;

        if (blockingCollider != null)
            blockingCollider.enabled = true;

        if (interactionCollider != null)
            interactionCollider.isTrigger = true;
    }

    private void Update()
    {
        if (playerInRange && !isOpen && Input.GetKeyDown(KeyCode.E))
        {
            if (playerInventory != null && playerInventory.hasKey)
            {
                OpenDoor();
            }
            else
            {
                Debug.Log("Você precisa da chave!");
            }
        }
    }

    private void OpenDoor()
    {
        isOpen = true;
        spriteRenderer.sprite = openSprite;

        if (blockingCollider != null)
            blockingCollider.enabled = false;
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            playerInRange = true;
            playerInventory = other.GetComponent<PlayerInventory>();

            if (playerInventory == null)
                playerInventory = other.GetComponentInParent<PlayerInventory>();
        }
    }

    private void OnTriggerExit2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            playerInRange = false;
            playerInventory = null;
        }
    }
}