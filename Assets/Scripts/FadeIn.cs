using UnityEngine;

public class FadeIn : MonoBehaviour
{
    [SerializeField] float fadeStartDistance = 60f; // fully dithered
    [SerializeField] float fadeEndDistance = 40f;   // fully solid

    Renderer[] renderers;
    MaterialPropertyBlock block;
    Camera cam;

    void Awake()
    {
        cam = Camera.main;
        renderers = GetComponentsInChildren<Renderer>();
        block = new MaterialPropertyBlock();
        block.SetFloat("_FadeAmount", 0f);
        foreach (var r in renderers)
            r.SetPropertyBlock(block);
    }

    void Update()
    {
        float dist = Vector3.Distance(transform.position, cam.transform.position);
        float t = Mathf.InverseLerp(fadeStartDistance, fadeEndDistance, dist) * 1.5f;
        block.SetFloat("_FadeAmount", t);
        foreach (var r in renderers)
            r.SetPropertyBlock(block);
    }
}
