using UnityEngine;

public class ItemSpawner : MonoBehaviour
{
    public GameObject itemPrefab;

    public float spawnZ = 150f;
    public float laneDistance = 2f;

    public float spawnIntervalMin = 0.5f;
    public float spawnIntervalMax = 1.5f;

    private float timer;

    void Start()
    {
        SetNextSpawnTime();
    }

    void Update()
    {
        timer -= Time.deltaTime;

        if (timer <= 0f)
        {
            SpawnItem();
            SetNextSpawnTime();
        }
    }

    void SpawnItem()
    {
        int lane = Random.Range(-1, 2); // -1, 0, 1

        Vector3 spawnPos = new Vector3(
            lane * laneDistance,
            0.5f,        // height (adjust to your sea)
            spawnZ
        );

        Instantiate(itemPrefab, spawnPos, itemPrefab.transform.rotation);
    }

    void SetNextSpawnTime()
    {
        timer = Random.Range(spawnIntervalMin, spawnIntervalMax);
    }
}