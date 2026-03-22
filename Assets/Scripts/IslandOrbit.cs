using UnityEngine;
using UnityEngine.InputSystem;

public class IslandOrbit : MonoBehaviour
{
    public float sensitivity = 0.3f;

    private Vector2 lastTouchPos;
    private bool isTouching = false;

    void Update()
    {
        // Touch (smartphone)
        if (Touchscreen.current != null)
        {
            var touch = Touchscreen.current.primaryTouch;

            if (touch.press.wasPressedThisFrame)
            {
                lastTouchPos = touch.position.ReadValue();
                isTouching = true;
            }

            if (touch.press.wasReleasedThisFrame)
                isTouching = false;

            if (isTouching && touch.press.isPressed)
            {
                Vector2 currentPos = touch.position.ReadValue();
                float dx = currentPos.x - lastTouchPos.x;
                transform.Rotate(Vector3.up, -dx * sensitivity, Space.World);
                lastTouchPos = currentPos;
            }
        }

        // Editor testing with mouse
#if UNITY_EDITOR
        var mouse = Mouse.current;
        if (mouse != null && mouse.leftButton.isPressed)
        {
            float dx = mouse.delta.ReadValue().x;
            transform.Rotate(Vector3.up, -dx * sensitivity, Space.World);
        }
#endif
    }
}
