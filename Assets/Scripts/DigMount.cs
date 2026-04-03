using UnityEngine;
using System.Collections;

// Attach to the mound GameObject that hides the treasure.
// Give it a Collider (doesn't need to be trigger).
public class DigMount : MonoBehaviour
{
    [Tooltip("The treasure GameObject to reveal when dug")]
    public GameObject treasure;

    [Tooltip("Fade duration in seconds")]
    public float fadeDuration = 0.5f;

    private bool treasureRevealed = false;

    public void Dig(string itemUsed = "")
    {
        if (!treasureRevealed)
        {
            treasureRevealed = true;
            if (treasure != null)
                treasure.SetActive(true);
        }

        // Only hide if dug with shovel
        if (itemUsed.ToLower() == "shovel")
            StartCoroutine(FadeOutAndHide());
    }

    private IEnumerator FadeOutAndHide()
    {
        Renderer[] renderers = GetComponentsInChildren<Renderer>();
        if (renderers.Length == 0)
        {
            gameObject.SetActive(false);
            yield break;
        }

        // Store original materials and colors
        Material[][] originalMaterials = new Material[renderers.Length][];
        Color[][] originalColors = new Color[renderers.Length][];

        for (int i = 0; i < renderers.Length; i++)
        {
            originalMaterials[i] = renderers[i].materials;
            originalColors[i] = new Color[originalMaterials[i].Length];
            for (int j = 0; j < originalMaterials[i].Length; j++)
            {
                originalColors[i][j] = originalMaterials[i][j].color;
            }
        }

        float t = 0f;
        while (t < 1f)
        {
            t += Time.deltaTime / fadeDuration;
            float alpha = Mathf.Lerp(1f, 0f, Mathf.Clamp01(t));

            for (int i = 0; i < renderers.Length; i++)
            {
                for (int j = 0; j < renderers[i].materials.Length; j++)
                {
                    Color col = originalColors[i][j];
                    col.a = alpha;
                    renderers[i].materials[j].color = col;
                }
            }
            yield return null;
        }

        gameObject.SetActive(false);
    }
}
