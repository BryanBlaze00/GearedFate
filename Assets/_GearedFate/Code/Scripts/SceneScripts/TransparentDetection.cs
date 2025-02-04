using System.Collections;
using BTG;
using UnityEngine;
using UnityEngine.Tilemaps;

/**
 * This class is responsible for making the object transparent when the player enters the trigger.
 */
public class TransparentDetection : MonoBehaviour
{
    [Range(0, 1)] [SerializeField] private float transparancyAmount = 0.8f;
    [SerializeField] private float fadeTime = 0.4f;

    private SpriteRenderer spriteRenderer;
    private Tilemap tilemap;

    private void Awake()
    {
        this.spriteRenderer = this.GetComponent<SpriteRenderer>();
        this.tilemap = this.GetComponent<Tilemap>();
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.TryGetComponent(out Player _))
        {
            if (this.spriteRenderer != null)
            {
                this.StartCoroutine(this.FadeRoutine(this.spriteRenderer, this.fadeTime, this.spriteRenderer.color.a, this.transparancyAmount));
            }
            else if (this.tilemap != null)
            {
                this.StartCoroutine(this.FadeRoutine(this.tilemap, this.fadeTime, this.tilemap.color.a, this.transparancyAmount));
            }
        }
    }

    private void OnTriggerExit2D(Collider2D other)
    {
        if (other.TryGetComponent(out Player _))
        {
            if (this.spriteRenderer != null && this.gameObject.activeSelf)
            {
                this.StartCoroutine(this.FadeRoutine(this.spriteRenderer, this.fadeTime, this.spriteRenderer.color.a, 1f));
            }
            else if (this.tilemap != null)
            {
                this.StartCoroutine(this.FadeRoutine(this.tilemap, this.fadeTime, this.tilemap.color.a, 1f));
            }
        }
    }


    private IEnumerator FadeRoutine(SpriteRenderer spriteRenderer, float fadeTime, float startValue,
        float targetTransparancy)
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
