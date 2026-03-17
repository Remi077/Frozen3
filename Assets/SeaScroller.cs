using UnityEngine;

public class SeaScroller : MonoBehaviour
{
    public float speed = 5f;       // Speed of sea movement
    public float resetZ = -50f;    // Where the plane resets
    public float startZ = 50f;     // Starting Z position

    void Update()
    {
        // Move plane towards negative Z
        transform.Translate(Vector3.back * speed * Time.deltaTime);

        // Reset plane to front
        if (transform.position.z <= resetZ)
        {
            Vector3 pos = transform.position;
            pos.z = startZ;
            transform.position = pos;
        }
    }
}