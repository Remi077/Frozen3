using System.Collections;
using UnityEngine;

[RequireComponent(typeof(Animator))]
public class TreasureController : MonoBehaviour
{
    public CameraController cameraController;
    public GameObject rewardText;
    public GameObject rewardButton;
    public float rewardDelay = 1.5f;

    private Animator animator;
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

    public void OnTreasureClicked()
    {
        if (islandOrbit != null) islandOrbit.canRotate = false;
        cameraController.FocusOnTreasure();
        StartCoroutine(OpenSequence());
    }

    IEnumerator OpenSequence()
    {
        yield return waitForOpen;
        animator.SetTrigger("Open");
        if (ScoreManager.Instance != null) ScoreManager.Instance.AddScore(10);
        yield return waitForReward;
        if (rewardText) rewardText.SetActive(true);
        if (rewardButton) rewardButton.SetActive(true);
    }
}