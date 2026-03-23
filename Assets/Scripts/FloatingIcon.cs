using UnityEngine;

public class FloatingIcon : MonoBehaviour
{
    public float riseSpeed = 1.5f;
    public float duration = 0.8f;

    private SpriteRenderer spriteRenderer;
    private float elapsed = 0f;

    void Awake()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();
    }

    void Update()
    {
        elapsed += Time.deltaTime;
        float t = elapsed / duration;

        transform.position += Vector3.up * riseSpeed * Time.deltaTime;
        spriteRenderer.color = new Color(1f, 1f, 1f, 1f - t);

        if (elapsed >= duration)
            Destroy(gameObject);
    }
}
