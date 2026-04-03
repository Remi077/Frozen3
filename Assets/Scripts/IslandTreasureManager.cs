using UnityEngine;

public class IslandTreasureManager : MonoBehaviour
{
    [Tooltip("Treasure prefab (empty root with chest + mount children)")]
    public GameObject treasurePrefab;

    [Tooltip("Camera to focus on treasure")]
    public CameraController cameraController;

    [Tooltip("Reward button UI")]
    public GameObject rewardButton;

    [Tooltip("Continue button UI")]
    public GameObject continueButton;

    [Tooltip("Spawn treasure with mount visible")]
    public bool spawnWithMount = true;

    [Tooltip("Tag or name pattern to find treasure positions")]
    public string treasurePosTag = "treasurePos";

    private void Start()
    {
        SpawnTreasure();
    }

    public void SpawnTreasure(bool? withMount = null)
    {
        // Find all treasure positions
        Transform[] allPositions = FindObjectsByType<Transform>(FindObjectsSortMode.None);
        System.Collections.Generic.List<Transform> treasurePositions = new System.Collections.Generic.List<Transform>();

        foreach (var t in allPositions)
        {
            if (t.gameObject.name.Contains(treasurePosTag))
                treasurePositions.Add(t);
        }

        if (treasurePositions.Count == 0)
        {
            Debug.LogWarning("No treasure positions found!");
            return;
        }

        // Pick random position
        Transform spawnPos = treasurePositions[Random.Range(0, treasurePositions.Count)];

        // Instantiate prefab
        GameObject treasure = Instantiate(treasurePrefab, spawnPos.position, spawnPos.rotation);

        // Configure mount visibility — only show if shovel is owned
        bool showMount = (withMount ?? spawnWithMount) && Inventory.owned.ContainsKey("shovel") && Inventory.owned["shovel"];
        Transform mountTransform = treasure.transform.Find("DirtMound");
        if (mountTransform != null)
            mountTransform.gameObject.SetActive(showMount);

        // Wire up the controller references
        TreasureController controller = treasure.GetComponentInChildren<TreasureController>();
        if (controller != null)
        {
            Transform cameraTarget = treasure.transform.Find("CameraTarget");
            controller.SetReferences(cameraController, rewardButton, continueButton, cameraTarget);
        }
    }
}
