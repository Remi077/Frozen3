using UnityEngine;

public class RotateCube : MonoBehaviour
{
    public float speed = 50f;

    void Update()
    {
        // ✅ Rotate around WORLD Y axis
        transform.Rotate(Vector3.up * speed * Time.deltaTime, Space.World);
    }
}