using UnityEngine;
using UnityEngine.InputSystem;

public class SwipeLaneMovement : MonoBehaviour
{
    public float laneDistance = 2f;
    public float moveSpeed = 10f;

    private int currentLane = 0;

    private PlayerInputActions inputActions;

    private Vector2 startTouch;

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
            startTouch = touch.position.ReadValue();

        if (touch.press.wasReleasedThisFrame)
        {
            Vector2 endTouch = touch.position.ReadValue();
            Vector2 swipe = endTouch - startTouch;

            if (Mathf.Abs(swipe.x) > 50)
            {
                if (swipe.x > 0)
                    MoveRight();
                else
                    MoveLeft();
            }
        }
    }

    void MoveLeft()
    {
        currentLane = Mathf.Max(-1, currentLane - 1);
    }

    void MoveRight()
    {
        currentLane = Mathf.Min(1, currentLane + 1);
    }

    void MovePlayer()
    {
        Vector3 target = new Vector3(currentLane * laneDistance, transform.position.y, transform.position.z);

        transform.position = Vector3.Lerp(
            transform.position,
            target,
            moveSpeed * Time.deltaTime
        );
    }
}