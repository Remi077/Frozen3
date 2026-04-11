using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using TMPro;
using System.Collections;
using System.Collections.Generic;

[System.Serializable]
public class EasterEggShopItem
{
    public string itemName;
    public Transform cameraTarget;
    public Collider triggerCollider;
}

[System.Serializable]
public class ShopItem
{
    public string itemName;
    public int price;
    public Transform cameraTarget;
    public Renderer itemRenderer;
    public bool isEdible;
    [Tooltip("Life points restored when eaten (capped at max). Only used if isEdible is true.")]
    public int healAmount;
}

public static class Inventory
{
    public static Dictionary<string, bool> owned = new Dictionary<string, bool>
    {
        { "pickaxe", false },
        { "shovel",  false },
    };

    public static void Reset()
    {
        var keys = new System.Collections.Generic.List<string>(owned.Keys);
        foreach (var key in keys)
            owned[key] = false;
    }
}

public class ShopCamera : MonoBehaviour
{
    [Header("Scene")]
    [SerializeField] string sceneName = "SeaScroll";

    [Header("Items")]
    public ShopItem[] items;

    [Header("Easter Egg")]
    public EasterEggShopItem easterEggItem;

    [Header("Camera Timing")]
    public float introHoldDuration = 3f;
    public float moveToFirstItemDuration = 1.5f;
    public float itemTransitionDuration = 0.8f;
    public float itemShrinkDuration = 0.35f;

    [Header("UI")]
    public GameObject popupMessageObject;
    public TextMeshProUGUI itemNameText;
    public TextMeshProUGUI itemPriceText;
    public GameObject itemUI;
    public Button buyButton;
    public Button nextArrowButton;
    public Button prevArrowButton;
    public Button leaveButton;

    [Header("Arrow Animation")]
    public float arrowOscillateScale = 0.15f;
    public float arrowOscillateSpeed = 2f;
    public float arrowShrinkDuration = 0.2f;
    public float arrowScaleUpDuration = 0.3f;

    private int currentIndex = 0;
    private bool inputEnabled = false;
    private Coroutine nextArrowCoroutine;
    private Coroutine prevArrowCoroutine;
    private Vector3 nextArrowBaseScale;
    private Vector3 prevArrowBaseScale;

    private bool inEasterEggMode = false;
    private int lastNormalIndex = 0;
    private Camera cam;
    private readonly HashSet<int> consumedThisSession = new HashSet<int>();

    private Vector2 touchStart;
    private bool swipeConsumed;

    void Start()
    {
        if (itemUI) itemUI.SetActive(false);
        if (leaveButton) leaveButton.gameObject.SetActive(false);

        if (nextArrowButton) { nextArrowBaseScale = nextArrowButton.transform.localScale; nextArrowButton.gameObject.SetActive(false); }
        if (prevArrowButton) { prevArrowBaseScale = prevArrowButton.transform.localScale; prevArrowButton.gameObject.SetActive(false); }

        if (buyButton)      buyButton.onClick.AddListener(OnBuy);
        if (nextArrowButton) nextArrowButton.onClick.AddListener(OnNextItem);
        if (prevArrowButton) prevArrowButton.onClick.AddListener(OnPrevItem);
        if (leaveButton)    leaveButton.onClick.AddListener(OnLeave);

        cam = GetComponent<Camera>();
        if (cam == null) cam = Camera.main;

        StartCoroutine(IntroSequence());
    }

    void Update()
    {
        if (!inputEnabled) return;
        HandleKeyboard();
        HandleTouch();
        HandleEasterEggTap();
    }

    void HandleKeyboard()
    {
        if (Keyboard.current == null) return;
        if (Keyboard.current.rightArrowKey.wasPressedThisFrame) OnNextItem();
        if (Keyboard.current.leftArrowKey.wasPressedThisFrame)  OnPrevItem();
    }

    void HandleTouch()
    {
        if (Touchscreen.current == null) return;
        var touch = Touchscreen.current.primaryTouch;

        if (touch.press.wasPressedThisFrame)
        {
            touchStart = touch.position.ReadValue();
            swipeConsumed = false;
        }

        if (!swipeConsumed && touch.press.isPressed)
        {
            Vector2 delta = touch.position.ReadValue() - touchStart;
            if (Mathf.Abs(delta.x) > 50f)
            {
                if (delta.x < 0f) OnNextItem();
                else              OnPrevItem();
                swipeConsumed = true;
            }
        }
    }

    IEnumerator IntroSequence()
    {
        if (popupMessageObject) popupMessageObject.SetActive(true);

        yield return new WaitForSeconds(introHoldDuration);

        if (items.Length > 0)
            yield return StartCoroutine(MoveCamera(items[0].cameraTarget, moveToFirstItemDuration));

        ShowItemUI(0);
        inputEnabled = true;
    }

    void ShowItemUI(int index)
    {
        if (itemUI) itemUI.SetActive(true);
        if (leaveButton) leaveButton.gameObject.SetActive(true);
        UpdateItemDisplay(index);
        UpdateArrows(index);
    }

    void UpdateItemDisplay(int index)
    {
        ShopItem item = items[index];
        if (itemNameText) itemNameText.text = item.itemName;
        if (itemPriceText) itemPriceText.text = item.price.ToString();

        if (buyButton)
        {
            var label = buyButton.GetComponentInChildren<TextMeshProUGUI>();
            if (item.isEdible)
            {
                bool eaten = consumedThisSession.Contains(index);
                buyButton.interactable = !eaten;
                if (label) label.text = eaten ? "eaten" : "eat";
            }
            else
            {
                string key = item.itemName.ToLower();
                bool alreadyOwned = Inventory.owned.ContainsKey(key) && Inventory.owned[key];
                buyButton.interactable = !alreadyOwned;
                if (label) label.text = alreadyOwned ? "owned" : "buy";
            }
        }
    }

    void UpdateArrows(int index)
    {
        SetArrow(nextArrowButton, index > 0,                   ref nextArrowCoroutine, nextArrowBaseScale);
        SetArrow(prevArrowButton, index < items.Length - 1,    ref prevArrowCoroutine, prevArrowBaseScale);
    }

    void SetArrow(Button arrow, bool show, ref Coroutine coroutine, Vector3 baseScale)
    {
        if (arrow == null) return;
        if (show)
        {
            if (!arrow.gameObject.activeSelf)
            {
                if (coroutine != null) StopCoroutine(coroutine);
                arrow.transform.localScale = Vector3.zero;
                arrow.gameObject.SetActive(true);
                coroutine = StartCoroutine(ScaleUpThenOscillate(arrow.gameObject, baseScale));
            }
        }
        else
        {
            if (arrow.gameObject.activeSelf)
            {
                if (coroutine != null) StopCoroutine(coroutine);
                coroutine = StartCoroutine(ShrinkAndHide(arrow.gameObject, baseScale));
            }
        }
    }

    void HandleEasterEggTap()
    {
        if (inEasterEggMode) return;
        if (easterEggItem == null || easterEggItem.triggerCollider == null || cam == null) return;

        bool tapped = false;
        Vector2 tapPos = Vector2.zero;

        if (Mouse.current != null && Mouse.current.leftButton.wasPressedThisFrame)
        {
            tapped = true;
            tapPos = Mouse.current.position.ReadValue();
        }
        else if (Touchscreen.current != null)
        {
            var touch = Touchscreen.current.primaryTouch;
            if (touch.press.wasReleasedThisFrame && !swipeConsumed)
            {
                tapped = true;
                tapPos = touch.position.ReadValue();
            }
        }

        if (!tapped) return;
        if (UnityEngine.EventSystems.EventSystem.current.IsPointerOverGameObject()) return;

        Ray ray = cam.ScreenPointToRay(tapPos);
        if (Physics.Raycast(ray, out RaycastHit hit) && hit.collider == easterEggItem.triggerCollider)
            StartCoroutine(EnterEasterEgg());
    }

    IEnumerator EnterEasterEgg()
    {
        inputEnabled = false;
        inEasterEggMode = true;
        lastNormalIndex = currentIndex;

        yield return StartCoroutine(MoveCamera(easterEggItem.cameraTarget, itemTransitionDuration));

        if (itemNameText) itemNameText.text = easterEggItem.itemName;
        if (itemPriceText) itemPriceText.text = "";
        if (buyButton) buyButton.gameObject.SetActive(false);

        SetArrow(nextArrowButton, true, ref nextArrowCoroutine, nextArrowBaseScale);
        SetArrow(prevArrowButton, true, ref prevArrowCoroutine, prevArrowBaseScale);

        inputEnabled = true;
    }

    void ExitEasterEgg()
    {
        inEasterEggMode = false;
        if (buyButton) buyButton.gameObject.SetActive(true);
        StartCoroutine(TransitionToItem(lastNormalIndex));
    }

    void OnNextItem()
    {
        if (!inputEnabled) return;
        if (inEasterEggMode) { ExitEasterEgg(); return; }
        if (currentIndex <= 0) return;
        StartCoroutine(TransitionToItem(currentIndex - 1));
    }

    void OnPrevItem()
    {
        if (!inputEnabled) return;
        if (inEasterEggMode) { ExitEasterEgg(); return; }
        if (currentIndex >= items.Length - 1) return;
        StartCoroutine(TransitionToItem(currentIndex + 1));
    }

    IEnumerator TransitionToItem(int newIndex)
    {
        inputEnabled = false;
        currentIndex = newIndex;

        yield return StartCoroutine(MoveCamera(items[newIndex].cameraTarget, itemTransitionDuration));

        UpdateItemDisplay(newIndex);
        UpdateArrows(newIndex);

        inputEnabled = true;
    }

    void OnBuy()
    {
        if (!inputEnabled || items.Length == 0) return;
        ShopItem item = items[currentIndex];

        int score = ScoreManager.Instance != null ? ScoreManager.Instance.score : 0;
        if (score < item.price) { StartCoroutine(NotEnoughCoins()); return; }

        ScoreManager.Instance.AddScore(-item.price);
        if (item.itemRenderer != null) StartCoroutine(ShrinkAndDisableItem(item.itemRenderer));
        buyButton.interactable = false;

        if (item.isEdible)
        {
            consumedThisSession.Add(currentIndex);
            var label = buyButton.GetComponentInChildren<TextMeshProUGUI>();
            if (label) label.text = "eaten";
            if (LiveManager.Instance != null) LiveManager.Instance.AddLife(item.healAmount);
        }
        else
        {
            string key = item.itemName.ToLower();
            Inventory.owned[key] = true;
            foreach (var icon in FindObjectsByType<InventoryIcon>(FindObjectsInactive.Include, FindObjectsSortMode.None))
                icon.Refresh();
            var label = buyButton.GetComponentInChildren<TextMeshProUGUI>();
            if (label) label.text = "owned";
        }
    }

    private Coroutine notEnoughCoroutine;

    IEnumerator NotEnoughCoins()
    {
        if (notEnoughCoroutine != null) StopCoroutine(notEnoughCoroutine);
        var label = buyButton.GetComponentInChildren<TextMeshProUGUI>();
        string original = label.text;
        label.text = "not enough coins";
        yield return new WaitForSeconds(1.5f);
        label.text = original;
        notEnoughCoroutine = null;
    }

    void OnLeave()
    {
        SceneManager.LoadScene(sceneName);
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

    IEnumerator ShrinkAndDisableItem(Renderer r)
    {
        Transform t = r.transform;
        Vector3 originalScale = t.localScale;
        float elapsed = 0f;
        while (elapsed < itemShrinkDuration)
        {
            elapsed += Time.deltaTime;
            t.localScale = Vector3.LerpUnclamped(originalScale, Vector3.zero, Mathf.SmoothStep(0f, 1f, elapsed / itemShrinkDuration));
            yield return null;
        }
        r.enabled = false;
        t.localScale = originalScale;
    }

    IEnumerator ScaleUpThenOscillate(GameObject obj, Vector3 baseScale)
    {
        float t = 0f;
        while (t < 1f)
        {
            t += Time.deltaTime / arrowScaleUpDuration;
            obj.transform.localScale = baseScale * Mathf.SmoothStep(0f, 1f, Mathf.Clamp01(t));
            yield return null;
        }
        while (true)
        {
            float s = 1f + arrowOscillateScale * Mathf.Sin(Time.time * arrowOscillateSpeed * Mathf.PI * 2f);
            obj.transform.localScale = baseScale * s;
            yield return null;
        }
    }

    IEnumerator ShrinkAndHide(GameObject obj, Vector3 baseScale)
    {
        Vector3 current = obj.transform.localScale;
        float t = 0f;
        while (t < 1f)
        {
            t += Time.deltaTime / arrowShrinkDuration;
            obj.transform.localScale = current * (1f - Mathf.SmoothStep(0f, 1f, Mathf.Clamp01(t)));
            yield return null;
        }
        obj.SetActive(false);
        obj.transform.localScale = baseScale;
    }
}
