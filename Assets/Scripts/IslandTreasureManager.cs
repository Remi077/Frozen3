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

        // Determine which obstacle to show based on inventory
        bool hasShovel = Inventory.owned.ContainsKey("shovel") && Inventory.owned["shovel"];
        bool hasPickaxe = Inventory.owned.ContainsKey("pickaxe") && Inventory.owned["pickaxe"];

        bool showMound = false;
        bool showRockChest = false;

        if (hasShovel && hasPickaxe)
        {
            if (Random.value < 0.5f) showMound = true;
            else showRockChest = true;
        }
        else if (hasShovel)
            showMound = true;
        else if (hasPickaxe)
            showRockChest = true;

        Transform mountTransform = treasure.transform.Find("DirtMound");
        if (mountTransform != null)
            mountTransform.gameObject.SetActive(showMound);

        Transform rockChestTransform = treasure.transform.Find("RockChest");
        if (rockChestTransform != null)
            rockChestTransform.gameObject.SetActive(showRockChest);

        // Wire up the controller references
        TreasureController controller = treasure.GetComponentInChildren<TreasureController>();
        if (controller != null)
        {
            Transform cameraTarget = treasure.transform.Find("CameraTarget");
            controller.SetReferences(cameraController, rewardButton, continueButton, cameraTarget);
        }
    }
}
