using System.Collections;
using BTG;
using UnityEngine;
using UnityEngine.Tilemaps;

/// <summary>
/// This class is responsible for making the object transparent when the player enters the trigger.
/// </summary>
public class TransparentDetection : MonoBehaviour
{
    [Range(0, 1)]
    [SerializeField]
    private float transparancyAmount = 0.8f;
    [SerializeField]
    private float fadeTime = 0.4f;

    private SpriteRenderer spriteRenderer;
    private Tilemap tilemap;

    private void Awake()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();
        tilemap = GetComponent<Tilemap>();
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.TryGetComponent(out Player _))
        {
            if (spriteRenderer != null)
            {
                StartCoroutine(FadeRoutine(spriteRenderer, fadeTime, spriteRenderer.color.a, transparancyAmount));
            }
            else if (tilemap != null)
            {
                StartCoroutine(FadeRoutine(tilemap, fadeTime, tilemap.color.a, transparancyAmount));
            }
        }
    }

    private void OnTriggerExit2D(Collider2D other)
    {
        if (other.TryGetComponent(out Player _))
        {
            if (spriteRenderer != null && gameObject.activeSelf)
            {
                StartCoroutine(FadeRoutine(spriteRenderer, fadeTime, spriteRenderer.color.a, 1f));
            }
            else if (tilemap != null)
            {
                StartCoroutine(FadeRoutine(tilemap, fadeTime, tilemap.color.a, 1f));
            }
        }
    }

    private IEnumerator FadeRoutine(SpriteRenderer spriteRenderer, float fadeTime, float startValue, float targetTransparancy)
    {
        float elapsedTime = 0;
        var baseColor = spriteRenderer.color;

        while (elapsedTime < fadeTime)
        {
            elapsedTime += Time.deltaTime;
            var newAlpha = Mathf.Lerp(startValue, targetTransparancy, elapsedTime / fadeTime);
            spriteRenderer.color = new Color(baseColor.r, baseColor.g, baseColor.b, newAlpha);
            yield return null;
        }
    }

    private IEnumerator FadeRoutine(Tilemap tilemap, float fadeTime, float startValue, float targetTransparancy)
    {
        float elapsedTime = 0;
        var baseColor = tilemap.color;

        while (elapsedTime < fadeTime)
        {
            elapsedTime += Time.deltaTime;
            var newAlpha = Mathf.Lerp(startValue, targetTransparancy, elapsedTime / fadeTime);
            tilemap.color = new Color(baseColor.r, baseColor.g, baseColor.b, newAlpha);
            yield return null;
        }
    }
}
