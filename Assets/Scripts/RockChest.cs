using UnityEngine;
using System.Collections;

public class RockChest : MonoBehaviour
{
    [Tooltip("The treasure GameObject to reveal when broken")]
    public GameObject treasure;

    [Tooltip("Fade duration in seconds")]
    public float fadeDuration = 0.5f;

    public void HitWithPickaxe()
    {
        if (treasure != null)
            treasure.SetActive(true);

        StartCoroutine(FadeOutAndHide());
    }

    // Shovel drop is intentionally ignored.

    private IEnumerator FadeOutAndHide()
    {
        Renderer[] renderers = GetComponentsInChildren<Renderer>();
        if (renderers.Length == 0)
        {
            gameObject.SetActive(false);
            yield break;
        }

        Material[][] originalMaterials = new Material[renderers.Length][];
        Color[][] originalColors = new Color[renderers.Length][];

        for (int i = 0; i < renderers.Length; i++)
        {
            originalMaterials[i] = renderers[i].materials;
            originalColors[i] = new Color[originalMaterials[i].Length];
            for (int j = 0; j < originalMaterials[i].Length; j++)
                originalColors[i][j] = originalMaterials[i][j].color;
        }

        float t = 0f;
        while (t < 1f)
        {
            t += Time.deltaTime / fadeDuration;
            float alpha = Mathf.Lerp(1f, 0f, Mathf.Clamp01(t));

            for (int i = 0; i < renderers.Length; i++)
                for (int j = 0; j < renderers[i].materials.Length; j++)
                {
                    Color col = originalColors[i][j];
                    col.a = alpha;
                    renderers[i].materials[j].color = col;
                }

            yield return null;
        }

        gameObject.SetActive(false);
    }
}
