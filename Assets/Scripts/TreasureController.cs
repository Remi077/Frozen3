using UnityEngine;

[RequireComponent(typeof(Animator))]
public class TreasureController : MonoBehaviour
{
    public CameraController cameraController;

    private Animator animator;
    private IslandOrbit islandOrbit;

    void Awake()
    {
        animator = GetComponent<Animator>();
        islandOrbit = FindAnyObjectByType<IslandOrbit>();
    }

    public void OnTreasureClicked()
    {
        if (islandOrbit != null) islandOrbit.canRotate = false;
        cameraController.FocusOnTreasure();
        Invoke("OpenTreasure", 1.0f); // delay for camera move
    }

    void OpenTreasure()
    {
        animator.SetTrigger("Open");
    }
}