using UnityEngine;

public class Rock : MonoBehaviour
{
    public int value = 1;

    private bool hit = false;

    private void OnTriggerEnter(Collider other)
    {
        if (hit) return;

        if (other.CompareTag("Player"))
        {
            hit = true;

            LiveManager.Instance.removeLife(value);
            // Destroy(gameObject);
        }
    }
}