using UnityEngine;

public class BoatAnimator : MonoBehaviour
{
    [Header("Tilt")]
    public float maxTiltAngle = 20f;
    public float tiltSpeed = 5f;

    [Header("Bob")]
    public float bobAmplitude = 0.1f;
    public float bobFrequency = 0.8f;

    private SwipeLaneMovement swipe;
    private float initialY;

    void Awake()
    {
        swipe = GetComponentInParent<SwipeLaneMovement>();
        initialY = transform.localPosition.y;
    }

    void Update()
    {
        // Bob
        float bob = initialY + Mathf.Sin(Time.time * bobFrequency * Mathf.PI * 2f) * bobAmplitude;
        transform.localPosition = new Vector3(transform.localPosition.x, bob, transform.localPosition.z);

        // Tilt
        swipe.targetTilt = Mathf.Lerp(swipe.targetTilt, 0f, tiltSpeed * Time.deltaTime);
        float currentTilt = transform.localEulerAngles.x > 180f ? transform.localEulerAngles.x - 360f : transform.localEulerAngles.x;
        transform.localEulerAngles = new Vector3(Mathf.Lerp(currentTilt, swipe.targetTilt, tiltSpeed * Time.deltaTime), transform.localEulerAngles.y, transform.localEulerAngles.z);
    }
}
