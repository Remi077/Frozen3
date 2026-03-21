using UnityEngine;

public class SeaScroller : MonoBehaviour
{
    public float speed = 10f;
    public float resetZ = -150f;
    public float startZ = 150f;

    public bool destroyOnReset = false; // ✅ NEW

    void Update()
    {
        // Move in world space
        transform.position += Vector3.back * speed * Time.deltaTime;

        if (transform.position.z <= resetZ)
        {
            if (destroyOnReset)
            {
                Destroy(gameObject); // ✅ destroy instead of looping
            }
            else
            {
                // Loop back to front
                transform.position = new Vector3(
                    transform.position.x,
                    transform.position.y,
                    startZ
                );
            }
        }
    }
}