using UnityEngine;

public class CameraController : MonoBehaviour
{
    public Transform targetPosition;
    public Transform treasure;
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

            Vector3 dirToTreasure = treasure.position - transform.position;
            Quaternion targetRotation = Quaternion.LookRotation(dirToTreasure);
            transform.rotation = Quaternion.Lerp(transform.rotation, targetRotation, Time.deltaTime * speed);
        }
    }
}