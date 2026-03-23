using UnityEngine;

public class SlideDownUI : MonoBehaviour
{
    public Vector2 targetPosition; // where it should end
    public float speed = 5f;

    private RectTransform rectTransform;

    void Start()
    {
        rectTransform = GetComponent<RectTransform>();
    }

    void Update()
    {
        rectTransform.anchoredPosition = Vector2.Lerp(
            rectTransform.anchoredPosition,
            targetPosition,
            speed * Time.deltaTime
        );
    }
}