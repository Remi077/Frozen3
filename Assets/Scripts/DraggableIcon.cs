using UnityEngine;
using UnityEngine.EventSystems;
using System.Collections;

// Attach to the shovel icon UI element.
// Canvas must have a GraphicRaycaster; EventSystem must exist in scene.
public class DraggableIcon : MonoBehaviour, IBeginDragHandler, IDragHandler, IEndDragHandler
{
    [Tooltip("Item key — must match Inventory.owned")]
    public string itemName = "shovel";

    [Tooltip("Layer mask for the 3D dig mounts")]
    public LayerMask digMountLayer;

    [Tooltip("Spring-back animation duration")]
    public float springBackDuration = 0.3f;

    public static bool isDragging { get; private set; }

    private RectTransform rectTransform;
    private Canvas canvas;
    private Vector2 originalAnchoredPosition;
    private Coroutine springBackCoroutine;

    void Awake()
    {
        rectTransform = GetComponent<RectTransform>();
        canvas = GetComponentInParent<Canvas>();
    }

    void Start()
    {
        // Hide if not owned (works alongside InventoryIcon if present)
        string key = itemName.ToLower();
        bool owned = Inventory.owned.ContainsKey(key) && Inventory.owned[key];
        gameObject.SetActive(owned);
    }

    public void OnBeginDrag(PointerEventData eventData)
    {
        originalAnchoredPosition = rectTransform.anchoredPosition;
        isDragging = true;
        if (springBackCoroutine != null) StopCoroutine(springBackCoroutine);
    }

    public void OnDrag(PointerEventData eventData)
    {
        // Move icon with finger/mouse
        rectTransform.anchoredPosition += eventData.delta / canvas.scaleFactor;
    }

    public void OnEndDrag(PointerEventData eventData)
    {
        // Raycast into the 3D world from the drop screen position
        Ray ray = Camera.main.ScreenPointToRay(eventData.position);
        if (Physics.Raycast(ray, out RaycastHit hit, 100f, digMountLayer))
        {
            var mount = hit.collider.GetComponent<DigMount>();
            if (mount != null)
            {
                mount.Dig(itemName);
                isDragging = false;
                // Spring back after dig
                springBackCoroutine = StartCoroutine(SpringBackToOrigin());
                return;
            }
        }

        // Nothing hit — snap back
        isDragging = false;
        springBackCoroutine = StartCoroutine(SpringBackToOrigin());
    }

    private IEnumerator SpringBackToOrigin()
    {
        Vector2 startPos = rectTransform.anchoredPosition;
        float t = 0f;
        while (t < 1f)
        {
            t += Time.deltaTime / springBackDuration;
            float eased = Mathf.SmoothStep(0f, 1f, Mathf.Clamp01(t));
            rectTransform.anchoredPosition = Vector2.Lerp(startPos, originalAnchoredPosition, eased);
            yield return null;
        }
        rectTransform.anchoredPosition = originalAnchoredPosition;
    }
}
