using UnityEngine;

public class CloudMover : MonoBehaviour
{
    [SerializeField] float forwardSpeed = 1.5f;
    [SerializeField] float driftSpeed = 0.15f;
    [SerializeField] float resetDistance = 60f;   // how far back to teleport
    [SerializeField] float tooCloseDistance = 5f; // trigger reset when this close to cam

    Camera cam;
    Vector3 drift;

    void Start()
    {
        cam = Camera.main;
        drift = new Vector3(Random.Range(-1f, 1f), Random.Range(-0.3f, 0.3f), 0f).normalized;
    }

    void Update()
    {
        // Billboard: face camera
        transform.rotation = Quaternion.LookRotation(cam.transform.forward, cam.transform.up);

        Vector3 camForward = cam.transform.forward;

        // Move toward camera + random lateral drift
        transform.position += camForward * forwardSpeed * Time.deltaTime;
        transform.position += drift * driftSpeed * Time.deltaTime;

        // How far ahead of camera along its view axis
        float depthFromCam = Vector3.Dot(transform.position - cam.transform.position, camForward);
        if (depthFromCam < tooCloseDistance)
            transform.position -= camForward * resetDistance;
    }
}
