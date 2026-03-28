using UnityEngine;
using UnityEngine.InputSystem;
using System.Collections;

public class VillageIntroCamera : MonoBehaviour
{
    [Header("Camera Positions")]
    public Transform middleView;
    public Transform leftView;
    public Transform rightView;

    [Header("Timing")]
    public float introDelay = 0.5f;
    public float moveToMiddleDuration = 1.5f;
    public float swipeTransitionDuration = 0.8f;

    [Header("UI")]
    public GameObject popupMessageObject;
    public GameObject leftArrow;
    public GameObject rightArrow;
    public GameObject leaveButton;
    public int swipesBeforeLeave = 4;

    [Header("Arrow Animation")]
    public float arrowOscillateScale = 0.15f;
    public float arrowOscillateSpeed = 2f;
    public float arrowShrinkDuration = 0.2f;
    public float arrowScaleUpDuration = 0.3f;

    private enum ViewState { Initial, Middle, Left, Right, Transitioning }
    private ViewState state = ViewState.Initial;

    private Vector2 startTouch;
    private bool swipeConsumed;
    private bool inputEnabled = false;

    private Vector3 leftArrowBaseScale;
    private Vector3 rightArrowBaseScale;
    private Coroutine leftArrowCoroutine;
    private Coroutine rightArrowCoroutine;

    private int swipeCount = 0;
    private bool leaveButtonShown = false;

    void Start()
    {
        if (leftArrow)   { leftArrowBaseScale  = leftArrow.transform.localScale;  leftArrow.SetActive(false); }
        if (rightArrow)  { rightArrowBaseScale = rightArrow.transform.localScale; rightArrow.SetActive(false); }
        if (leaveButton) leaveButton.SetActive(false);

        StartCoroutine(IntroSequence());
    }

    IEnumerator IntroSequence()
    {
        if (popupMessageObject) popupMessageObject.SetActive(true);

        yield return new WaitForSeconds(introDelay);

        yield return StartCoroutine(MoveCamera(middleView, moveToMiddleDuration));

        state = ViewState.Middle;

        ShowArrow(leftArrow,  ref leftArrowCoroutine,  leftArrowBaseScale);
        ShowArrow(rightArrow, ref rightArrowCoroutine, rightArrowBaseScale);

        inputEnabled = true;
    }

    void Update()
    {
        if (!inputEnabled) return;

        HandleKeyboard();
        HandleTouch();
    }

    void HandleKeyboard()
    {
        if (Keyboard.current == null) return;

        if (Keyboard.current.leftArrowKey.wasPressedThisFrame)
            TrySwipeLeft();

        if (Keyboard.current.rightArrowKey.wasPressedThisFrame)
            TrySwipeRight();
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

        if (!swipeConsumed && touch.press.isPressed)
        {
            Vector2 swipe = touch.position.ReadValue() - startTouch;
            if (Mathf.Abs(swipe.x) > 50f)
            {
                if (swipe.x > 0f)
                    TrySwipeLeft();
                else
                    TrySwipeRight();

                swipeConsumed = true;
            }
        }
    }

    void TrySwipeLeft()
    {
        if (state == ViewState.Middle)
            StartCoroutine(SwipeTransition(leftView, ViewState.Left));
        else if (state == ViewState.Right)
            StartCoroutine(SwipeTransition(middleView, ViewState.Middle));
    }

    void TrySwipeRight()
    {
        if (state == ViewState.Middle)
            StartCoroutine(SwipeTransition(rightView, ViewState.Right));
        else if (state == ViewState.Left)
            StartCoroutine(SwipeTransition(middleView, ViewState.Middle));
    }

    IEnumerator SwipeTransition(Transform target, ViewState newState)
    {
        inputEnabled = false;
        state = ViewState.Transitioning;

        swipeCount++;
        if (!leaveButtonShown && swipeCount >= swipesBeforeLeave)
        {
            leaveButtonShown = true;
            if (leaveButton)
            {
                leaveButton.SetActive(true);
                StartCoroutine(ScaleUpThenOscillate(leaveButton, leaveButton.transform.localScale));
            }
        }

        state = newState;
        UpdateArrows();

        yield return StartCoroutine(MoveCamera(target, swipeTransitionDuration));

        inputEnabled = true;
    }

    void UpdateArrows()
    {
        if (state == ViewState.Left)
        {
            HideArrow(leftArrow,  ref leftArrowCoroutine,  leftArrowBaseScale);
        }
        else if (state == ViewState.Right)
        {
            HideArrow(rightArrow, ref rightArrowCoroutine, rightArrowBaseScale);
        }
        else if (state == ViewState.Middle)
        {
            if (leftArrow  && !leftArrow.activeSelf)  ShowArrow(leftArrow,  ref leftArrowCoroutine,  leftArrowBaseScale);
            if (rightArrow && !rightArrow.activeSelf) ShowArrow(rightArrow, ref rightArrowCoroutine, rightArrowBaseScale);
        }
    }

    void ShowArrow(GameObject arrow, ref Coroutine coroutine, Vector3 baseScale)
    {
        if (arrow == null) return;
        if (coroutine != null) StopCoroutine(coroutine);
        arrow.transform.localScale = Vector3.zero;
        arrow.SetActive(true);
        coroutine = StartCoroutine(ScaleUpThenOscillate(arrow, baseScale));
    }

    void HideArrow(GameObject arrow, ref Coroutine coroutine, Vector3 baseScale)
    {
        if (arrow == null) return;
        if (coroutine != null) StopCoroutine(coroutine);
        coroutine = StartCoroutine(ShrinkAndHide(arrow, baseScale));
    }

    IEnumerator ScaleUpThenOscillate(GameObject arrow, Vector3 baseScale)
    {
        float t = 0f;
        while (t < 1f)
        {
            t += Time.deltaTime / arrowScaleUpDuration;
            arrow.transform.localScale = baseScale * Mathf.SmoothStep(0f, 1f, Mathf.Clamp01(t));
            yield return null;
        }

        while (true)
        {
            float s = 1f + arrowOscillateScale * Mathf.Sin(Time.time * arrowOscillateSpeed * Mathf.PI * 2f);
            arrow.transform.localScale = baseScale * s;
            yield return null;
        }
    }

    IEnumerator ShrinkAndHide(GameObject arrow, Vector3 baseScale)
    {
        Vector3 currentScale = arrow.transform.localScale;
        float t = 0f;
        while (t < 1f)
        {
            t += Time.deltaTime / arrowShrinkDuration;
            arrow.transform.localScale = currentScale * (1f - Mathf.SmoothStep(0f, 1f, Mathf.Clamp01(t)));
            yield return null;
        }

        arrow.SetActive(false);
        arrow.transform.localScale = baseScale;
    }

    IEnumerator MoveCamera(Transform target, float duration)
    {
        Vector3 startPos = transform.position;
        Quaternion startRot = transform.rotation;

        float t = 0f;
        while (t < 1f)
        {
            t += Time.deltaTime / duration;
            float eased = Mathf.SmoothStep(0f, 1f, Mathf.Clamp01(t));
            transform.position = Vector3.Lerp(startPos, target.position, eased);
            transform.rotation = Quaternion.Lerp(startRot, target.rotation, eased);
            yield return null;
        }

        transform.position = target.position;
        transform.rotation = target.rotation;
    }
}
