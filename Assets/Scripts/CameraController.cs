using UnityEngine;

public class CameraController : MonoBehaviour
{
    public Transform targetPosition;
    public float speed = 2f;

    private bool moveToTarget = false;

    public void FocusOnTreasure()
    {
        moveToTarget = true;
    }

    void Update()
    {
        if (moveToTarget)
        {
            transform.position = Vector3.Lerp(
                transform.position,
                targetPosition.position,
                Time.deltaTime * speed
            );

            transform.rotation = Quaternion.Lerp(
                transform.rotation,
                targetPosition.rotation,
                Time.deltaTime * speed
            );
        }
    }
}