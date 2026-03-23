using UnityEngine;
using System.Collections.Generic;

[RequireComponent(typeof(Renderer))]
public class SeaScroller : MonoBehaviour
{
    public static float Speed = 20f;  // change this from anywhere to affect all scrollers
    private static readonly List<SeaScroller> all = new();

    public float resetZ = -150f;
    public float startZ = 150f;

    public bool destroyOnReset = false; // ✅ NEW

    void Awake()
    {
        if (!destroyOnReset) all.Add(this);
    }

    void OnDestroy()
    {
        all.Remove(this);
    }

    void Update()
    {
        transform.position += Vector3.back * Speed * Time.deltaTime;
    }

    void LateUpdate()
    {
        if (transform.position.z <= resetZ)
        {
            if (destroyOnReset)
            {
                Destroy(gameObject); // ✅ destroy instead of looping
            }
            else
            {
                // Place directly behind the rearmost plane
                float furthestZ = float.MinValue;
                SeaScroller furthestPlane = null;
                foreach (var s in all)
                {
                    if (s != this && s.transform.position.z > furthestZ)
                    {
                        furthestZ = s.transform.position.z;
                        furthestPlane = s;
                    }
                }
                // LateUpdate ensures all planes have moved this frame before we read
                // any positions, so the front edge of the furthest plane is accurate
                // and the seam is gap-free.
                float frontEdge = furthestPlane.GetComponent<Renderer>().bounds.max.z;
                float halfLength = GetComponent<Renderer>().bounds.extents.z;
                float newZ = frontEdge + halfLength;
                transform.position = new Vector3(
                    transform.position.x,
                    transform.position.y,
                    newZ
                );
            }
        }
    }
}