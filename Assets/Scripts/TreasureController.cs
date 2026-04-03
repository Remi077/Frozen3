using System.Collections;
using UnityEngine;

[RequireComponent(typeof(Animator))]
public class TreasureController : MonoBehaviour
{
    public float rewardDelay = 1.5f;

    private Animator animator;
    private CameraController cameraController;
    private GameObject rewardButton;
    private GameObject continueButton;
    private IslandOrbit islandOrbit;
    private WaitForSeconds waitForOpen;
    private WaitForSeconds waitForReward;

    void Awake()
    {
        animator = GetComponent<Animator>();
        islandOrbit = FindAnyObjectByType<IslandOrbit>();
        waitForOpen = new WaitForSeconds(1.0f);
        waitForReward = new WaitForSeconds(rewardDelay);
    }

    public void SetReferences(CameraController camera, GameObject button, GameObject continueBtn, Transform cameraTarget)
    {
        cameraController = camera;
        rewardButton = button;
        continueButton = continueBtn;
        if (cameraController != null && cameraTarget != null)
            cameraController.SetTarget(cameraTarget);
    }

    public void OnTreasureClicked()
    {
        if (islandOrbit != null) islandOrbit.canRotate = false;
        if (cameraController != null) cameraController.FocusOnTreasure(transform);
        StartCoroutine(OpenSequence());
    }

    IEnumerator OpenSequence()
    {
        yield return waitForOpen;
        animator.SetTrigger("Open");
        if (ScoreManager.Instance != null) ScoreManager.Instance.AddScore(10);
        yield return waitForReward;
        if (rewardButton) rewardButton.SetActive(true);
        if (continueButton) continueButton.SetActive(true);
    }
}