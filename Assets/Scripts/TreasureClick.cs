using UnityEngine;
using UnityEngine.InputSystem;

public class TreasureClick : MonoBehaviour
{
    public TreasureController treasureController;

    void Update()
    {
        Vector2? screenPos = null;

        if (Touchscreen.current != null && Touchscreen.current.primaryTouch.press.wasPressedThisFrame)
            screenPos = Touchscreen.current.primaryTouch.position.ReadValue();
        else if (Mouse.current != null && Mouse.current.leftButton.wasPressedThisFrame)
            screenPos = Mouse.current.position.ReadValue();

        if (screenPos == null) return;

        Ray ray = Camera.main.ScreenPointToRay(screenPos.Value);
        if (Physics.Raycast(ray, out RaycastHit hit) && hit.transform == transform)
            treasureController.OnTreasureClicked();
    }
}