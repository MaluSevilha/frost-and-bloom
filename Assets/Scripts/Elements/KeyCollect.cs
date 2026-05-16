using UnityEngine;

public class KeyCollect : MonoBehaviour
{
    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            PlayerInventory inventory = other.GetComponent<PlayerInventory>();

            if (inventory != null)
            {
                inventory.hasKey = true;
            }

            gameObject.SetActive(false);
        }
    }
}