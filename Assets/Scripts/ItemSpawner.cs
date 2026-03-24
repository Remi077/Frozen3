using UnityEngine;

public class ItemSpawner : MonoBehaviour
{
    public GameObject itemPrefab;

    public float spawnZ = 150f;
    public float laneDistance = 2f;

    public float spawnIntervalMin = 0.5f;
    public float spawnIntervalMax = 1.5f;

    public bool randomYRotation = true;

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

        Quaternion rotation = randomYRotation
            ? Quaternion.Euler(0f, Random.Range(0f, 360f), 0f)
            : itemPrefab.transform.rotation;
        GameObject spawned = Instantiate(itemPrefab, spawnPos, rotation);

        if (randomYRotation)
        {
            Vector3 s = spawned.transform.localScale;
            s.y *= Random.Range(0.7f, 1.3f);
            spawned.transform.localScale = s;
        }

    }

    void SetNextSpawnTime()
    {
        timer = Random.Range(spawnIntervalMin, spawnIntervalMax);
    }
}