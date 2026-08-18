using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class ScreenFlash : MonoBehaviour
{
    public static ScreenFlash Instance { get; private set; }

    public Image flashImage;

    private Coroutine activeFlash;

    private void Awake()
    {
        if (Instance != null && Instance != this) { Destroy(gameObject); return; }
        Instance = this;

        if (flashImage != null)
        {
            flashImage.raycastTarget = false;
            SetAlpha(0f);
        }
    }

    public void Flash(float fadeDuration = 0.5f)
    {
        Flash(Color.white, fadeDuration);
    }

    public void Flash(Color color, float fadeDuration = 0.5f)
    {
        if (flashImage == null)
        {
            return;
        }

        if (activeFlash != null) StopCoroutine(activeFlash);
        activeFlash = StartCoroutine(FlashRoutine(color, fadeDuration));
    }

    private IEnumerator FlashRoutine(Color color, float fadeDuration)
    {
        Color c = color;
        c.a = 1f;
        flashImage.color = c;

        float timer = 0f;
        while (timer < fadeDuration)
        {
            timer += Time.deltaTime;
            float alpha = Mathf.Lerp(1f, 0f, timer / fadeDuration);
            SetAlpha(alpha);
            yield return null;
        }

        SetAlpha(0f);
        activeFlash = null;
    }

    private void SetAlpha(float alpha)
    {
        Color c = flashImage.color;
        c.a = alpha;
        flashImage.color = c;
    }
}