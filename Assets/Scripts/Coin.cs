using UnityEngine;

public class Coin : MonoBehaviour
{
    public int value = 1;
    public GameObject floatingIconPrefab;

    private bool collected = false;

    private void OnTriggerEnter(Collider other)
    {
        if (collected) return;

        if (other.CompareTag("Player"))
        {
            collected = true;

            if (floatingIconPrefab != null)
                Instantiate(floatingIconPrefab, transform.position, Quaternion.identity);

            ScoreManager.Instance.AddScore(value);
            Destroy(gameObject);
        }
    }
}