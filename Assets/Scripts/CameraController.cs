using UnityEngine;

public class CameraController : MonoBehaviour
{
    public float speed = 2f;

    private Transform targetPosition;
    private Transform treasure;
    private bool moveToTarget = false;

    public void SetTarget(Transform target)
    {
        targetPosition = target;
    }

    public void FocusOnTreasure(Transform treasureTransform)
    {
        treasure = treasureTransform;
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