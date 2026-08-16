using UnityEngine;

[RequireComponent(typeof(SpriteRenderer))]
public class ExplosionEffect : MonoBehaviour
{
    [Header("Timing")]
    public float duration = 0.4f;

    [Header("Scale")]
    public float startScale = 0.3f;
    public float endScale = 1.8f;

    [Header("Fade")]
    [Range(0f, 1f)] public float startAlpha = 1f;
    [Range(0f, 1f)] public float endAlpha = 0f;

    private SpriteRenderer spriteRenderer;
    private float timer;

    private void Awake()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();
    }

    private void Start()
    {
        transform.localScale = Vector3.one * startScale;
        SetAlpha(startAlpha);
    }

    private void Update()
    {
        timer += Time.deltaTime;
        float t = duration > 0f ? Mathf.Clamp01(timer / duration) : 1f;

        float scale = Mathf.Lerp(startScale, endScale, t);
        transform.localScale = Vector3.one * scale;

        float alpha = Mathf.Lerp(startAlpha, endAlpha, t);
        SetAlpha(alpha);

        if (t >= 1f)
        {
            Destroy(gameObject);
        }
    }

    private void SetAlpha(float alpha)
    {
        if (spriteRenderer == null) return;
        Color c = spriteRenderer.color;
        c.a = alpha;
        spriteRenderer.color = c;
    }
}