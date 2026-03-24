using UnityEngine;
using UnityEngine.InputSystem;

public class SwipeLaneMovement : MonoBehaviour
{
    public float laneDistance = 2f;
    public float moveSpeed = 10f;

    [HideInInspector] public float targetTilt = 0f;
    public float maxTiltAngle = 20f;

    private int currentLane = 0;
    private PlayerInputActions inputActions;
    private Vector2 startTouch;
    private bool swipeConsumed;

    void Awake()
    {
        inputActions = new PlayerInputActions();
    }

    void OnEnable()
    {
        inputActions.Enable();
    }

    void OnDisable()
    {
        inputActions.Disable();
    }

    void Update()
    {
        HandleKeyboard();
        HandleTouch();
        MovePlayer();
    }

    void HandleKeyboard()
    {
        if (Keyboard.current.leftArrowKey.wasPressedThisFrame)
            MoveLeft();

        if (Keyboard.current.rightArrowKey.wasPressedThisFrame)
            MoveRight();
    }

    void HandleTouch()
    {
        if (Touchscreen.current == null) return;

        var touch = Touchscreen.current.primaryTouch;

        if (touch.press.wasPressedThisFrame)
        {
            startTouch = touch.position.ReadValue();
            swipeConsumed = false;
        }

        // Trigger as soon as threshold is crossed, don't wait for release
        if (!swipeConsumed && touch.press.isPressed)
        {
            Vector2 swipe = touch.position.ReadValue() - startTouch;

            if (Mathf.Abs(swipe.x) > 50)
            {
                if (swipe.x > 0)
                    MoveRight();
                else
                    MoveLeft();

                swipeConsumed = true; // prevent repeated triggers for the same swipe
            }
        }
    }

    void MoveLeft()
    {
        currentLane = Mathf.Max(-1, currentLane - 1);
        targetTilt = maxTiltAngle;
    }

    void MoveRight()
    {
        currentLane = Mathf.Min(1, currentLane + 1);
        targetTilt = -maxTiltAngle;
    }

    void MovePlayer()
    {
        Vector3 target = new Vector3(currentLane * laneDistance, transform.position.y, transform.position.z);
        transform.position = Vector3.Lerp(transform.position, target, moveSpeed * Time.deltaTime);
    }
}
