using UnityEngine;

public class Coin : MonoBehaviour
{
    public int value = 1;

    private bool collected = false;

    private void OnTriggerEnter(Collider other)
    {
        if (collected) return;

        if (other.CompareTag("Player"))
        {
            collected = true;

            ScoreManager.Instance.AddScore(value);
            Destroy(gameObject);
        }
    }
}